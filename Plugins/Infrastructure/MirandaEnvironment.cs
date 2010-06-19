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
using System.IO;
using Virtuoso.Hyphen;
using System.Windows.Forms;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public static class MirandaEnvironment
    {
        #region Fields

        private static Version mirandaVersion;
        private static StringEncoding mirandaStringEncoding;

        internal const string MirandaPluginsFolderRelativePath = @"plugins\",
                              ManagedPluginsFolderName = "managed",
                              ManagedPluginsFolderRelativePath = MirandaPluginsFolderRelativePath + ManagedPluginsFolderName;

        private static readonly string mirandaFolderPath = Application.StartupPath,
                       mirandaPluginsFolderPath = Path.Combine(mirandaFolderPath, MirandaPluginsFolderRelativePath),
                       managedPluginsFolderPath = Path.Combine(mirandaFolderPath, ManagedPluginsFolderRelativePath),
                       mirandaBootIniPath = mirandaFolderPath + @"\MirandaBoot.ini";        

        #endregion

        #region Methods

        public static string GetManagedSubdirectoryRelativePath(string subDir)
        {
            return Path.Combine(ManagedPluginsFolderRelativePath, subDir);
        }

        #endregion

        #region Properties

        public static StringEncoding MirandaStringEncoding
        {
            get
            {
                return mirandaStringEncoding;
            }
            internal set
            {
                mirandaStringEncoding = value;
            }
        }

        public static string MirandaFolderPath
        {
            get         
            {
                return mirandaFolderPath;
            }
        }

        public static string MirandaPluginsFolderPath
        {
            get
            {
                return mirandaPluginsFolderPath;
            }
        }

        public static string ManagedPluginsFolderPath
        {
            get
            {
                return managedPluginsFolderPath;
            }
        }

        public static Version MirandaVersion
        {
            get
            {
                return mirandaVersion;
            }
            internal set
            {
                mirandaVersion = value;
            }
        }

        public static Version HyphenVersion
        {
            get
            {
                return Loader.HyphenVersion;
            }
        }

        public static string MirandaBootIniPath
        {
            get
            {
                return mirandaBootIniPath;
            }
        }

        #endregion        
    }
}
