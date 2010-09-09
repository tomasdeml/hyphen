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

namespace Virtuoso.Miranda.Plugins
{
    [ConfigurationOptions("0.1.0.0", ProfileBound = true), Serializable]
    internal class PMConfiguration : PluginConfiguration
    {
        #region .ctors
        
        private PMConfiguration() { }

        protected override void InitializeDefaultConfiguration()
        {
            disabledPlugins = new List<string>(1);
            base.InitializeDefaultConfiguration();
        }

        public static void Initialize()
        {
            if (singleton != null)
                throw new InvalidOperationException();

            singleton = Load<PMConfiguration>();
        }

        #endregion

        #region Properties

        private static PMConfiguration singleton;
        public static PMConfiguration Singleton
        {
            get
            {
                if (singleton == null)
                    throw new InvalidOperationException();

                return singleton;
            }
        }

        private List<string> disabledPlugins;
        public List<string> DisabledPlugins
        {
            get { return disabledPlugins; }
            set { disabledPlugins = value; }
        }

        #endregion

        #region Methods

        public static void Reset()
        {
            singleton = PluginConfiguration.GetDefaultConfiguration<PMConfiguration>();
        }

        public static void Reload()
        {
            Initialize();
        }

        #endregion
    }
}
