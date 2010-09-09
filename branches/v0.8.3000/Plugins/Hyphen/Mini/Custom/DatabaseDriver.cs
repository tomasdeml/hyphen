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
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins;

namespace Virtuoso.Hyphen.Mini.Custom
{
    [LoaderOptions(LoaderOptions.HasCustomApiExports | LoaderOptions.CannotBeUnloaded)]
    public abstract class DatabaseDriver : StandalonePlugin
    {
        #region Fields

        private GCHandle DatabaseLinkGcHandle;
        private UnmanagedStructHandle<DatabaseLink> DatabaseLinkHandle;

        private volatile bool Disposed;

        #endregion

        #region .ctors

        protected DatabaseDriver() { }

        ~DatabaseDriver()
        {
            UnloadPreThunk(0);
        }

        #endregion

        #region Api Handlers

        [CustomApiExportHandler("DatabasePluginInfo")]
        internal object DatabasePluginInfoThunk(object[] args)
        {
            DatabaseLink link = new DatabaseLink();

            link.Size = Marshal.SizeOf(typeof(DatabaseLink));
            link.GetCapability = GetCapabilityThunk;
            link.GetFriendlyName = GetFriendlyNameThunk;
            link.GrokHeader = GrokHeaderThunk;
            link.Init = InitPreThunk;
            link.MakeDatabase = MakeDatabaseThunk;
            link.Unload = UnloadPreThunk;
            DatabaseLinkHandle = new UnmanagedStructHandle<DatabaseLink>(ref link);

            DatabaseLinkGcHandle = GCHandle.Alloc(link);
            return DatabaseLinkHandle.IntPtr;
        }

        #endregion

        #region Thunks

        /// <summary>
        /// MirandaContext is not available at the time of the invocation.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected abstract int MakeDatabaseThunk(string profile, ref int error);

        protected abstract int GrokHeaderThunk(string profile, ref int error);

        protected abstract int GetCapabilityThunk(int flags);

        protected abstract int GetFriendlyNameThunk(IntPtr buffer, int size, int shortName);

        private int InitPreThunk(string profile, IntPtr pLink)
        {
            return -(Math.Abs(Module.Load(pLink)) + Math.Abs(InitThunk(profile, pLink)));
        }

        protected abstract int InitThunk(string profile, IntPtr link);

        private int UnloadPreThunk(int wasLoaded)
        {
            try
            {
                if (!Disposed)
                {
                    UnloadThunk(wasLoaded);
                    DatabaseLinkHandle.Free();

                    if (DatabaseLinkGcHandle.IsAllocated)
                        DatabaseLinkGcHandle.Free();                    
                }
            }
            finally
            {
                Disposed = true;
            }

            return 0;
        }

        protected abstract int UnloadThunk(int wasLoaded);

        #endregion        
    }
}
