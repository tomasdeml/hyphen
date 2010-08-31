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
using System.IO;
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Hyphen
{
    [CLSCompliant(false)]
    public sealed class FusionContext : RemoteObject
    {
        #region Fields

        private readonly AssemblyProbe assemblyProbe;
        private readonly IntPtr nativePluginLink;
        private readonly Loader loader;
        
        private static FusionContext emptySingleton;

        #endregion

        #region .ctors

        // Empty .ctor
        private FusionContext() { }

        internal FusionContext(Loader loader, AssemblyProbe pluginProbe, IntPtr nativePluginLink)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");

            if (pluginProbe == null)
                throw new ArgumentNullException("pluginProbe");

            if (nativePluginLink == IntPtr.Zero) 
                throw new ArgumentNullException("nativePluginLink");

            this.loader = loader;
            this.assemblyProbe = pluginProbe;
            this.nativePluginLink = nativePluginLink;
        }

        #endregion

        #region Properties

        private void CheckEmpty()
        {
            // TODO: Localize
            if (IsInvalid) throw new InvalidOperationException("This context is empty.");
        }

        public bool IsInvalid
        {
            get
            {
                return this.nativePluginLink == IntPtr.Zero;
            }
        }

        internal static FusionContext Empty
        {
            get
            {
                return emptySingleton ?? (emptySingleton = new FusionContext());
            }
        }

        public AssemblyProbe AssemblyProbe
        {
            get { CheckEmpty(); return assemblyProbe; }
        } 
        
        internal IntPtr NativePluginLink
        {
            get { CheckEmpty(); return nativePluginLink; }
        }

        internal Loader Loader
        {
            get
            {
                return loader;
            }
        }

        #endregion
    }
}
