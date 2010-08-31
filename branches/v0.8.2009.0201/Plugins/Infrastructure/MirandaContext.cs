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
using System.Runtime.InteropServices;
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Miranda.Plugins.Collections;
using System.Collections.ObjectModel;
using Virtuoso.Hyphen;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public sealed class MirandaContext
    {
        #region Constants

        private const string MS_SYSTEM_GETVERSIONTEXT = "Miranda/System/GetVersionText";

        /// <summary>
        /// Returns Miranda's RTL/CRT function poiners to malloc() free() realloc() -- 0.1.2.2+
        /// This is useful for preallocation of memory for use with Miranda's services
        /// that Miranda can free -- or reallocation of a block of memory passed with a service.
        /// Do not use with memory unless it is explicitly expected the memory *can*
        /// or *shall* be used in this way. The passed structure is expected to have it's .cbSize initialised
        /// wParam=0, lParam = (LPARAM) &MM_INTERFACE.
        /// </summary>
        private const string MS_SYSTEM_GET_MMI = "Miranda/System/GetMMI";

        #endregion

        #region Fields

        private static MirandaContext singleton;
        private PluginManagerBase pluginManager;

        private readonly ServiceCallInterceptionManager serviceInterceptors;
        private readonly MirandaDatabase mirandaDatabase;
        private ProtocolDictionary protocols;
        private readonly MirandaPluginLink pluginLink;
        private readonly ContactList contactList;

        #endregion

        #region .ctors

        private MirandaContext(PluginManagerBase pluginManager, MirandaPluginLink mirandaLink, bool skipContextInfo)
        {
            if (mirandaLink == null)
                throw new ArgumentNullException("mirandaLink");

            this.pluginManager = pluginManager;
            this.mirandaDatabase = new MirandaDatabase();
            this.pluginLink = mirandaLink;
            this.contactList = new ContactList();
            this.serviceInterceptors = new ServiceCallInterceptionManager();

            GetMMInterface();

            PopulateEnvironmentInformation();

            if (!skipContextInfo)
                PopulateContextInformation();
            else
                this.protocols = new ProtocolDictionary(0);
        }

        /// <summary>
        /// Initializes a context from a plugin link.
        /// </summary>
        /// <param name="mirandaLink"></param>
        /// <param name="skipContextPopulation"></param>
        internal static void InitializeCurrent(MirandaPluginLink mirandaLink, bool skipContextPopulation)
        {
            InitializeCurrent(mirandaLink, null, skipContextPopulation);
        }

        internal static void InitializeCurrent(MirandaPluginLink mirandaLink, PluginManagerBase pluginManager)
        {
            InitializeCurrent(mirandaLink, pluginManager, false);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void InitializeCurrent(MirandaPluginLink mirandaLink, PluginManagerBase pluginManager, bool skipContextPopulation)
        {
            if (singleton == null)
                singleton = new MirandaContext(pluginManager, mirandaLink, skipContextPopulation);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void InitializeCurrent(MirandaContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (singleton == null)
                singleton = context;
            else
                throw new InvalidOperationException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void InvalidateCurrent()
        {
            if (Initialized)
            {
                singleton.DetachPluginManager();
                singleton = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void AssociatePluginManager(PluginManagerBase manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            pluginManager = manager;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void DetachPluginManager()
        {
            pluginManager = null;
        }

        #endregion

        #region Initialization

        private void GetMMInterface()
        {
            mirandaMemoryManager = new MM_INTERFACE();
            mirandaMemoryManager.Size = Marshal.SizeOf(typeof(MM_INTERFACE));

            UnmanagedStructHandle<MM_INTERFACE> mmiHandle = new UnmanagedStructHandle<MM_INTERFACE>(ref mirandaMemoryManager);

            try
            {
                if (CallService(MS_SYSTEM_GET_MMI, IntPtr.Zero, mmiHandle.IntPtr) == CallbackResult.Success)
                    mmiHandle.MarshalBack(out mirandaMemoryManager);
                else
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_SYSTEM_GET_MMI, "1"));
            }
            finally
            {
                mmiHandle.Free();
            }
        }

        internal void PopulateContextInformation()
        {
            PopulateNetworkProtocols();
        }

        private unsafe void PopulateNetworkProtocols()
        {
            try
            {
                int count;
                PROTOCOLDESCRIPTOR** pointerArrayPtr;

                int result = CallServiceUnsafe(MirandaServices.MS_PROTO_ENUMPROTOCOLS, &count, &pointerArrayPtr);
                if (result != 0) throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MirandaServices.MS_PROTO_ENUMPROTOCOLS, result.ToString()));

                ProtocolDictionary protocols = new ProtocolDictionary(count);

                for (int i = 0; i < count; i++)
                {
                    // *(ptr_to_array_of_ptrs + i * sizeof(PROTOCOLDESCRIPTOR)) = *ptr_to_ptr = *ptr = data
                    PROTOCOLDESCRIPTOR nativeDescriptor = **(((PROTOCOLDESCRIPTOR**)pointerArrayPtr) + i);
                    Protocol protocol = new Protocol(ref nativeDescriptor);

                    protocols.Add(protocol.Name, protocol);
                }

                this.protocols = protocols;
            }
            catch (Exception)
            {
                this.protocols = new ProtocolDictionary(0);
            }
        }

        private void PopulateEnvironmentInformation()
        {
            InteropBuffer buffer = InteropBufferPool.AcquireBuffer();

            try
            {
                buffer.Lock();

                int result = CallService(MS_SYSTEM_GETVERSIONTEXT, buffer.SizeAsUIntPtr, buffer.IntPtr);
                Debug.Assert(result == 0);

                if (result == 0 && Translate.ToString(buffer.IntPtr, StringEncoding.Ansi).IndexOf("Unicode") != -1)
                    MirandaEnvironment.MirandaStringEncoding = StringEncoding.Unicode;
                else
                    MirandaEnvironment.MirandaStringEncoding = StringEncoding.Ansi;
            }
            finally
            {
                buffer.Unlock();
                InteropBufferPool.ReleaseBuffer(buffer);
            }

            MirandaEnvironment.MirandaVersion = Translate.FromMirandaVersion((uint)CallService(MirandaServices.MS_SYSTEM_GETVERSION));
        }

        #endregion        

        #region Properties

        internal MirandaPluginLink PluginLink
        {
            get
            {
                return this.pluginLink;
            }
        }

        private MM_INTERFACE mirandaMemoryManager;
        internal MM_INTERFACE MirandaMemoryManager
        {
            get { return mirandaMemoryManager; }
        }

        internal PluginManagerBase PluginManager
        {
            get
            {
                if (this.pluginManager == null)
                    throw new InvalidOperationException("No plugin manager associated with this context.");

                return this.pluginManager;
            }
        }

        public bool HasPluginManager
        {
            get
            {
                return this.pluginManager != null;
            }
        }

        public static MirandaContext Current
        {
            get
            {
                if (singleton == null)
                    throw new InvalidOperationException(TextResources.ExceptionMsg_MirandaContextNotAvailable);

                return singleton;
            }
        }

        public static bool Initialized
        {
            get
            {
                return singleton != null;
            }
        }

        public ServiceCallInterceptionManager ServiceCallInterceptors
        {
            get
            {
                return this.serviceInterceptors;
            }
        }

        public MirandaDatabase MirandaDatabase
        {
            get
            {
                return this.mirandaDatabase;
            }
        }

        public ProtocolDictionary Protocols
        {
            get
            {
                return this.protocols;
            }
        }

        public ContactList ContactList
        {
            get { return contactList; }
        }

        #endregion

        #region Events

        public event EventHandler ModulesLoaded;

        internal void RaiseModulesLoadedEvent()
        {
            if (ModulesLoaded != null)
                ModulesLoaded(this, EventArgs.Empty);
        }

        internal event EventHandler IsolatedModePluginsUnloading;

        internal void RaiseIsolatedModePluginsUnloadingEvent()
        {
            if (IsolatedModePluginsUnloading != null)
                IsolatedModePluginsUnloading(null, EventArgs.Empty);
        }

        #endregion

        #region CallService

        public int CallService(string serviceName)
        {
            return CallService(serviceName, UIntPtr.Zero, IntPtr.Zero, false);
        }

        public int CallService(string serviceName, IntPtr wParam, IntPtr lParam)
        {
            return CallService(serviceName, Translate.ToHandle(wParam), lParam, false);
        }

        [CLSCompliant(false)]
        public int CallService(string serviceName, UIntPtr wParam, IntPtr lParam)
        {
            return CallService(serviceName, wParam, lParam, false);
        }

        internal int CallService(string serviceName, UIntPtr wParam, IntPtr lParam, bool noInterception)
        {
            if (String.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("service");

            int returnCode =
                serviceInterceptors.RequiresInterception(serviceName) && !noInterception ?
                serviceInterceptors[serviceName](wParam, lParam) : pluginLink.NativePluginLink.CallService(serviceName, wParam, lParam);

            return returnCode;
        }

        internal unsafe int CallServiceUnsafe(string serviceName, void* wParam, void* lParam)
        {
            return pluginLink.CallServiceUnsafe(serviceName, wParam, lParam);
        }

        #endregion
    }
}
