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
using System.Reflection;
using Virtuoso.Miranda.Plugins.Resources;
using System.Security;
using System.Security.Policy;
using System.IO;
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.Diagnostics;
using System.Security.Permissions;

namespace Virtuoso.Miranda.Plugins.Helpers
{
    /* CLASS ORIGIN: Loki project */
    public abstract class Sandbox : RemoteObject
    {
        #region Fields

        private AppDomain hostingAppDomain;
        protected AppDomain HostingAppDomain
        {
            get { return hostingAppDomain; }
        }

        #endregion

        #region Nested classes

        private sealed class MirandaContextInitHelper : RemoteObject
        {
            public MirandaContextInitHelper(MirandaContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                MirandaContext.InvalidateCurrent();
                MirandaContext.InitializeCurrent(context);
            }
        }

        #endregion

        #region .ctors

        protected Sandbox() { }

        public static void Unload(Sandbox sandbox)
        {
            if (sandbox == null)
                throw new ArgumentNullException("sandbox");

            if (sandbox.hostingAppDomain == null)
                throw new ArgumentException();

            if (sandbox.hostingAppDomain == AppDomain.CurrentDomain)
                throw new InvalidOperationException(TextResources.ExceptionMsg_UnableToUnloadPluginMangerFromCurrentAppDomain);

            sandbox.OnSandboxUnload();
            sandbox.UnloadHostingAppDomain();
        }

        #endregion

        #region Virtuals

        protected virtual void InitializeAppDomainSetup(AppDomainSetup domainSetup) { }

        protected virtual void OnSandboxUnload() { }

        #endregion

        #region Helpers

        protected static StrongName GetStrongName(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            AssemblyName assemblyName = assembly.GetName();
            Debug.Assert(assemblyName != null, "Could not get assembly name");

            byte[] publicKey = assemblyName.GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
                throw new InvalidOperationException(String.Format("{0} is not strongly named", assembly));

            StrongNamePublicKeyBlob keyBlob = new StrongNamePublicKeyBlob(publicKey);
            return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
        }

        #endregion

        #region Methods

        protected void SetUpHostingAppDomain(string name)
        {
            SetUpHostingAppDomain(name, null, null);
        }

        protected void SetUpHostingAppDomain(string name, Evidence evidence, PermissionSet permissions, params StrongName[] fullTrust)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (hostingAppDomain != null)
                throw new InvalidOperationException();

            AppDomainSetup currentSetup = AppDomain.CurrentDomain.SetupInformation;
            AppDomainSetup domainSetup = new AppDomainSetup();

            domainSetup.ApplicationName = name;
            domainSetup.ApplicationBase = currentSetup.ApplicationBase;
            domainSetup.PrivateBinPath = String.Format("{0};{1};", MirandaEnvironment.MirandaPluginsFolderRelativePath, MirandaEnvironment.ManagedPluginsFolderRelativePath);
            domainSetup.ConfigurationFile = currentSetup.ConfigurationFile;

            InitializeAppDomainSetup(domainSetup);

            if (permissions == null)
                hostingAppDomain = AppDomain.CreateDomain(name, null, domainSetup);
            else
                hostingAppDomain = AppDomain.CreateDomain(name, evidence, domainSetup, permissions, fullTrust);
        }

        protected void UnloadHostingAppDomain()
        {
            if (hostingAppDomain == null)
                throw new InvalidOperationException();
            else
            {
                AppDomain.Unload(hostingAppDomain);
                hostingAppDomain = null;
            }
        }

        protected T InstantiateRemoteObject<T>(string assemblyName, string typeName, params object[] args) where T : class
        {
            return Activator.CreateInstance(hostingAppDomain, assemblyName, typeName, true, BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public, null, args, null, null, null).Unwrap() as T;
        }

        protected T InstantiateRemoteObjectFrom<T>(string assemblyFile, string typeName, params object[] args) where T : class
        {
            return Activator.CreateInstanceFrom(hostingAppDomain, assemblyFile, typeName, true, BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public, null, args, null, null, null).Unwrap() as T;
        }

        protected void InitializeRemoteContext(MirandaContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (hostingAppDomain == null)
                throw new InvalidOperationException();

            InstantiateRemoteObject<MirandaContextInitHelper>(Assembly.GetExecutingAssembly().FullName, typeof(MirandaContextInitHelper).FullName, context);
        }

        public void SetUnhandledExceptionHandler(UnhandledExceptionEventHandler handler)
        {
            hostingAppDomain.UnhandledException += handler;
        }

        public void RemoveUnhandledExceptionHandler(UnhandledExceptionEventHandler handler)
        {
            hostingAppDomain.UnhandledException -= handler;
        }

        #endregion
    }
}
