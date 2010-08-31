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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Virtuoso.Hyphen.Configuration;
using Virtuoso.Hyphen.Mini;
using Virtuoso.Hyphen.Native;
using Virtuoso.Miranda.Plugins;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Configuration;
using Virtuoso.Miranda.Plugins.Configuration.Forms;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Miranda.Plugins.Forms.Controls;
using Virtuoso.Miranda.Plugins.Helpers;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Miranda.Plugins.ThirdParty.Updater;

namespace Virtuoso.Hyphen
{
    /// <summary>
    /// Controls managed plugin fusion and orchestrates the runtime.
    /// </summary>
    internal sealed class Loader : RemoteObject
    {
        #region Fields

        #region Constants

        public const string LogCategory = "Hyphen";

        private static readonly Uri HyphenUpdateUrl = new Uri("http://virtuosity.aspweb.cz/files/miranda/development/hyphen/updates/hyphen_update.zip"),
            HyphenVersionUrl = new Uri("http://virtuosity.aspweb.cz/files/miranda/development/hyphen/updates/hyphen_update_version.txt"),
            HyphenHomepageUrl = new Uri("http://virtuosity.aspweb.cz");

        private static readonly Version MinMirandaVersion = new Version(0, 7, 0, 0);

        #endregion

        #region Services

        private const string ServicePrefix = "Virtuoso.Hyphen.Loader.Services";

        private const string LoadUnloadPluginsServiceName = ServicePrefix + "LoadUnloadPlugins";
        private const string ConfigureModulesServiceName = ServicePrefix + "ConfigureModules";
        private const string ShowManagedMenuServiceName = ServicePrefix + "ShowManagedMenu";
        private const string ManagePluginsServiceName = ServicePrefix + "ManagePlugins";

        #endregion

        #region Common

        private static Loader Singleton;
        private static readonly object SyncObject = new object();

        private volatile bool Unloaded;

        private PLUGININFO pluginInfo;
        private static IntPtr DummyPluginInfo;

        private UnmanagedStructHandle<PLUGININFO> PluginInfoHandle;
        private UnmanagedStructHandle<PLUGININFOEX> PluginInfoExHandle;
        private MirandaPluginLink PluginLink;

        private FusionContext FusionContext;
        private PluginsSandbox IsolatedPluginsSandbox;

        private MenuItemDeclarationAttribute PluginTasksItem;
        private ManagedMainMenu ManagedMainMenu;

        private readonly HookDescriptorCollection InternalHooks = new HookDescriptorCollection();

        private FileSystemWatcher PluginsFolderWatcher;
        private readonly ManualResetEvent PluginsLoadedEvent = new ManualResetEvent(false);

        private Mutex SingleInstanceMutex;
        private SynchronizationContext UIThreadSyncContext;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets a current version of the runtime.
        /// </summary>
        public static Version HyphenVersion
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Gets a value whether the plugins are loaded. Not synchronized, always use in a lock.
        /// </summary>
        /// <remarks>THIS MUST NOT BE SYNCHRONIZED, ALWAYS CALLED IN LOCK! (possible deadlock in FusionProgressDialog).</remarks>
        public bool PluginsLoaded
        {
            get
            {
                return IsolatedPluginsSandbox != null;
            }
        }

        /// <summary>
        /// Gets Hyphen plugin info.
        /// </summary>
        public PLUGININFO PluginInfo
        {
            get
            {
                return this.pluginInfo;
            }
        }

        #endregion

        #region API IMPL

        #region First-Init

        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <remarks>
        /// Called from the exported IL stubs as a first method to initialize the Loader singleton. 
        /// </remarks>
        public static void Initialize()
        {
            lock (SyncObject)
            {
                if (Singleton == null)
                    Singleton = new Loader();
            }
        }

