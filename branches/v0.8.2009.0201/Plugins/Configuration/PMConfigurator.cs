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
using Virtuoso.Miranda.Plugins.Configuration;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Miranda.Plugins.Configuration.Forms.Controls;

namespace Virtuoso.Miranda.Plugins.Configuration
{
    internal sealed class PMConfigurator : IInternalConfigurator
    {
        #region .ctors

        private PMConfigurator() { }

        #endregion

        #region Properties

        private static IConfigurablePlugin singleton;
        public static IConfigurablePlugin Singleton
        {
            get
            {
                return singleton ?? (singleton = new PMConfigurator());
            }
        }

        #endregion

        #region IConfigurablePlugin Members

        public string Name
        {
            get { return "Plugins"; }
        }

        private PluginConfiguration[] configuration;
        public PluginConfiguration[] Configuration
        {
            get { return configuration ?? (configuration = new PluginConfiguration[] { PMConfiguration.Singleton }); }
        }

        public void PopulateConfiguration(CategoryCollection categories)
        {
            Category category = new Category(TextResources.Config_Management, TextResources.Config_Management_Description);
            categories.Add(category);

            CategoryItem item = new CategoryItem(TextResources.Config_Management_Plugins, TextResources.Config_Management_Plugins_Description, typeof(PluginManagementContent));
            item.Image = VisualResources.Image_64x67_Management;
            category.Items.Add(item);
        }

        public void ResetConfiguration()
        {
            PMConfiguration.Reset();
        }

        public void ReloadConfiguration()
        {
            PMConfiguration.Reload();
        }

        #endregion
    }
}
