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
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Hyphen.Configuration
{
    /* NOTE: WHEN ACCESSING THE CONFIG FROM THE OUTSIDE OF THE LOADER, REMEMBER THAT ITS SINGLETON
     * RESIDES IN THE DEFAULT APPDOMAIN, THUS IT IS NOT INITIALIZED IN OTHER APPDOMAINS */
    [Serializable, ConfigurationOptions("0.0.0.6", Encrypt = false, ProfileBound = false)]
    internal sealed class RuntimeConfiguration : PluginConfiguration
    {
        #region Fields

        private static RuntimeConfiguration singleton;

        private string customFusionAssemblyName, customAssemblyProbeTypeName, customPluginManagerTypeName;
        private bool loadPluginsOnStartup, lazyUnload;

        #endregion

        #region .ctors

        private RuntimeConfiguration() { }

        public static RuntimeConfiguration Singleton
        {
            get
            {
                if (singleton == null)
                    throw new InvalidOperationException("Configuration not initialized.");

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

        protected override void InitializeDefaultConfiguration()
        {
            lazyUnload = true;
        }

        #endregion

        #region Properties

        public string CustomPluginManagerTypeName
        {
            get { return customPluginManagerTypeName; }
            set { customPluginManagerTypeName = value; }
        }

        public string CustomAssemblyProbeTypeName
        {
            get { return customAssemblyProbeTypeName; }
            set { customAssemblyProbeTypeName = value; }
        }

        public string CustomFusionAssemblyName
        {
            get { return customFusionAssemblyName; }
            set { customFusionAssemblyName = value; }
        }

        public bool LoadPluginsOnStartup
        {
            get { return loadPluginsOnStartup; }
            set { loadPluginsOnStartup = value; }
        }

        public bool UseLazyUnload
        {
            get { return lazyUnload; }
            set { lazyUnload = value; }
        }

        #endregion

        #region Methods

        public static void Initialize()
        {
            singleton = RuntimeConfiguration.Load<RuntimeConfiguration>();
        }

        public static void Reset()
        {
            singleton = PluginConfiguration.GetDefaultConfiguration<RuntimeConfiguration>();
        }

        public static void Reload()
        {
            Initialize();
        }

        #endregion
    }
}
