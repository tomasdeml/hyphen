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
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins.Resources;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;
using Virtuoso.Hyphen.Native;
using Virtuoso.Miranda.Plugins;
using System.Runtime.CompilerServices;
using Virtuoso.Hyphen.Mini.Custom;
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Forms;

namespace Virtuoso.Hyphen.Mini
{
    /// <summary>
    /// Represents a standalone module loaded into the default AppDomain.
    /// </summary>
    public sealed partial class Module
    {
        #region Fields

        private const string LogCategory = "HyphenMini";
        private const string MasterSuffx = ".master.dll";

        private Assembly MasterAssembly;
        private StandalonePlugin standalonePlugin;
        private PluginDescriptor PluginDescriptor;
        private bool isPostV07Build20Api;

        private IntPtr PluginInfoPtr;
        private IntPtr MirandaPluginInterfacesPtr;

        private readonly Assembly MiniAssembly;
        private volatile bool Loaded, Unloaded;
        private LoaderOptions LoaderOptions;

        private AuxiliaryPluginManager PluginManager;
        private Loader HyphenLoader;

        private readonly string MasterDirectory, MasterAssemblyPath;

        #endregion

        #region .ctors & .dctors

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class.
        /// </summary>
        /// <param name="exApi">TRUE if the modules runs under a post-0.7#20 API; FALSE otherwise.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal Module(bool exApi)
        {
            // Init runtime
            Loader.Initialize();

            // Get a stub assembly establishing the connection
            MiniAssembly = Assembly.GetCallingAssembly();

            // Get the Loader instance based on stub's version
            Version supportedVersion = MiniAssembly.GetName().Version;
            HyphenLoader = Loader.GetInstance(new Version(supportedVersion.ToString(3)));

            if (HyphenLoader == null)
            {
                string message = String.Format("Hyphen.Mini module requested a Loader of version {0}, but that one is not available. Upgrade Hyphen.", supportedVersion);
                Log.DebuggerWrite(5, LogCategory, message);

                throw new RuntimeNotSupportedException(null, supportedVersion);
            }

            isPostV07Build20Api = exApi;
            MasterDirectory = Path.GetDirectoryName(MiniAssembly.Location);
            MasterAssemblyPath = Path.GetFileName(MiniAssembly.Location);

            Log.DebuggerWrite(0, LogCategory, "Connection between Miranda and '" + MasterAssemblyPath + "' established.");
        }

        /// <summary>
        /// Finalizes the module.
        /// </summary>
        ~Module()
        {
            Unloaded = true;
        }

        #endregion

        #region API IMPL

        /// <summary>
        /// Represents the MirandaPluginInfo export of Miranda's API.
        /// </summary>
        /// <param name="version">Miranda version (in Miranda's format)</param>
        /// <returns>Ptr to an instance of the PLUGININFO(EX) structure.</returns>
        internal IntPtr MirandaPluginInfo(uint version)
        {
            // Already called?
            if (PluginInfoPtr != IntPtr.Zero)
                return PluginInfoPtr;

            Log.DebuggerWrite(0, LogCategory, "MirandaPluginInfo export invoked for " + MasterAssemblyPath);
            StandalonePlugin plugin = null;

            try
            {
                // Execute common init tasks
                HyphenLoader.MirandaPluginInfoShared(version);

                // Load the plugin
                plugin = LoadActualPlugin();

                // OK
                if (plugin != null)
                {
                    plugin.Module = this;
                    plugin.AfterModuleInitializationInternal();

                    // OK
                    if (PublishPluginInformation(plugin, version))
                    {
                        standalonePlugin = plugin;
                        MasterAssembly = plugin.GetType().Assembly;

                        ProbeCustomApiExports(plugin);
                        return PluginInfoPtr;
                    }
                }
                else
                    Log.DebuggerWrite(0, LogCategory, "No master assembly found for '" + MasterAssemblyPath + "' - aborting initialization");
            }
            catch (Exception e)
            {
                DefaultExceptionHandler.Create(plugin).HandleException(e, null);
                Log.DebuggerWrite(5, LogCategory, "An error occurred while executing the MirandaPluginInfo export\n" + e.ToString());
            }

            // Return dummy instance, not null (crashes Miranda 0.8.0.1)
            return PluginInfoPtr = Loader.GetDummyPluginInfo();
        }