        /// <summary>
        /// Initializes the Loader and prepares its PLUGININFO.
        /// </summary>
        private Loader()
        {
            AppDomain.CurrentDomain.UnhandledException += TrapUnhandledException;
            Log.Write(0, LogCategory, "Initializing Hyphen...");

            try
            {
                InitializePluginInfo();
                Virtuoso.Miranda.Plugins.Infrastructure.RuntimeEnvironment.Initialize();

                Log.Write(0, LogCategory, "Hyphen successfully initialized.");
            }
            catch (Exception e)
            {
                Log.Write(5, LogCategory, "Failed constructing the PLUGININFO." + Environment.NewLine + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Publishes the plugin info and marshals it into a ptr.
        /// </summary>
        private void InitializePluginInfo()
        {
            PLUGININFO pluginInfo = new PLUGININFO();
            PopulatePluginInfo(pluginInfo);
            PluginInfoHandle = new UnmanagedStructHandle<PLUGININFO>(ref pluginInfo);

            PLUGININFOEX pluginInfoEx = new PLUGININFOEX(UUID.HyphenUUID);
            PopulatePluginInfo(pluginInfoEx);
            PluginInfoExHandle = new UnmanagedStructHandle<PLUGININFOEX>(ref pluginInfoEx);

            this.pluginInfo = pluginInfoEx;
        }

        /// <summary>
        /// Populates the plugin info with Hyphen's identity.
        /// </summary>
        /// <param name="pluginInfo">Plugin info.</param>
        private static void PopulatePluginInfo(PLUGININFO pluginInfo)
        {
            if (pluginInfo == null)
                throw new ArgumentNullException("pluginInfo");

            pluginInfo.Size = Marshal.SizeOf(pluginInfo.GetType());
            pluginInfo.Author = "virtuoso";
            pluginInfo.AuthorEmail = "deml.tomas@seznam.cz";
            pluginInfo.Copyright = "© 2006-2008, virtuoso";
            pluginInfo.Description = "Microsoft.net runtime for managed plugins.";
            pluginInfo.HomePage = HyphenHomepageUrl.ToString();
            pluginInfo.Flags = (byte)PluginFlags.UnicodeAware;
            pluginInfo.ReplacesDefaultModule = 0;
            pluginInfo.ShortName = "Hyphen";
            pluginInfo.Version = Translate.ToMirandaVersion(HyphenVersion);
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets a specified version of the Loader.
        /// </summary>
        /// <param name="requiredVersion">A version of the Loader to be returned.</param>
        /// <returns>An instance of the Loader or NULL when the version cannot be satisfied.</returns>
        /// <remarks>
        /// Cannot be inlined because of the HyphenVersion property which relies on the Assembly.GetExecutingAssembly() method.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Loader GetInstance(Version requiredVersion)
        {
            if (requiredVersion > HyphenVersion)
                return null;
            else
                return GetInstance();
        }

        /// <summary>
        /// Gets an instance of the Loader.
        /// </summary>
        /// <returns>An instance of the Loader.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Initialize()"/> method not called.</exception>
        public static Loader GetInstance()
        {
            lock (SyncObject)
            {
                if (Singleton == null)
                    throw new InvalidOperationException();

                return Singleton;
            }
        }

        #endregion

        #region APIs

        #region MirandaPluginInfo

        /// <summary>
        /// Represents the MirandaPluginInfo export of Miranda's API.
        /// </summary>
        /// <param name="version">Miranda version (in Miranda's format)</param>
        /// <returns>Ptr to an instance of the PLUGININFO structure.</returns>
        public IntPtr MirandaPluginInfo(uint version)
        {
            MirandaPluginInfoShared(version);

            if (!SupportsMirandaVersion(version))
                return GetDummyPluginInfo();
            else
            {
                lock (SyncObject)
                    return PluginInfoHandle.IntPtr;
            }
        }

        /// <summary>
        /// Represents the MirandaPluginInfoEx export of Miranda's API.
        /// </summary>
        /// <param name="version">Miranda version (in Miranda's format)</param>
        /// <returns>Ptr to an instance of the PLUGININFOEX structure.</returns>
        /// <remarks>
        /// Specific to post-0.7#20 Miranda API.
        /// </remarks>
        public IntPtr MirandaPluginInfoEx(uint version)
        {
            MirandaPluginInfoShared(version);

            if (!SupportsMirandaVersion(version))
                return GetDummyPluginInfo();
            else
            {
                lock (SyncObject)
                    return PluginInfoExHandle.IntPtr;
            }
        }

        /// <summary>
        /// Gets the pointer to a dummy plugin info structure.
        /// </summary>
        /// <returns>Pointer.</returns>
        public static IntPtr GetDummyPluginInfo()
        {
            lock (SyncObject)
            {
                if (DummyPluginInfo == IntPtr.Zero)
                {
                    DummyPluginInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PLUGININFO)));
                    Marshal.StructureToPtr(new PLUGININFO(), DummyPluginInfo, false);
                }

                return DummyPluginInfo;
            }
        }

        /// <summary>
        /// Performs initialization steps common to all MirandaPluginInfo exports.
        /// </summary>
        /// <param name="version">Miranda version.</param>
        public void MirandaPluginInfoShared(uint version)
        {
            if (MirandaEnvironment.MirandaVersion == null)
                MirandaEnvironment.MirandaVersion = Translate.FromMirandaVersion(version);
        }

        public static bool SupportsMirandaVersion(uint version)
        {
            return SupportsMirandaVersion(Translate.FromMirandaVersion(version));
        }

        public static bool SupportsMirandaVersion(Version version)
        {
            return (version >= MinMirandaVersion);
        }

        /// <summary>
        /// Represents the MirandaPluginInterfaces export of Miranda's API.
        /// </summary>
        /// <returns>Ptr to an array of interface GUIDs.</returns>
        public IntPtr MirandaPluginInterfaces()
        {
            lock (SyncObject)
                return UUID.HyphenInterfaceUUIDs;
        }

        #endregion

        #region Load

        /// <summary>
        /// Called by a standalone module to ensure the Loader is ready (the module could be loaded before Hyphen).
        /// </summary>
        /// <param name="pPluginLink">Ptr to an instance of the PLUGINLINK structure.</param>
        /// <remarks>
        /// Calls the <see cref="Load(IntPtr)"/> to ensure that the Loader is ready. This method is needed
        /// to handle a situation when a standalone module is loaded before Hyphen. 
        /// Does nothing when Hyphen is already loaded.
        /// </remarks>
        public void ModuleInducedLoad(IntPtr pPluginLink)
        {
            Load(pPluginLink);
        }

        /// <summary>
        /// Represents the Load export of Miranda API. Loads Hyphen and initializes the runtime.
        /// </summary>
        /// <param name="pPluginLink">Ptr to an instance of the PLUGINLINK structure.</param>
        /// <returns>Result.</returns>
        public int Load(IntPtr pPluginLink)
        {
            lock (SyncObject)
            {
                Virtuoso.Miranda.Plugins.Infrastructure.RuntimeEnvironment.HyphenIsLoading = true;

                try
                {
                    // Hyphen not loaded yet...
                    if (PluginLink == null)
                    {
                        Log.Write(0, LogCategory, "Loading Hyphen...");

                        VerifyFxConfiguration();
                        EnsureSingleInstance();

                        InitializeRuntimeContext(pPluginLink);
                        HookRuntimeEvents();
                    }

                    Log.Write(0, LogCategory, "Hyphen is loaded.");
                    return CallbackResult.Success;
                }
                catch (Exception e)
                {
                    Log.Write(5, LogCategory, "Failed loading Hyphen - " + e.ToString());
                    MirandaPlugin.Hyphen.Singleton.HandleException(e, null);

                    Unload();
                    return CallbackResult.Failure;
                }
                finally
                {
                    Virtuoso.Miranda.Plugins.Infrastructure.RuntimeEnvironment.HyphenIsLoading = false;
                }
            }
        }

        /// <summary>
        /// Initializes the runtime context (including configuration).
        /// </summary>
        /// <param name="pPluginLink">Ptr to PLUGINLINK to initialize from.</param>
        private void InitializeRuntimeContext(IntPtr pPluginLink)
        {
            // Init configuration
            RuntimeConfiguration.Initialize();

            // Marshal the plugin link
            PluginLink = MirandaPluginLink.FromPointer(pPluginLink);

            // Initialize temporary context (specific to default AppDomain) for standalone modules
            MirandaContext.InitializeCurrent(PluginLink, true);
        }

        /// <summary>
        /// Hooks to the ModulesLoaded event to complete context initialization.
        /// </summary>
        private void HookRuntimeEvents()
        {
            try
            {
                HookDescriptor modulesLoadedEventHook = HookDescriptor.SetUpAndStore(InternalHooks, MirandaEvents.ME_SYSTEM_MODULESLOADED, MirandaPlugin.Hyphen.Singleton.Descriptor, CompleteInitialization, HookType.EventHook);
                HookManager.CreateHook(modulesLoadedEventHook);
            }
            catch (Exception e)
            {
                Log.Write(5, LogCategory, "Failed hooking to the modules-loaded event. Initialization failed.");
                throw new MirandaException(TextResources.ExceptionMsg_InternalErrorOccurred, e);
            }
        }

        #region Initialization

        /// <summary>
        /// Completes the runtime initialization and fires the ModulesLoaded event.
        /// </summary>
        private int CompleteInitialization(UIntPtr wParam, IntPtr lParam)
        {
            lock (SyncObject)
            {
                try
                {
                    // Unhook the event
                    HookDescriptor descriptor = InternalHooks.Find(MirandaEvents.ME_SYSTEM_MODULESLOADED);
                    HookManager.DestroyHook(descriptor);
                    InternalHooks.Remove(descriptor);

                    // Hook the shutdown event to unload Hyphen
                    HookManager.CreateHook(HookDescriptor.SetUpAndStore(InternalHooks, MirandaEvents.ME_SYSTEM_OKTOEXIT, MirandaPlugin.Hyphen.Singleton.Descriptor, UnloadOnShutdownService, HookType.EventHook));

                    InitializeUpdater();
                    InitializePluginsFolder();
                    InitializeMenu();

                    MirandaContext.Current.RaiseModulesLoadedEvent();
                }
                catch (Exception e)
                {
                    Log.Write(5, LogCategory, "Failed initializing Loader - " + e.Message);
                    Unload();
                }

                return (int)CallbackResult.Success;
            }
        }

        /// <summary>
        /// Registers Hyphen for updates via Updater.
        /// </summary>
        private static void InitializeUpdater()
        {
            if (UpdaterPlugin.IsUpdateSupported())
            {
                Update update = new Update(MirandaPlugin.Hyphen.Singleton, HyphenUpdateUrl, HyphenVersionUrl, " ");
                UpdaterPlugin.RegisterForUpdate(update);
            }
        }

        /// <summary>
        /// Initializes the FileSystemWatcher to watch for plugin changes.
        /// </summary>
        private void InitializePluginsFolder()
        {
            if (!Directory.Exists(MirandaEnvironment.ManagedPluginsFolderPath))
                Directory.CreateDirectory(MirandaEnvironment.ManagedPluginsFolderPath);

            PluginsFolderWatcher = new FileSystemWatcher(MirandaEnvironment.ManagedPluginsFolderPath, "*.dll");
            PluginsFolderWatcher.IncludeSubdirectories = false;
            PluginsFolderWatcher.NotifyFilter = NotifyFilters.LastWrite;
            PluginsFolderWatcher.Deleted += PluginsWatcherHandler;
            PluginsFolderWatcher.Changed += PluginsWatcherHandler;
            PluginsFolderWatcher.Created += PluginsWatcherHandler;
        }

        /// <summary>
        /// Populates Miranda's menu with Hyphen's items and initializes managed menu for plugins.
        /// </summary>
        private void InitializeMenu()
        {
            PluginDescriptor descriptor = MirandaPlugin.Hyphen.Singleton.Descriptor;
            ContactList clist = MirandaContext.Current.ContactList;

            // Create services for the items
            HookManager.CreateHook(HookDescriptor.SetUpAndStore(InternalHooks, LoadUnloadPluginsServiceName, descriptor, LoadUnloadPluginsService, HookType.ServiceFunction));
            HookManager.CreateHook(HookDescriptor.SetUpAndStore(InternalHooks, ConfigureModulesServiceName, descriptor, ConfigureModulesService, HookType.ServiceFunction));
            HookManager.CreateHook(HookDescriptor.SetUpAndStore(InternalHooks, ShowManagedMenuServiceName, descriptor, ShowManagedMenuService, HookType.ServiceFunction));
            HookManager.CreateHook(HookDescriptor.SetUpAndStore(InternalHooks, ManagePluginsServiceName, descriptor, ManagePluginsService, HookType.ServiceFunction));

            InitializeManagedMenu();

            MenuItemDeclarationAttribute item = new MenuItemDeclarationAttribute(TextResources.UI_Text_LoadUnloadPlugins, TextResources.UI_Text_Hyphen, LoadUnloadPluginsServiceName);
            item.IsContactMenuItem = false;
            item.HasIcon = true;
            item.UseEmbeddedIcon = true;
            item.IconID = "Virtuoso.Miranda.Plugins.Resources.LoadUnloadPlugins.ico";
            clist.AddMenuItem(MirandaPlugin.Hyphen.Singleton, item);

            item = PluginTasksItem = new MenuItemDeclarationAttribute(TextResources.UI_Text_ManagePlugins, TextResources.UI_Text_Hyphen, ManagePluginsServiceName);
            item.IsContactMenuItem = false;
            item.HasIcon = true;
            item.UseEmbeddedIcon = true;
            item.IconID = "Virtuoso.Miranda.Plugins.Resources.Configure.ico";
            clist.AddMenuItem(MirandaPlugin.Hyphen.Singleton, item);

            item = new MenuItemDeclarationAttribute(TextResources.UI_Text_ConfigureStandaloneModules, TextResources.UI_Text_Hyphen, ConfigureModulesServiceName);
            item.IsContactMenuItem = false;
            item.HasIcon = true;
            item.UseEmbeddedIcon = true;
            item.IconID = "Virtuoso.Miranda.Plugins.Resources.Configure.ico";
            clist.AddMenuItem(MirandaPlugin.Hyphen.Singleton, item);
        }

        /// <summary>
        /// Initializes the managed menu for managed plugins. This menu, unlike Miranda's, supports item removal.
        /// </summary>
        private void InitializeManagedMenu()
        {
            ManagedMainMenu = new ManagedMainMenu();

            MenuItemDeclarationAttribute item = new MenuItemDeclarationAttribute(TextResources.UI_Text_ShowManagedMainMenu, ShowManagedMenuServiceName);
            item.IsContactMenuItem = false;
            item.HasIcon = true;
            item.UseEmbeddedIcon = true;
            item.IconID = "Virtuoso.Miranda.Plugins.Resources.ShowManagedMenuItems.ico";

            // Add a proxy item for the managed menu into M's menu (on behalf of a dummy plugin)
            MirandaContext.Current.ContactList.AddMenuItem(MirandaPlugin.Hyphen.Singleton, item);
        }

        #endregion

        #endregion

        #region Unload

        /// <summary>
        /// Represents the Unload export of Miranda API. Unloads Hyphen and shuts down the runtime.
        /// </summary>
        /// <returns>Unload result.</returns>
        /// <remarks>
        /// Called by the UnloadOnShutdownService to unload managed plugins before Miranda does.
        /// Managed plugins SHOULD NOT be unloaded when Miranda's unloading, because I do not consider it safe 
        /// (i.e. heap corruption is likely to occur).
        /// </remarks>
        public int Unload()
        {
            lock (SyncObject)
            {
                try
                {
                    Log.Write(0, LogCategory, "Hyphen unload begin.");

                    if (Unloaded)
                        return (int)CallbackResult.Success;

                    if (RuntimeConfiguration.Initialized)
                        RuntimeConfiguration.Singleton.Save();

                    bool lazy = RuntimeConfiguration.Singleton.UseLazyUnload;

                    if (PluginsLoaded)
                        UnloadPlugins(lazy);

                    // Lazy unload (only Tray disposal, all other resources will be cleaned by the CLR)
                    if (lazy)
                    {
                        Log.Write(0, LogCategory, "Lazy unload completed.");
                    }
                    // Complete unload
                    else
                    {
                        PerformFullUnload();
                    }

                    return (int)CallbackResult.Success;
                }
                catch (Exception e)
                {
                    Log.Write(5, LogCategory, "Failed unloading Hyphen.\n" + e.ToString());
                    return (int)CallbackResult.Failure;
                }
                finally
                {
                    Unloaded = true;
                }
            }
        }

        /// <summary>
        /// Unloads Hyphen completelly.
        /// </summary>
        private void PerformFullUnload()
        {
            DisposePluginsWatcher();

            PluginInfoHandle.Free();
            PluginInfoExHandle.Free();

            InteropBufferPool.Dispose();            

            // Invalidate context only if there are no standalone modules that may depend on it
            if (MirandaContext.Initialized && !ModuleManager.Singleton.HasModules)
                MirandaContext.InvalidateCurrent();

            Log.Write(0, LogCategory, "Unload completed.");
        }

        /// <summary>
        /// Disposes the plugin's folder watcher (if needed).
        /// </summary>
        private void DisposePluginsWatcher()
        {
            if (PluginsFolderWatcher != null)
                PluginsFolderWatcher.Dispose();
        }

        /// <summary>
        /// Disposes the Tray manager (if needed).
        /// </summary>
        private void DisposePluginTasksMenu()
        {
            /*if (trayManager != null)
                trayManager.Dispose();*/
        }

        /// <summary>
        /// Broadcasts the BeforeShutdown event to the managed plugins and unloads Hyphen on Miranda's shutdown.
        /// </summary>
        private int UnloadOnShutdownService(UIntPtr wParam, IntPtr lParam)
        {
            try
            {
                lock (SyncObject)
                {
                    HookDescriptor descriptor = InternalHooks.Find(MirandaEvents.ME_SYSTEM_OKTOEXIT);
                    HookManager.DestroyHook(descriptor);
                    InternalHooks.Remove(descriptor);

                    if (PluginsLoaded)
                        IsolatedPluginsSandbox.PluginManager.DoContextCallback<object>(BroadcastBeforeMirandaExitEvent, null);

                    Unload();
                }
            }
            catch (Exception e)
            {
                Log.Write(5, LogCategory, "Unable to unload Hyphen from UnloadOnShutdownService: " + e.Message);
            }

            return 0;
        }

        /// <summary>
        /// Broadcasts the BeforeShutdown event to the managed plugins.
        /// </summary>
        private static void BroadcastBeforeMirandaExitEvent(PluginManagerBase sender, object state)
        {
            try
            {
                SynchronizationHelper.BeginCollectionUpdate(sender.Plugins);

                foreach (PluginDescriptor descriptor in sender.Plugins)
                {
                    try
                    {
                        SynchronizationHelper.BeginDescriptorUpdate(descriptor);
                        descriptor.Plugin.BeforeMirandaShutdownInternal();
                    }
                    finally
                    {
                        SynchronizationHelper.EndUpdate(descriptor);
                    }
                }
            }
            finally
            {
                SynchronizationHelper.EndUpdate(sender.Plugins);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Loader Control

        #region Loading

        #region Core

        /// <summary>
        /// Loads / unloads managed plugins.
        /// </summary>
        private int LoadUnloadPluginsService(UIntPtr wParam, IntPtr lParam)
        {
            lock (SyncObject)
            {
                try
                {
                    if (!PluginsLoaded)
                        LoadPlugins();
                    else if (DialogResult.Yes == MessageBox.Show(TextResources.MsgBox_Text_LoadUnloadPlugins_Unload, TextResources.MsgBox_Caption_LoadUnloadPlugins, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        UnloadPlugins();
                }
                catch (Exception e)
                {
                    MirandaPlugin.Hyphen.Singleton.HandleException(e, null);
                }
            }

            return (int)CallbackResult.Success;
        }

        /// <summary>
        /// Reloads managed plugins.
        /// </summary>
        private void ReloadPlugins()
        {
            UnloadPlugins();
            LoadPlugins();
        }

        /// <summary>
        /// Loads managed plugins.
        /// </summary>
        private void LoadPlugins()
        {
            lock (SyncObject)
            {
                if (PluginsLoaded)
                    throw new InvalidOperationException(TextResources.ExceptionMsg_InternalErrorOccurred);

                InitializePluginTasksMenu();

                // Show the progress dialog => start fusion
                FusionProgressDialog.ShowDialog(StartFusion);
            }
        }

        /// <summary>
        /// Called by the FusionProgressDialog on another thread to perform the fusion itself.
        /// </summary>
        private void StartFusion()
        {
            Log.Write(0, LogCategory, "Loading plugins...");

            InitializeSandbox();
            InitializeFusionContext();

            LoadPluginManager();

            try
            {
                IsolatedPluginsSandbox.PluginManager.FindAndLoadPlugins();
                ClearStringResolverCache();
            }
            catch (Exception e)
            {
                UnloadPlugins();
                TrapUnhandledException(this, new UnhandledExceptionEventArgs(e, false));
            }
        }

        /// <summary>
        /// Initializes a sandbox for plugins.
        /// </summary>
        private void InitializeSandbox()
        {
            IsolatedPluginsSandbox = new PluginsSandbox();
            IsolatedPluginsSandbox.SetUnhandledExceptionHandler(TrapUnhandledException);
            IsolatedPluginsSandbox.LoadAssemblyProbe();
        }

        /// <summary>
        /// Initializes a fusion context.
        /// </summary>
        private void InitializeFusionContext()
        {
            FusionContext = new FusionContext(this, IsolatedPluginsSandbox.AssemblyProbe, PluginLink.NativePluginLinkPtr);
        }

        /// <summary>
        /// Loads the Plugin Manager.
        /// </summary>
        private void LoadPluginManager()
        {
            IsolatedPluginsSandbox.LoadPluginManager(FusionContext);

            // Associate the PM with the default context for management purposes
            MirandaContext.Current.AssociatePluginManager(IsolatedPluginsSandbox.PluginManager);

            IsolatedPluginsSandbox.PluginManager.FusionCompleted += delegate
            {
                PluginsLoadedEvent.Set();
                PluginsFolderWatcher.EnableRaisingEvents = true;
            };

            PromoteManagedMenuIntoAppDomain(IsolatedPluginsSandbox.PluginManager);
        }

        /// <summary>
        /// Initializes the Tray Manager.
        /// </summary>
        public void InitializePluginTasksMenu()
        {
            /*InitializeUISyncContext();
            UIThreadSyncContext.Send(delegate { trayManager = new TrayMenuManager(true); }, null);*/
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Initializes the UI sync context for event dispathing on the current thread.
        /// </summary>
        private void InitializeUISyncContext()
        {
            if (UIThreadSyncContext == null)
                UIThreadSyncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        /// <summary>
        /// Registers managed main menu for Miranda's AddMenuItem services in an AppDomain.
        /// </summary>
        /// <param name="pluginManager">Plugin manager.</param>
        public void PromoteManagedMenuIntoAppDomain(PluginManagerBase pluginManager)
        {
            if (pluginManager == null)
                throw new ArgumentNullException("pluginManager");

            if (ManagedMainMenu != null)
            {
                pluginManager.DoContextCallback(delegate(PluginManagerBase _sender, ManagedMainMenu _menu)
                {
                    ManagedMainMenu.RegisterInterceptors(_menu);
                }, ManagedMainMenu);
            }
        }

        /// <summary>
        /// Clears the StringResolver cache (plugins are loaded and resolvers are junk now).
        /// </summary>
        private static void ClearStringResolverCache()
        {
            StringResolverCache cache = StringResolverCache.Singleton;

            lock (cache)
                cache.Clear();
        }

        #endregion

        #endregion

        #region Services

        /// <summary>
        /// Shows managed menu.
        /// </summary>
        private int ShowManagedMenuService(UIntPtr wParam, IntPtr lParam)
        {
            lock (SyncObject)
            {
                // If there are no standalone modules and managed plugins are not loaded, LOAD THEM AND SHOW THE MENU
                if (!ModuleManager.Singleton.HasModules && !PluginsLoaded)
                {
                    // Async
                    LoadPlugins();
                    PluginsLoadedEvent.WaitOne(10000, false);
                }

                if (ManagedMainMenu != null)
                    ManagedMainMenu.ShowUnderCursor();
            }

            return (int)CallbackResult.Success;
        }

        /// <summary>
        /// Shows a configuration dialog to configure standalone modules.
        /// </summary>
        private int ConfigureModulesService(UIntPtr wParam, IntPtr lParam)
        {
            ConfigurationDialog.Present(false);
            return (int)CallbackResult.Success;
        }

        /// <summary>
        /// Shows Isolated-plugins management dialog.
        /// </summary>
        private int ManagePluginsService(UIntPtr wParam, IntPtr lParam)
        {
            if (!PluginsLoaded)
                MessageBox.Show(TextResources.MsgBox_Text_NoPluginsLoaded, TextResources.MsgBox_Caption_NoPluginsLoaded, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                ManagePlugins();

            return CallbackResult.Success;
        }

        /// <summary>
        /// Shows the plugin management dialog.
        /// </summary>
        public void ManagePlugins()
        {
            if (PluginsLoaded)
            {
                MirandaContext.Current.PluginManager.DoContextCallback<object>(delegate
                {
                    ConfigurationDialog.Present(false, ConfigurationDialog.CreatePath(PMConfigurator.Singleton, TextResources.Config_Management, TextResources.Config_Management_Plugins));
                }, null);
            }
        }

        /// <summary>
        /// Handles a plugin file change.
        /// </summary>
        private void PluginsWatcherHandler(object sender, FileSystemEventArgs e)
        {
            lock (SyncObject)
            {
                try
                {
                    if (!PluginsLoaded)
                        return;

                    if (DialogResult.Yes == MessageBox.Show(TextResources.MsgBox_Text_PluginUpdated, String.Format(TextResources.MsgBox_Formatable1_Caption_PluginUpdated, e.Name.Substring(0, e.Name.Length - 4)), MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        // Async
                        ReloadPlugins();
                        PluginsLoadedEvent.WaitOne(10000, false);

                        MessageBox.Show(TextResources.MsgBox_Text_PluginReloadComplete, TextResources.MsgBox_Caption_PluginReloadComplete, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(0, LogCategory, "Plugin reload failed: " + ex.ToString());
                }
            }
        }

        #endregion

        #region Unloading

        /// <summary>
        /// Unloads managed plugins.
        /// </summary>
        public void UnloadPlugins()
        {
            UnloadPlugins(false);
        }

        /// <summary>
        /// Unloads managed plugins.
        /// </summary>
        /// <param name="lazy">TRUE to perform fast unload only; FALSE to perfrom full unload.</param>
        public void UnloadPlugins(bool lazy)
        {
            lock (SyncObject)
            {
                if (!PluginsLoaded)
                    throw new InvalidOperationException(TextResources.ExceptionMsg_InternalErrorOccurred);

                DisposePluginTasksMenu();

                PluginsFolderWatcher.EnableRaisingEvents = false;
                PluginsLoadedEvent.Reset();

                MirandaContext.Current.DetachPluginManager();
                IsolatedPluginsSandbox.PluginManager.Shutdown(lazy);

                if (!lazy)
                    Sandbox.Unload(IsolatedPluginsSandbox);

                IsolatedPluginsSandbox = null;
            }
        }

        #endregion

        #endregion

        #region Misc.

        /// <summary>
        /// Handles Hyphen (default AppDomain) unhandled exceptions.
        /// </summary>
        private void TrapUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MirandaPlugin.Hyphen.Singleton.HandleException((Exception)e.ExceptionObject, null);
        }

        /// <summary>
        /// Handles Windows Forms (default AppDomain) unhandled exceptions.
        /// </summary>
        private void TrapUnhandledException(object sender, ThreadExceptionEventArgs e)
        {
            TrapUnhandledException(sender, new UnhandledExceptionEventArgs(e.Exception, false));
        }

        /// <summary>
        /// Verifies the .config file is present and when it is not, the default one is created.
        /// </summary>
        private static void VerifyFxConfiguration()
        {
            string configFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            if (String.IsNullOrEmpty(configFileName) || !File.Exists(configFileName))
            {
                using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Virtuoso.Miranda.Plugins.Resources.miranda32.exe.config")))
                using (StreamWriter writer = new StreamWriter(configFileName))
                    writer.Write(reader.ReadToEnd());

                MessageBox.Show(TextResources.MsgBox_Text_MirandaRestartRequired, TextResources.ExceptionMsg_MirandaRestartRequired, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                throw new MirandaException(TextResources.ExceptionMsg_MirandaRestartRequired);
            }
        }

        /// <summary>
        /// Ensures only a single instance of Hyphen is loaded into the process.
        /// </summary>
        private void EnsureSingleInstance()
        {
            bool acquired = false;
            SingleInstanceMutex = new Mutex(true, GetSingletonMutexName(), out acquired);

            if (!acquired)
                throw new NotSupportedException(TextResources.ExceptionMsg_HyphenSxSNotSupported);
        }

        /// <summary>
        /// Formats singleton's mutex name.
        /// </summary>
        /// <returns>Mutext name.</returns>
        private static string GetSingletonMutexName()
        {
            return String.Format("{0}::Hyphen", Process.GetCurrentProcess().Id.ToString());
        }

        /// <summary>
        /// Invokes a delegate on behalf of Loader's AppDomain (i.e. the default one).
        /// </summary>
        /// <param name="del">Delegate.</param>
        /// <param name="args">Optional arguments.</param>
        /// <returns></returns>
        public object DoContextCallback(Delegate del, params object[] args)
        {
            if (del == null)
                throw new ArgumentNullException("del");

            return del.DynamicInvoke(args);
        }

        /// <summary>
        /// Verifies whether the code is executing in the default AppDomain.
        /// </summary>
        /// <exception cref="NotSupportedException">The code is not executing in the default AppDomain.</exception>
        public static void VerifyDefaultDomain()
        {
            if (!AppDomain.CurrentDomain.IsDefaultAppDomain())
                throw new NotSupportedException(TextResources.ExceptionMsg_FeatureNotAvailableInDomain);
        }

        #endregion
    }
}