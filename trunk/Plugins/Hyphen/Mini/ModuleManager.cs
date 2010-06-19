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
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Hyphen.Mini
{
    public sealed class ModuleManager
    {
        #region Fields

        private static readonly object SyncObject = new object();
        private static ModuleManager singleton;

        private readonly ModuleCollection RegistredModulesCollection = new ModuleCollection();
        private readonly ModuleReadOnlyCollection registeredModules;

        #endregion

        #region .ctors

        private ModuleManager()
        {
            this.registeredModules = new ModuleReadOnlyCollection(RegistredModulesCollection);
        }

        #endregion

        #region Properties

        public static ModuleManager Singleton
        {
            get
            {
                Loader.VerifyDefaultDomain();

                lock (SyncObject)
                    return singleton ?? (singleton = new ModuleManager());
            }
        }

        public ModuleReadOnlyCollection RegisteredModules
        {
            get
            {
                return registeredModules;
            }
        }

        public bool HasModules
        {
            get
            {
                return registeredModules.Count > 0;
            }
        }

        #endregion

        #region Methods

        internal void RegisterModule(Module module)
        {
            if (module == null) 
                throw new ArgumentNullException("module");

            RegistredModulesCollection.Add(module);
        }

        internal void UnregisterModule(Module module)
        {
            if (module == null) 
                throw new ArgumentNullException("module");

            RegistredModulesCollection.Remove(module);
        }

        #endregion
    }
}
