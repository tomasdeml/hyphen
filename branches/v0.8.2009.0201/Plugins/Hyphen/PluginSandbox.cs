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
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Virtuoso.Miranda.Plugins.Helpers;
using Virtuoso.Hyphen.Configuration;

namespace Virtuoso.Hyphen
{
    internal sealed class PluginsSandbox : Sandbox
    {
        #region Fields

        private AssemblyProbe assemblyProbe;
        public AssemblyProbe AssemblyProbe
        {
            get { return assemblyProbe; }
        }

        private PluginManagerBase pluginManager;
        public PluginManagerBase PluginManager
        {
            get { return pluginManager; }
        }

        #endregion

        #region .ctors

        public PluginsSandbox()
        {
            SetUpHostingAppDomain("HyphenPlugins");
        }

        protected override void InitializeAppDomainSetup(AppDomainSetup domainSetup)
        {
            base.InitializeAppDomainSetup(domainSetup);

            domainSetup.ShadowCopyFiles = true.ToString();
            domainSetup.ShadowCopyDirectories = MirandaEnvironment.ManagedPluginsFolderPath;
        }

        public void LoadAssemblyProbe()
        {
            AssemblyProbe customProbe = null;

            try
            {
                string fusionAssemblyName = RuntimeConfiguration.Singleton.CustomFusionAssemblyName;
                string assemblyProbeTypeName = RuntimeConfiguration.Singleton.CustomAssemblyProbeTypeName;

                if (!String.IsNullOrEmpty(fusionAssemblyName) && !String.IsNullOrEmpty(assemblyProbeTypeName))
                    customProbe = InstantiateRemoteObject<AssemblyProbe>(fusionAssemblyName, assemblyProbeTypeName, null);
            }
            catch (Exception e)
            {
                Log.Write(0, Loader.LogCategory, "Unable to load custom assembly probe (" + e.Message + "), defaulting to the built-in one.");
            }

            assemblyProbe = customProbe ?? InstantiateRemoteObject<AssemblyProbe>(GetType().Assembly.FullName, typeof(DefaultAssemblyProbe).FullName);
        }

        public void LoadPluginManager(FusionContext context)
        {
            PluginManagerBase customManager = null;

            try
            {
                string fusionAssemblyName = RuntimeConfiguration.Singleton.CustomFusionAssemblyName;
                string pluginManagerTypeName = RuntimeConfiguration.Singleton.CustomPluginManagerTypeName;

                if (!String.IsNullOrEmpty(fusionAssemblyName) && !String.IsNullOrEmpty(pluginManagerTypeName))
                    customManager = InstantiateRemoteObject<PluginManagerBase>(fusionAssemblyName, pluginManagerTypeName, context);
            }
            catch (Exception e)
            {
                Log.Write(0, Loader.LogCategory, "Unable to load custom plugin manager (" + e.Message + "), defaulting to the built-in one.");
            }

            pluginManager = customManager ?? InstantiateRemoteObject<PluginManagerBase>(GetType().Assembly.FullName, typeof(DefaultPluginManager).FullName, context);
        }

        #endregion

        #region Properties

        public string HostingAppDomainPrivateBinPath
        {
            get
            {
                return HostingAppDomain.SetupInformation.PrivateBinPath;
            }
        }

        #endregion
    }
}
