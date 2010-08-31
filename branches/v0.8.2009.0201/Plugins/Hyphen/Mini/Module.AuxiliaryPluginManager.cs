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
using System.Runtime.CompilerServices;
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Hyphen.Mini
{
    partial class Module
    {
        /// <summary>
        /// Auxiliary Plugin Manager used to load a standalone plugin into the default AppDomain.
        /// </summary>
        private sealed class AuxiliaryPluginManager : PluginManagerBase
        {
            #region Fields

            private static AuxiliaryPluginManager Singleton;

            #endregion

            #region .ctors

            private AuxiliaryPluginManager()
                : base(FusionContext.Empty, false, false) { }

            #endregion

            #region Impl

            protected internal override void FindAndLoadPlugins()
            {
                throw new NotSupportedException();
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public static AuxiliaryPluginManager GetInstance()
            {
                return Singleton ?? (Singleton = new AuxiliaryPluginManager());
            }

            /// <summary>
            /// Populates context information to be available for standalone plugins residing in the default AppDomain.
            /// </summary>
            /// <remarks>The information are not published when there are no standalone modules to conserve resources.</remarks>
            public void FinishInitialization()
            {
                MirandaContext.Current.PopulateContextInformation();
            }

            #endregion
        }
    }
}
