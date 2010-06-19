/***********************************************************************\
 * Virtuoso.Miranda.Plugins (Hyphen)                                   *
 * Provides a managed wrapper for API of IM client Miranda.            *
 * Copyright (C) 2006-2009 virtuoso                                    *
 *                    deml.tomas@seznam.cz                             *
 *                                                                     *
 * This library is free software; you can redistribute it and/or       *
 * modify it under the terms of the GNU Lesser General Public          *
 * License as published by the Free Software Foundation; either        *
 * version 2.1 of the License, or (at your option) any later version.  *
 *                                                                     *
 * This library is distributed in the hope that it will be useful,     *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of      *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU   *
 * Lesser General Public License for more details.                     *
\***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Collections;
using System.Reflection;
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Hyphen.Mini;

namespace Virtuoso.Miranda.Plugins
{
    [DebuggerDisplay("{Plugin}")]
    public sealed class PluginDescriptor : IDescriptor
    {
        #region Fields

        private static readonly Type HookDescriptorType = typeof(HookDescriptor),
            EventHookAttribType = typeof(EventHookAttribute),
            MenuItemAttribType = typeof(MenuItemDeclarationAttribute),
            ServiceFncAttribType = typeof(ServiceFunctionAttribute),
            CallbackDelegType = typeof(Callback);

        private readonly MirandaPlugin plugin;
        private PluginState PluginStateInternal;

        private readonly HookDescriptorCollection eventHooks, serviceFunctions;
        private readonly EventHandleCollection eventHandles;

        #endregion

        #region .ctors

        private PluginDescriptor(MirandaPlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            this.plugin = plugin;
            
            this.eventHooks = new HookDescriptorCollection();
            this.serviceFunctions = new HookDescriptorCollection();
            this.eventHandles = new EventHandleCollection();

            Initialize();
        }

        private void Initialize()
        {
            Plugin.Descriptor = this;

            Type pluginType = Plugin.GetType();
            Assembly pluginAssembly = pluginType.Assembly;

            MethodInfo[] methods = pluginType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (MethodInfo method in methods)
            {
                PopulateMethodHooksByAttribute<EventHookAttribute>(method, EventHooks);
                PopulateMethodHooksByAttribute<ServiceFunctionAttribute>(method, ServiceFunctions);

                PopulateMethodLevelDeclaredMenuItems(method);
            }

            PopulateTopLevelDeclaredMenuItems();
            Plugin.AfterMenuItemsPopulationInternal(Plugin.MenuItemsCollection);

            Plugin.AfterPluginInitializationInternal();
        }

        internal static PluginDescriptor SetUp(MirandaPlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            try
            {
                return new PluginDescriptor(plugin);
            }
            catch (FusionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_UnableToSetUpPluginDescriptor, e.Message), plugin.GetType().Assembly, plugin.GetType(), plugin, e);
            }
        }

        #endregion

        #region Helpers

        private void PopulateMethodHooksByAttribute<TAttrib>(MethodInfo method, HookDescriptorCollection hookBag) where TAttrib : HookAttribute
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (hookBag == null)
                throw new ArgumentNullException("hookBag");

            Type attribType = typeof(TAttrib);

            if (method.IsDefined(attribType, true))
            {
                TAttrib[] attributes = (TAttrib[])Attribute.GetCustomAttributes(method, attribType, true);

                if (attributes.Length > 0)
                    PopulateMethodHooks<TAttrib>(method, hookBag, attributes);
            }
        }

        private void PopulateMethodHooks<TAttrib>(MethodInfo method, HookDescriptorCollection hookBag, params TAttrib[] attributes) where TAttrib : HookAttribute
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (hookBag == null)
                throw new ArgumentNullException("hookBag");

            if (attributes == null)
                throw new ArgumentNullException("attributes");

            if (attributes.Length == 0)
                return;

            Callback hookCallback = Delegate.CreateDelegate(CallbackDelegType, Plugin, method, false) as Callback;

            if (hookCallback == null)
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_InvalidMethodSignature, method.Name), Plugin.GetType().Assembly, Plugin.GetType(), Plugin, null);

            foreach (TAttrib attribute in attributes)
            {
                HookDescriptor.SetUpAndStore(hookBag, attribute.HookName, this, hookCallback, attribute.HookType);
            }
        }

        private void PopulateTopLevelDeclaredMenuItems()
        {
            Type pluginType = Plugin.GetType();

            if (pluginType.IsDefined(MenuItemAttribType, true))
            {
                MenuItemDeclarationAttribute[] menuItemAttribs = (MenuItemDeclarationAttribute[])pluginType.GetCustomAttributes(MenuItemAttribType, true);
                Plugin.MenuItemsCollection.AddRange(menuItemAttribs);
            }
        }

        private void PopulateMethodLevelDeclaredMenuItems(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (method.IsDefined(MenuItemAttribType, true))
            {
                string methodService = String.Format("{0}.{1}", Plugin.GetType().FullName, method.Name);
                MenuItemDeclarationAttribute[] menuItems = (MenuItemDeclarationAttribute[])method.GetCustomAttributes(MenuItemAttribType, true);

                bool serviceRegistered = false;
                
                foreach (MenuItemDeclarationAttribute menuItem in menuItems)
                {
                    if (!String.IsNullOrEmpty(menuItem.Service))
                        throw new InvalidOperationException(String.Format(TextResources.ExceptionMsg_Formatable3_MenuItemServiceAlreadySet, menuItem.Text, methodService, menuItem.Service));

                    if (!serviceRegistered)
                    {
                        PopulateMethodHooks<ServiceFunctionAttribute>(method, ServiceFunctions, new ServiceFunctionAttribute(methodService));
                        serviceRegistered = true;
                    }

                    menuItem.Service = methodService;
                    Plugin.MenuItemsCollection.Add(menuItem);
                }
            }
        }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return plugin.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PluginDescriptor))
                return false;

            return plugin.Equals(((PluginDescriptor)obj).plugin);
        }

        public void SetPluginState(PluginState newState)
        {
            SetPluginState(newState, false);
        }

        public void SetPluginState(PluginState newState, bool rememberState)
        {
            MirandaContext.Current.PluginManager.SetPluginState(this, newState, rememberState);
        }

        internal void AssociateHook(HookDescriptor hook)
        {
            if (hook == null)
                throw new ArgumentNullException("hook");

            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(this);

                switch (hook.HookType)
                {
                    case HookType.EventHook:
                        eventHooks.Add(hook);
                        break;
                    case HookType.ServiceFunction:
                        serviceFunctions.Add(hook);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("hook");
                }
            }
            finally
            {
                SynchronizationHelper.EndUpdate(this);
            }
        }

        internal void UpdatePluginState(PluginState state)
        {
            if (!Enum.IsDefined(typeof(PluginState), state))
                throw new ArgumentOutOfRangeException("state");

            PluginStateInternal = state;
        }

        #endregion

        #region Properties

        public PluginState PluginState
        {
            get
            {
                return PluginStateInternal;
            }
        }        

        public MirandaPlugin Plugin
        {
            get
            {
                return plugin;
            }
        }

        public bool IsStandalone
        {
            get
            {
                return plugin is StandalonePlugin;
            }
        }

        internal bool IsConfigurable
        {
            get
            {
                return plugin.HasOptions && plugin is IConfigurablePlugin;
            }
        }

        internal HookDescriptorCollection EventHooks
        {
            get
            {
                return eventHooks;
            }
        }

        internal HookDescriptorCollection ServiceFunctions
        {
            get
            {
                return serviceFunctions;
            }
        }

        internal EventHandleCollection EventHandles
        {
            get
            {
                return eventHandles;
            }
        }

        #endregion
    }
}
