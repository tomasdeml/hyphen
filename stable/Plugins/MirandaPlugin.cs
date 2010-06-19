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
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate int Callback(UIntPtr wParam, IntPtr lParam);

    public enum PluginState : int
    {
        Disabled,
        Enabled,
        CrashDisabled
    }

    public abstract partial class MirandaPlugin : ContextWorker, ISettingOwner
    {
        #region Fields

        private MenuItemDeclarationCollection menuItemsCollection;
        private MenuItemDeclarationReadOnlyCollection menuItemsReadOnly;

        private PluginDescriptor descriptor;

        #endregion

        #region .ctors

        protected MirandaPlugin()
        {
            menuItemsCollection = new MenuItemDeclarationCollection();
            menuItemsReadOnly = new MenuItemDeclarationReadOnlyCollection(menuItemsCollection);
        }

        #endregion

        #region Properties

        public abstract string Name { get; }

        public abstract string Author { get; }

        public abstract string Description { get; }

        public abstract Uri HomePage { get; }

        public abstract Version Version { get; }

        public abstract bool HasOptions { get; }

        public MenuItemDeclarationReadOnlyCollection MenuItems
        {
            get { return menuItemsReadOnly; }
        }

        public bool Initialized
        {
            get
            {
                return descriptor != null;
            }
        }

        internal MenuItemDeclarationCollection MenuItemsCollection
        {
            get
            {
                return menuItemsCollection;
            }
        }

        protected internal PluginDescriptor Descriptor
        {
            get
            {
                if (descriptor == null)
                    throw new InvalidOperationException(TextResources.ExceptionMsg_PluginNotInitialized);

                return descriptor;
            }
            internal set
            {
                if (descriptor != null)
                    throw new InvalidOperationException(TextResources.ExceptionMsg_PluginAlreadyInitialized);

                descriptor = value;
            }
        }

        internal string UniqueName
        {
            get
            {
                return String.Format("{0}.{1}.{2}", Author, Name, Version);
            }
        }

        #endregion

        #region Methods

        public sealed override int GetHashCode()
        {
            return GetType().FullName.GetHashCode();
        }

        public sealed override bool Equals(object obj)
        {
            if (obj == null) return false;
            MirandaPlugin other = obj as MirandaPlugin;

            if (other == null) return false;
            return GetHashCode() == other.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} by {1}, v{2}", Name, Author, Version);
        }

        internal static IExceptionHandler GetExceptionHandler(PluginDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            // Custom handler (i.e. Hyphen's)
            if (descriptor.Plugin is IExceptionHandler)
                return (IExceptionHandler)descriptor.Plugin;
            // Isolated plugin crashed
            else if (!descriptor.IsStandalone)
                return MirandaContext.Current.PluginManager;
            // Generic handler
            else
                return DefaultExceptionHandler.Create(descriptor.Plugin);
        }

        #endregion

        #region Events

        internal virtual void AfterMenuItemsPopulationInternal(MenuItemDeclarationCollection items) { AfterMenuItemsPopulation(items); }
        protected virtual void AfterMenuItemsPopulation(MenuItemDeclarationCollection items) { }

        internal virtual void BeforeMirandaShutdownInternal() { BeforeMirandaShutdown(); }
        protected virtual void BeforeMirandaShutdown() { }

        internal virtual void BeforePluginDisableInternal() { BeforePluginDisable(); }
        protected virtual void BeforePluginDisable() { }

        internal virtual void AfterPluginEnableInternal() { AfterPluginEnable(); }
        protected virtual void AfterPluginEnable() { }

        internal virtual void AfterPluginInitializationInternal() { AfterPluginInitialization(); }
        protected virtual void AfterPluginInitialization() { }

        #endregion
    }
}
