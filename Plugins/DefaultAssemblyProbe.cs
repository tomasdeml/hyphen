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
using System.IO;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Hyphen;

namespace Virtuoso.Miranda.Plugins
{
    internal sealed class DefaultAssemblyProbe : AssemblyProbe
    {
        #region .ctors

        public DefaultAssemblyProbe() { }

        #endregion

        #region Impls

        public override string[] FindAssemblies()
        {
            try
            {
                return Directory.Exists(MirandaEnvironment.ManagedPluginsFolderRelativePath) ?
                            Directory.GetFiles(MirandaEnvironment.ManagedPluginsFolderRelativePath, "*.dll") : new string[0];
            }
            catch (Exception e)
            {
                Log.Write(3, Loader.LogCategory, "Unable to find plugins: " + e.Message);
                return new string[0];
            }
        }

        #endregion
    }
}
