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
using Virtuoso.Miranda.Plugins.Configuration.Forms.Controls;
using Virtuoso.Miranda.Plugins.Configuration.Forms;
using Virtuoso.Miranda.Plugins.Configuration;

namespace Virtuoso.Miranda.Plugins
{
    public abstract class ConfigurableMirandaPlugin<TConfiguration> : MirandaPlugin, IConfigurablePluginBase<TConfiguration> where TConfiguration : PluginConfiguration
    {
        #region Fields

        private TConfiguration pluginConfiguration;
        public TConfiguration PluginConfiguration
        {
            get { return pluginConfiguration; }
        }

        private PluginConfiguration[] configuration;
        PluginConfiguration[] IConfigurablePlugin.Configuration
        {
            get { return configuration ?? (configuration = new PluginConfiguration[] { pluginConfiguration }); }
        }

        #endregion

        #region .ctors

        protected ConfigurableMirandaPlugin() { }

        #endregion

        #region Methods

        internal override void AfterPluginInitializationInternal()
        {
            pluginConfiguration = Infrastructure.PluginConfiguration.Load<TConfiguration>();
            base.AfterPluginInitializationInternal();
        }

        internal override void BeforePluginDisableInternal()
        {
            SaveConfiguration();
            base.BeforePluginDisableInternal();
        }

        internal override void BeforeMirandaShutdownInternal()
        {
            SaveConfiguration();
            base.BeforeMirandaShutdownInternal();
        }

        private void SaveConfiguration()
        {
            //if (pluginConfiguration.IsDirty)
            pluginConfiguration.Save();
        }

        public void ResetConfiguration()
        {
            pluginConfiguration = Infrastructure.PluginConfiguration.GetDefaultConfiguration<TConfiguration>();
        }

        public void ReloadConfiguration()
        {
            pluginConfiguration = Infrastructure.PluginConfiguration.Load<TConfiguration>();
        }

        public abstract void PopulateConfiguration(CategoryCollection categories);

        #endregion

        #region Properties

        public override bool HasOptions
        {
            get { return true; }
        }

        #endregion
    }
}
