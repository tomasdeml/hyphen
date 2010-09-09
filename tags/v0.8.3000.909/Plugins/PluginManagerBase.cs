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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Virtuoso.Hyphen;
using Virtuoso.Hyphen.Mini;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins
{
    [CLSCompliant(false)]
    public abstract class PluginManagerBase : ContextWorker, IExceptionHandler
    {
        #region Constants

        internal const string LogCategory = Loader.LogCategory + "::PluginManagerBase";

        #endregion

        #region Fields

        protected internal static readonly Type PluginType = typeof(MirandaPlugin);
        protected internal static readonly Type ExposingPluginAttributeType = typeof(ExposingPluginAttribute);

        private bool initialized;
        private readonly PluginDescriptorCollection pluginDescriptors;
        private readonly AppDomain livingDomain;
        private readonly PluginDescriptorReadOnlyCollection pluginDescriptorsAsReadOnly;
        private readonly FusionContext fusionContext;

        #endregion

        #region .ctors

        protected PluginManagerBase(FusionContext fusionContext) : this(fusionContext, true, true) { }

        internal PluginManagerBase(FusionContext fusionContext, bool initializeMirandaContext, bool initializeConfiguration)
        {
            if (fusionContext == null)
                throw new ArgumentNullException("fusionContext");

            this.livingDomain = AppDomain.CurrentDomain;
            this.fusionContext = fusionContext;

            this.pluginDescriptors = new PluginDescriptorCollection();
            this.pluginDescriptorsAsReadOnly = new PluginDescriptorReadOnlyCollection(this.pluginDescriptors);

            if (initializeMirandaContext)
            {
                if (!fusionContext.IsInvalid)
                {
                    IntPtr pluginLink = fusionContext.NativePluginLink;

                    // Invalidate IsolatedPluginsSandbox's AppDomain context available to managed plugins
                    MirandaContext.InvalidateCurrent();

                    /* Why am I marshaling the link again? 'Cause MirandaPluginLink resides in another AppDomain 
                     * and se/de-serialization of it would be much slower */
                    MirandaContext.InitializeCurrent(MirandaPluginLink.FromPointer(pluginLink), this);
                }
                else
                    throw new ArgumentException("fusionContext");
            }

            if (initializeConfiguration)
                PMConfiguration.Initialize();
        }

        #endregion

        #region Events & delegates

        public delegate void PluginManagerContextCallback<T>(PluginManagerBase sender, T state);

        public static event EventHandler PrimaryPluginManagerInitialized;
        public event EventHandler FusionCompleted;
        public event EventHandler<PluginStateChangeEventArgs> PluginStateChange;

        protected static void FirePrimaryPluginManagerInitializedEvent(PluginManagerBase sender, EventArgs e)
        {
            if (PrimaryPluginManagerInitialized != null)
                PrimaryPluginManagerInitialized(sender, e);
        }

        protected void RaiseFusionCompletedEvent(EventArgs e)
        {
            if (FusionCompleted != null)
                FusionCompleted(this, e);
        }

        protected void FirePluginStateChangeEvent(PluginStateChangeEventArgs e)
        {
            if (PluginStateChange != null)
                PluginStateChange(this, e);
        }

        #endregion

        #region Properties

        protected bool Initialized
        {
            get
            {
                return initialized;
            }
        }

        protected PluginDescriptorCollection PluginDescriptors
        {
            get { return pluginDescriptors; }
        }

        protected AppDomain LivingDomain
        {
            get
            {
                return livingDomain;
            }
        }

        public PluginDescriptorReadOnlyCollection Plugins
        {
            get
            {
                return pluginDescriptorsAsReadOnly;
            }
        }

        public FusionContext FusionContext
        {
            get
            {
                return fusionContext;
            }
        }

        #endregion

        #region Fusion

        protected internal abstract void FindAndLoadPlugins();

        protected internal static Type[] GetExposedPlugins(Assembly assembly)
        {
            if (!assembly.IsDefined(ExposingPluginAttributeType, false))
                return new Type[0];

            return Array.ConvertAll<ExposingPluginAttribute, Type>((ExposingPluginAttribute[])assembly.GetCustomAttributes(PluginManagerBase.ExposingPluginAttributeType, false),
                delegate(ExposingPluginAttribute attrib)
                {
                    return attrib.PluginType;
                });
        }

        protected void DeclareInitialized()
        {
            initialized = true;
        }

        protected virtual void AccountPluginDescriptor(PluginDescriptor pluginDescriptor)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException("pluginDescriptor");

            try
            {
                SynchronizationHelper.BeginCollectionUpdate(pluginDescriptors);

                if (pluginDescriptors.ContainsDescriptorOf(pluginDescriptor.Plugin))
                    throw new InvalidOperationException(TextResources.ExceptionMsg_PluginAlreadyInitialized);

                pluginDescriptors.Add(pluginDescriptor);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(pluginDescriptors);
            }
        }

        protected internal static MirandaPlugin InstantiatePlugin(Type type)
        {
            return InstantiatePlugin(type, false);
        }

        internal static MirandaPlugin InstantiatePlugin(Type type, bool acceptIndividualPlugins)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsSubclassOf(PluginType) || (!acceptIndividualPlugins && type.IsSubclassOf(StandalonePlugin.PluginType)))
                return null;

            LoaderOptionsAttribute loaderOptions = LoaderOptionsAttribute.Get(type, LoaderOptionsOwner.Type);

            if (loaderOptions.RequiredVersion > Loader.HyphenVersion)
                throw new RuntimeNotSupportedException(type, loaderOptions.RequiredVersion);

            if (!loaderOptions.SupportsMirandaVersion(MirandaEnvironment.MirandaVersion))
                throw new RuntimeNotSupportedException(type, loaderOptions.MinimalMirandaVersion, false);

            return (MirandaPlugin)Activator.CreateInstance(type, true);
        }

        protected static void RegisterMenuItems(PluginDescriptor pluginDescriptor)
        {
            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(pluginDescriptor);
                MirandaPlugin owner = pluginDescriptor.Plugin;

                ContactList list = MirandaContext.Current.ContactList;

                foreach (MenuItemDeclarationAttribute menuItemAttrib in owner.MenuItemsCollection)
                    list.AddMenuItem(owner, menuItemAttrib);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(pluginDescriptor);
            }
        }

        protected static void UnregisterMenuItems(PluginDescriptor pluginDescriptor)
        {
            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(pluginDescriptor);
                MirandaPlugin owner = pluginDescriptor.Plugin;

                ContactList list = MirandaContext.Current.ContactList;

                foreach (MenuItemDeclarationAttribute menuItemAttrib in owner.MenuItems)
                {
                    bool result = list.ModifyMenuItem(owner, menuItemAttrib, null, MenuItemProperties.Hidden, null, 0, false);
                    Debug.Assert(result);
                }
            }
            finally
            {
                SynchronizationHelper.EndUpdate(pluginDescriptor);
            }
        }

        protected void HookPlugin(PluginDescriptor pluginDescriptor)
        {
            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(pluginDescriptor);

                MirandaContext context = MirandaContext.Current;

                HookManager.CreateHooks(pluginDescriptor.ServiceFunctions.ToArray());
                HookManager.CreateHooks(pluginDescriptor.EventHooks.ToArray());
            }
            finally
            {
                SynchronizationHelper.EndUpdate(pluginDescriptor);
            }
        }

        #endregion

        #region Management

        public void DoContextCallback<T>(PluginManagerContextCallback<T> del, T state)
        {
            if (del == null)
                throw new ArgumentNullException("del");

            del(this, state);
        }

        public virtual void SetPluginState(PluginDescriptor pluginDescriptor, PluginState newState)
        {
            SetPluginState(pluginDescriptor, newState, false);
        }

        public virtual void SetPluginState(PluginDescriptor pluginDescriptor, PluginState newState, bool rememberState)
        {
            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(pluginDescriptor);
                PluginState previousState = pluginDescriptor.PluginState;

                if (previousState == newState || previousState == PluginState.CrashDisabled && newState != PluginState.Enabled)
                    return;

                pluginDescriptor.UpdatePluginState(newState);
                FirePluginStateChangeEvent(new PluginStateChangeEventArgs(previousState, newState));

                if (newState == PluginState.Enabled)
                    EnablePlugin(pluginDescriptor, rememberState);
                else
                    DisablePlugin(pluginDescriptor, rememberState);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(pluginDescriptor);
            }
        }

        public PluginDescriptor LoadPlugin(MirandaPlugin plugin)
        {
            return LoadPlugin(plugin, true);
        }

        #region Internals

        private void EnablePlugin(PluginDescriptor pluginDescriptor, bool rememberState)
        {
            HookPlugin(pluginDescriptor);
            RegisterMenuItems(pluginDescriptor);

            pluginDescriptor.Plugin.AfterPluginEnableInternal();

            if (rememberState)
            {
                List<string> disabledPlugins = PMConfiguration.Singleton.DisabledPlugins;

                lock (disabledPlugins)
                    disabledPlugins.Remove(pluginDescriptor.Plugin.UniqueName);
            }
        }

        private void DisablePlugin(PluginDescriptor pluginDescriptor, bool rememberState)
        {
            pluginDescriptor.Plugin.BeforePluginDisableInternal();

            PluginDialog.CloseDialogs(pluginDescriptor, true);
            UnregisterMenuItems(pluginDescriptor);

            lock (MirandaContext.Current.PluginLink)
            {
                UnhookEvents(pluginDescriptor);
                DestroyServices(pluginDescriptor);
                DestroyEvents(pluginDescriptor);
            }

            if (rememberState)
            {
                string typeName = pluginDescriptor.Plugin.UniqueName;
                List<string> disabledPlugins = PMConfiguration.Singleton.DisabledPlugins;

                lock (disabledPlugins)
                    if (!disabledPlugins.Contains(typeName))
                        disabledPlugins.Add(typeName);
            }
        }

        protected bool IsEnabled(MirandaPlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            List<string> disabledPlugins = PMConfiguration.Singleton.DisabledPlugins;

            lock (disabledPlugins)
                return !disabledPlugins.Contains(plugin.UniqueName);
        }

        private static void DestroyEvents(PluginDescriptor pluginDescriptor)
        {
            foreach (EventHandle handle in pluginDescriptor.EventHandles)
            {
                try
                {
                    SynchronizationHelper.BeginHandleUpdate(handle);
                    EventManager.RemoveEvent(handle);
                }
                finally
                {
                    SynchronizationHelper.EndUpdate(handle);
                }
            }
        }

        private static void DestroyServices(PluginDescriptor pluginDescriptor)
        {
            int result = 0;
            MirandaContext context = MirandaContext.Current;

            foreach (HookDescriptor hook in pluginDescriptor.ServiceFunctions)
                HookManager.DestroyHook(hook);
        }

        private static void UnhookEvents(PluginDescriptor pluginDescriptor)
        {
            int result = 0;
            MirandaContext context = MirandaContext.Current;

            foreach (HookDescriptor hookDesc in pluginDescriptor.EventHooks)
                HookManager.DestroyHook(hookDesc);
        }

        protected internal virtual void Shutdown(bool lazy)
        {
            try
            {
                Log.DebuggerWrite(0, LogCategory, "Shutting down Plugin Manager...");
                SynchronizationHelper.BeginCollectionUpdate(this.pluginDescriptors);

                PMConfiguration.Singleton.Save();                

                if (!lazy)
                {
                    MirandaContext.Current.RaiseIsolatedModePluginsUnloadingEvent();

                    foreach (PluginDescriptor pluginDescriptor in this.pluginDescriptors)
                        SetPluginState(pluginDescriptor, PluginState.Disabled);
                }
            }
            finally
            {
                SynchronizationHelper.EndUpdate(this.pluginDescriptors);
                Log.DebuggerWrite(0, LogCategory, "Plugin Manager was shut down; all managed plugins were disabled");
            }
        }

        protected internal virtual PluginDescriptor LoadPlugin(MirandaPlugin plugin, bool accountDescriptor)
        {
            PluginDescriptor descriptor = PluginDescriptor.SetUp(plugin);

            if (accountDescriptor)
                AccountPluginDescriptor(descriptor);

            return descriptor;
        }

        public virtual void HandleException(Exception e, PluginDescriptor descriptor)
        {
            if (descriptor != null)
            {
                if (DialogResult.OK == ErrorDialog.PresentModal(e, DefaultExceptionHandler.Create(descriptor.Plugin), String.Format(TextResources.MsgBox_Formatable2_Text_PluginError, Environment.NewLine, descriptor.Plugin.Name, e.Message), true))
                    descriptor.SetPluginState(PluginState.CrashDisabled);
            }
            else
            {
                DefaultExceptionHandler.Create().HandleException(e, descriptor);
            }
        }

        #endregion

        #endregion
    }
}
