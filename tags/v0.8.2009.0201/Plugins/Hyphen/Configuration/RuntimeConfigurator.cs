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
using Virtuoso.Miranda.Plugins;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Hyphen.Configuration.Controls;

namespace Virtuoso.Hyphen.Configuration
{
    internal sealed class RuntimeConfigurator : IInternalConfigurator
    {
        #region .ctors

        private RuntimeConfigurator() { }

        #endregion

        #region Properties

        private static IConfigurablePlugin singleton;
        public static IConfigurablePlugin Singleton
        {
            get
            {
                return singleton ?? (singleton = new RuntimeConfigurator());
            }
        }

        public string Name
        {
            get { return "Hyphen"; }
        }

        private PluginConfiguration[] configuration;
        public PluginConfiguration[] Configuration
        {
            get { return configuration ?? (configuration = new PluginConfiguration[] { RuntimeConfiguration.Singleton }); }
        }

        public void ResetConfiguration()
        {
            RuntimeConfiguration.Reset();
        }

        public void ReloadConfiguration()
        {
            RuntimeConfiguration.Reload();
        }
        #endregion

        #region Handlers

        public void PopulateConfiguration(CategoryCollection categories)
        {
            Category category = new Category(TextResources.Config_General, TextResources.Config_General_Description);

            CategoryItem item = new CategoryItem(TextResources.Config_General_About, TextResources.Config_General_About_Description, typeof(AboutContent));
            item.Image = VisualResources.Image_64x67_Information;
            category.Items.Add(item);

            categories.Add(category);
        }

        #endregion
    }
}
