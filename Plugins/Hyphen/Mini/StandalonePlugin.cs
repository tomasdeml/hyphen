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
using Virtuoso.Miranda.Plugins;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Hyphen.Mini.Custom;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Forms.Controls;
using Virtuoso.Hyphen.Native;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Hyphen.Mini
{   
    public abstract class StandalonePlugin : MirandaPlugin
    {
        #region Fields

        internal static readonly Type PluginType = typeof(StandalonePlugin);

        private readonly CustomApiExportDescriptorCollection customApiHandlers;
        private Module module;

        #endregion

        #region .ctors

        protected StandalonePlugin()
        {
            customApiHandlers = new CustomApiExportDescriptorCollection();
        }

        #endregion

        #region Properties

        protected internal Module Module
        {
            get { return this.module; }
            internal set { this.module = value; }
        }

        public abstract string Copyright { get; }

        public abstract string AuthorEmail { get; }

        public virtual PluginFlags Flags { get { return PluginFlags.UnicodeAware; } }

        public abstract int ReplacesDefaultModule { get; }

        public abstract Guid UUID { get; }

        public abstract Guid[] PluginInterfaces { get; }

        internal CustomApiExportDescriptorCollection CustomApiHandlers
        {
            get { return customApiHandlers; }
        }

        private bool hasCustomPluginInterfaces = true;
        internal bool HasCustomPluginInterfaces
        {
            get { return hasCustomPluginInterfaces; }
        }

        private bool hasCustomPluginInfo = true;
        internal bool HasCustomPluginInfo
        {
            get { return hasCustomPluginInfo; }
        }

        #endregion

        #region Methods

        /* Impl note: We use this to unload a plugin instead of the Unload export because there were some Access
         * Violation exceptions before */
        [EventHook(MirandaEvents.ME_SYSTEM_OKTOEXIT)]
        internal int BeforeMirandaShutdownTriggerService(UIntPtr wParam, IntPtr lParam)
        {
            BeforeMirandaShutdown();
            module.Unload();

            return 0;
        }

        [CLSCompliant(false)]
        protected internal virtual IntPtr MirandaPluginInfo(uint version, bool ex)
        {
            hasCustomPluginInfo = false;
            PLUGININFO info = ex ? new PLUGININFOEX() : new PLUGININFO();

            info.Size = Marshal.SizeOf(info.GetType());
            info.Author = Author;
            info.AuthorEmail = AuthorEmail;
            info.Copyright = Copyright;
            info.Description = Description;
            info.HomePage = HomePage == null ? String.Empty : HomePage.ToString();
            info.Flags = (byte)Flags;
            info.ReplacesDefaultModule = ReplacesDefaultModule;
            info.ShortName = Name;
            info.Version = Translate.ToMirandaVersion(Version);
            if (ex) ((PLUGININFOEX)info).UUID = UUID;

            IntPtr pInfo = Marshal.AllocHGlobal(info.Size);
            Marshal.StructureToPtr(info, pInfo, false);

            return pInfo;
        }

        protected internal virtual IntPtr MirandaPluginInterfaces()
        {
            hasCustomPluginInterfaces = false;
            Guid[] interfaces = PluginInterfaces;

            int uuidSize = Marshal.SizeOf(typeof(Guid));
            IntPtr pInterfaces = Marshal.AllocHGlobal((interfaces.Length + 1) * uuidSize);

            byte[] uuidBytes = null;
            long baseAddr = pInterfaces.ToInt64();

            for (int i = 0; i < interfaces.Length; i++)
            {
                uuidBytes = interfaces[i].ToByteArray();
                Marshal.Copy(uuidBytes, 0, new IntPtr(baseAddr + i * uuidSize), uuidBytes.Length);
            }

            // MIID_LAST
            uuidBytes = Virtuoso.Miranda.Plugins.Native.UUID.Last.ToByteArray();
            Marshal.Copy(uuidBytes, 0, new IntPtr(baseAddr + interfaces.Length * uuidSize), uuidBytes.Length);

            return pInterfaces;
        }

        internal virtual void AfterModuleInitializationInternal() { AfterModuleInitialization(); }
        protected virtual void AfterModuleInitialization() { }

        internal virtual void LoadInternal(IntPtr pPluginLink) { Load(pPluginLink); }
        protected virtual void Load(IntPtr pPluginLink) { }

        internal virtual void UnloadInternal() { Unload(); }
        protected virtual void Unload() { }

        #endregion        
    }
}
