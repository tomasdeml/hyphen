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
using System.IO;
using System.Reflection;
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins
{
    internal sealed class DefaultPluginManager : PluginManagerBase
    {
        #region Fields & Constants

        public new const string LogCategory = Loader.LogCategory + "::PluginManager";
        public const string InternalServiceNamePrefix = "Virtuoso.Miranda.Plugins.Services.";

        #endregion

        #region .ctors

        public DefaultPluginManager(FusionContext fusionContext)
            : base(fusionContext, true, true)
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Log.DebuggerWrite(0, LogCategory, "Hyphen Plugin Manager v" + assemblyName.Version.ToString() + " is initializing, please wait...");            

            FirePrimaryPluginManagerInitializedEvent(this, EventArgs.Empty);
            Log.DebuggerWrite(0, LogCategory, "Default Plugin Manager initialized.");
        }

        #endregion

        #region Fusion

        protected internal override void FindAndLoadPlugins()
        {
            if (FusionContext.IsInvalid)
                throw new InvalidOperationException(TextResources.ExceptionMsg_InvalidFusionContext);

            if (Initialized)
                throw new InvalidOperationException(TextResources.ExceptionMsg_PluginManagerAlreadyInitialized);

            Assembly currentAssembly = null;
            Type currentType = null;
            MirandaPlugin currentPlugin = null;

            try
            {
                string[] paths = FusionContext.AssemblyProbe.FindAssemblies();

                if (paths.Length == 0)
                {
                    Log.DebuggerWrite(1, LogCategory, "No managed plugins found.");
                    return;
                }

                Log.DebuggerWrite(0, LogCategory, "Found " + paths.Length + " managed plugins...");

                foreach (string path in paths)
                    LoadAssembly(path, ref currentAssembly, ref currentType, ref currentPlugin);
            }
            catch (Exception e)
            {
                FusionException fEx = null;
                Log.DebuggerWrite(0, LogCategory, e.ToString());

                if (e is IOException)
                    fEx = new FusionException(TextResources.ExceptionMsg_IOErrorOccurred, currentAssembly, null, null, e);
                if (e is FusionException)
                    fEx = (FusionException)e;
                else
                    fEx = new FusionException(e.Message, currentAssembly, currentType, null, e);

                HandleException(fEx, currentPlugin != null ? currentPlugin.Descriptor : (PluginDescriptor)null);
            }
            finally
            {
                DeclareInitialized();
                RaiseFusionCompletedEvent(EventArgs.Empty);

                Infrastructure.PluginConfiguration.FlushCaches();
                Log.DebuggerWrite(0, LogCategory, "Fusion completed.");
            }
        }

        private void LoadAssembly(string path, ref Assembly currentAssembly, ref Type currentType, ref MirandaPlugin currentPlugin)
        {
            Log.DebuggerWrite(0, LogCategory, "Loading assembly '" + path + "'...");

            try
            {
                currentAssembly = Assembly.Load(Path.GetFileNameWithoutExtension(path));

                foreach (Type type in GetExposedPlugins(currentAssembly))
                    LoadPluginFromType(currentType = type);
            }
            catch (BadImageFormatException bifE)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_UnmanagedImageFound, path), bifE.FusionLog, null, null, null, bifE);
            }
            catch (FileNotFoundException fnfE)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_AssemblyLoadError, currentAssembly != null ? currentAssembly.ToString() : path), fnfE.FusionLog, currentAssembly, currentType, null, fnfE);
            }
            catch (FusionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_AssemblyLoadError, path, e.Message), currentAssembly.ToString(), null, null, null, e);
            }
        }

        private void LoadPluginFromType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Log.DebuggerWrite(0, LogCategory, "Loading plugin '" + type.Name + "'...");

            try
            {
                MirandaPlugin plugin = InstantiatePlugin(type, false);

                if (plugin == null)
                    return;

                PluginDescriptor pluginDescriptor = LoadPlugin(plugin);

                // If not disabled, enable the plugin
                if (IsEnabled(plugin))
                    SetPluginState(pluginDescriptor, PluginState.Enabled);

                Log.DebuggerWrite(0, LogCategory, "Plugin successfully loaded.");
            }
            catch (MissingMethodException mmEx)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_NoValidPluginCtorFound, type.FullName), type.Assembly, type, null, mmEx);
            }
            catch (TargetInvocationException tiEx)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable2_ErrorWhileInstantiatingPlugin,
                    type.FullName, tiEx.InnerException != null ? tiEx.InnerException.Message : TextResources.UI_Label_Unknown),
                    type.Assembly, type, null, tiEx.InnerException);
            }
            catch (MethodAccessException maEx)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_UnauthorizedToInstantiatePlugin, type.FullName), type.Assembly, type, null, maEx);
            }
        }

        #endregion
    }
}