        /// <summary>
        /// Represents the MirandaPluginInterfaces export of Miranda's API.
        /// </summary>
        /// <returns>Ptr to an array of interface GUIDs.</returns>
        internal IntPtr MirandaPluginInterfaces()
        {
            if (MirandaPluginInterfacesPtr != IntPtr.Zero)
                return MirandaPluginInterfacesPtr;

            try
            {
                return (MirandaPluginInterfacesPtr = standalonePlugin.MirandaPluginInterfaces());
            }
            catch (Exception e)
            {
                DefaultExceptionHandler.Create(StandalonePlugin).HandleException(e, StandalonePlugin.Descriptor);
                Log.DebuggerWrite(5, LogCategory, "An error occured while executing the MirandaPluginInterfaces export\n" + e.ToString());
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Represents the Load export of Miranda API. Loads Hyphen and initializes the module.
        /// </summary>
        /// <param name="pPluginLink">Ptr to an instance of the PLUGINLINK structure.</param>
        /// <returns>Load result.</returns>
        internal int Load(IntPtr pPluginLink)
        {
            try
            {
                if (Loaded)
                    throw new InvalidOperationException(TextResources.ExceptionMsg_PluginAlreadyInitialized);

                Log.DebuggerWrite(0, LogCategory, "Load export invoked for " + MasterAssemblyPath);

                // Initialize the runtime (if necessary)
                HyphenLoader.ModuleInducedLoad(pPluginLink);
                ModuleManager.Singleton.RegisterModule(this);

                // Get the auxiliary plugin manager to load a standalone plugin into the default AppDomain
                PluginManager = AuxiliaryPluginManager.GetInstance();
                PluginDescriptor = PluginManager.LoadPlugin(standalonePlugin, false);

                // Call plugin's Load export
                standalonePlugin.LoadInternal(pPluginLink);

                // When Miranda completes initialization...
                MirandaContext.Current.ModulesLoaded += ModulesLoadedHandler;

                Log.DebuggerWrite(0, LogCategory, "Finishing " + MasterAssemblyPath + " initialization");
                return (int)CallbackResult.Success;
            }
            catch (Exception e)
            {
                DefaultExceptionHandler.Create(StandalonePlugin).HandleException(e, StandalonePlugin.Descriptor);
                Log.DebuggerWrite(5, LogCategory, "An error occurred while executing the Load export\n" + e.ToString());

                return (int)CallbackResult.Failure;
            }
            finally
            {
                Loaded = true;
            }
        }

        /// <summary>
        /// Enables the plugin when Miranda completes initialization.
        /// </summary>
        private void ModulesLoadedHandler(object sender, EventArgs e)
        {
            try
            {
                // DO NOT TOUCH
                PluginManager.FinishInitialization();

                /* Inject the managed menu into our (default) AppDomain
                 * (the menu is used for additional items ONLY) */
                HyphenLoader.PromoteManagedMenuIntoAppDomain(PluginManager);

                // Enable the plugin
                if (PluginLoaded && PluginInitialized)
                    PluginManager.SetPluginState(PluginDescriptor, PluginState.Enabled);
            }
            catch (Exception ex)
            {
                DefaultExceptionHandler.Create(PluginDescriptor.Plugin).HandleException(ex, PluginDescriptor);
            }
        }

        /// <summary>
        /// Represents the Unload export of Miranda API. Unloads Hyphen and shuts down the runtime.
        /// </summary>
        /// <returns>Unload result.</returns>
        internal int Unload()
        {
            try
            {
                if (!CanUnload())
                    return (int)CallbackResult.Success;

                // Call the Unload export
                standalonePlugin.UnloadInternal();

                PluginManager.SetPluginState(PluginDescriptor, PluginState.Disabled);
                ModuleManager.Singleton.UnregisterModule(this);

                if (!standalonePlugin.HasCustomPluginInfo)
                    Marshal.FreeHGlobal(PluginInfoPtr);

                if (!standalonePlugin.HasCustomPluginInterfaces)
                    Marshal.FreeHGlobal(MirandaPluginInterfacesPtr);

                PluginDescriptor = null;
                standalonePlugin = null;

                Log.DebuggerWrite(0, LogCategory, "Connection between Miranda and '" + MasterAssemblyPath + "' broken.");
            }
            catch (Exception e)
            {
                DefaultExceptionHandler.Create(StandalonePlugin).HandleException(e, StandalonePlugin.Descriptor);
                Log.DebuggerWrite(5, LogCategory, "An error occurred while executing the Unload export\n" + e.ToString());

                return (int)CallbackResult.Failure;
            }
            finally
            {
                Unloaded = true;
            }

            return (int)CallbackResult.Success;
        }

        /// <summary>
        /// Gets a indication whether a plugin can be unloaded.
        /// </summary>
        /// <returns></returns>
        private bool CanUnload()
        {
            return !Unloaded && PluginInitialized &&
                (LoaderOptions & LoaderOptions.CannotBeUnloaded) != LoaderOptions.CannotBeUnloaded;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gathers and publishes plugin information.
        /// </summary>
        /// <param name="plugin">Plugin to evaluate.</param>
        /// <param name="version">Miranda version to pass to the plugin.</param>
        /// <returns>TRUE if the information were obtained; FALSE if not.</returns>
        private bool PublishPluginInformation(StandalonePlugin plugin, uint version)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            try
            {
                LoaderOptionsAttribute loaderOptions = LoaderOptionsAttribute.Get(plugin.GetType(), LoaderOptionsOwner.Type);

                if (!loaderOptions.SupportsMirandaVersion(version))
                    return false;

                LoaderOptions = loaderOptions.Options;
                PluginInfoPtr = plugin.MirandaPluginInfo(version, isPostV07Build20Api);

                return true;
            }
            catch (Exception)
            {
                PluginInfoPtr = IntPtr.Zero;
                return false;
            }
        }

        /// <summary>
        /// Probes custom API exports of a standalone plugin.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        private void ProbeCustomApiExports(StandalonePlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            if ((LoaderOptions & LoaderOptions.HasCustomApiExports) != LoaderOptions.HasCustomApiExports)
                return;
            try
            {
                Type customApiHandlerAttribType = typeof(CustomApiExportHandlerAttribute),
                     customApiHandlerCallbackType = typeof(CustomApiExportCallback);

                foreach (MethodInfo method in plugin.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy))
                {
                    if (!method.IsDefined(customApiHandlerAttribType, true))
                        continue;

                    Delegate callbackDeleg = Delegate.CreateDelegate(customApiHandlerCallbackType, plugin, method, false);

                    if (callbackDeleg == null)
                        continue;

                    CustomApiExportHandlerAttribute attrib = (CustomApiExportHandlerAttribute)method.GetCustomAttributes(customApiHandlerAttribType, true)[0];
                    plugin.CustomApiHandlers.Add(new CustomApiExportDescriptor(attrib.ExportName, (CustomApiExportCallback)callbackDeleg));

                }
            }
            catch (Exception e)
            {
                Log.DebuggerWrite(5, LogCategory, "Unable to probe custom plugin api exports. " + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Loads the actual plugin behind a proxy assembly.
        /// </summary>
        /// <returns>An instance of a plugin.</returns>
        private StandalonePlugin LoadActualPlugin()
        {
            try
            {
                string assemblyPath = Path.Combine(MasterDirectory, Path.GetFileNameWithoutExtension(MasterAssemblyPath) + MasterSuffx);

                if (!File.Exists(assemblyPath))
                    return null;

                Assembly masterAssembly = Assembly.LoadFile(assemblyPath);
                Type[] exposedTypes = PluginManagerBase.GetExposedPlugins(masterAssembly);

                // Find a first standalone plugin
                Type masterType = Array.Find<Type>(exposedTypes, delegate(Type _type)
                {
                    return _type.IsSubclassOf(typeof(StandalonePlugin));
                });

                // None found
                if (masterType == null)
                    throw new TypeLoadException(String.Format(TextResources.ExceptionMsg_Formatable1_UnableToLoadMasterType, MasterAssemblyPath));

                try
                {
                    return (StandalonePlugin)PluginManagerBase.InstantiatePlugin(masterType, true);
                }
                catch (RuntimeNotSupportedException rvnsE)
                {
                    MessageBox.Show(String.Format(TextResources.ExceptionMsg_Formatable2_RuntimeVersionNotAvailable, masterType.FullName, rvnsE.RequiredVersion), TextResources.MsgBox_Caption_RuntimeVersionNotAvailable, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
            catch (Exception e)
            {
                Log.DebuggerWrite(5, LogCategory, "Unable to instantiate the master plugin. " + e.Message);
                throw;
            }
        }

        #endregion

        #region Equals etc.

        public override int GetHashCode()
        {
            return MiniAssembly.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Module other = obj as Module;

            if (other == null)
                return false;

            return other.GetHashCode() == GetHashCode();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a standalone plugin behind this module.
        /// </summary>
        public StandalonePlugin StandalonePlugin
        {
            get
            {
                return standalonePlugin;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the runtime runs under the post-0.7#20 Miranda API.
        /// </summary>
        public bool IsPostV07Build20Api
        {
            get
            {
                return isPostV07Build20Api;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the standalone plugin was loaded.
        /// </summary>
        private bool PluginLoaded
        {
            get
            {
                return standalonePlugin != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the standalone plugins was initialized (i.e. has a descriptor).
        /// </summary>
        private bool PluginInitialized
        {
            get
            {
                return PluginDescriptor != null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a custom API export.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="exportName">Export name.</param>
        /// <param name="data">Additional data.</param>
        /// <returns>Return value.</returns>
        internal T ExecuteCustomApiExport<T>(string exportName, params object[] data)
        {
            if (String.IsNullOrEmpty(exportName))
                throw new ArgumentNullException("exportName");

            if (data == null)
                throw new ArgumentNullException("data");

            if (!PluginLoaded)
                throw new InvalidOperationException(TextResources.ExceptionMsg_PluginNotInitialized);

            // Find the export
            CustomApiExportDescriptor descriptor = standalonePlugin.CustomApiHandlers.Find(delegate(CustomApiExportDescriptor _handler)
            {
                return _handler.ExportName == exportName;
            });

            if (descriptor == null)
                throw new NotImplementedException(exportName);
            else
                return descriptor.Execute<T>(data);
        }

        #endregion
    }
}
