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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Miranda.Plugins.Properties;
using Virtuoso.Miranda.Plugins.Configuration.Forms.Controls;
using RibbonStyle;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Hyphen;
using Virtuoso.Hyphen.Mini;
using System.Collections.ObjectModel;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Hyphen.Configuration;

namespace Virtuoso.Miranda.Plugins.Configuration.Forms
{
    /// <codename>Casablanca</codename>
    public sealed partial class ConfigurationDialog : SingletonDialog
    {
        #region Fields

        private bool Ok;
        private List<ConfigurableEntityDescriptor> ConfigurableEntities;

        private const char PathSeparator = '/';

        private string[] CurrentPath;
        private string ConfigurationParameter;

        private CategoryCollection categories;

        #endregion

        #region .ctors

        private ConfigurationDialog()
        {
            InitializeComponent();
            ((RibbonStyle.TabStripProfessionalRenderer)RibbonStrip.Renderer).HaloColor = Color.FromArgb(254, 209, 94);
            ((RibbonStyle.TabStripProfessionalRenderer)RibbonStrip.Renderer).BaseColor = Color.FromArgb(215, 227, 242);

            categories = new CategoryCollection();
            ConfigurableEntities = new List<ConfigurableEntityDescriptor>(5);
        }

        public static void Present(bool modal)
        {
            Present(modal, null, null);
        }

        public static void Present(bool modal, IConfigurablePlugin plugin)
        {
            Present(modal, plugin, null);
        }

        public static void Present(bool modal, string path)
        {
            Present(modal, null, path);
        }

        public static void Present(bool modal, IConfigurablePlugin plugin, string path)
        {
            ConfigurationDialog dialog = ConfigurationDialog.GetSingleton<ConfigurationDialog>(false);

            if (dialog == null)
            {
                dialog = new ConfigurationDialog();

                dialog.SetPath(path);
                dialog.Populate(plugin);
            }

            dialog.ShowSingleton(modal);
        }

        #endregion

        #region UI Handlers

        private void OkBTN_Click(object sender, EventArgs e)
        {
            Ok = true;
            Close();
        }

        private void CancelBTN_Click(object sender, EventArgs e)
        {
            Ok = false;
            Close();
        }

        private void ConfigurationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Ok && !CanDismissActiveControl())
            {
                e.Cancel = true;
                return;
            }

            foreach (ConfigurableEntityDescriptor descriptor in ConfigurableEntities)
                ProcessChanges(descriptor, Ok);

            if (Ok)
                Settings.Default.Save();
        }

        private void ProcessChanges(ConfigurableEntityDescriptor descriptor, bool save)
        {
            foreach (Category category in descriptor.Categories)
            {
                foreach (CategoryItem item in category.Items)
                {
                    if (!item.ControlInitialized)
                        continue;

                    CategoryItemControl control = item.Control;

                    if (save && control.IsDirty)
                        control.Save();

                    control.Close();
                }
            }

            if (save)
            {
                foreach (PluginConfiguration config in descriptor.Plugin.Configuration)
                    if (config != null)
                        config.Save();
            }
        }

        #endregion

        #region Methods

        #region Create/Set path

        public static string CreatePath(IConfigurablePlugin plugin, string category, string item)
        {
            return CreatePath(plugin, category, item, null);
        }

        public static string CreatePath(IConfigurablePlugin plugin, string category, string item, string parameter)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            if (String.IsNullOrEmpty(category))
                throw new ArgumentNullException("category");

            if (String.IsNullOrEmpty(item))
                throw new ArgumentNullException("item");

            return String.Format("{1}{0}{2}{0}{3}{0}{4}", PathSeparator.ToString(), plugin.Name, category, item, (parameter ?? String.Empty));
        }

        private void SetPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return;

            CurrentPath = path.Split(PathSeparator);            

            if (CurrentPath.Length != 4)
                throw new ArgumentOutOfRangeException("path");

            ConfigurationParameter = (String.IsNullOrEmpty(CurrentPath[3]) ? null : CurrentPath[3]);
        }

        private bool CanNavigateTo(IConfigurablePlugin plugin, Category category)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            if (category == null)
                throw new ArgumentNullException("category");

            if (CurrentPath == null)
                return false;

            return (CurrentPath[0] == plugin.Name && CurrentPath[1] == category.Name);
        }

        private bool CanNavigateTo(CategoryItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (CurrentPath == null)
                return false;

            return CurrentPath[2] == item.Name;
        }

        #endregion

        #region Entity population

        private void Populate(IConfigurablePlugin pluginToPopulate)
        {
            ConfigurableEntities.Clear();

            RibbonPageSwitcher.SelectedTabStripPage = null;
            RibbonStrip.Items.Clear();

            if (pluginToPopulate == null)
            {
                PopulateHyphenConfiguration();

                if (AppDomain.CurrentDomain.IsDefaultAppDomain())
                    PopulateStandalonePlugins();
                else
                    PopulateIsolatedPlugins();
            }
            else
            {
                ConfigurableEntities.Add(new ConfigurableEntityDescriptor(pluginToPopulate));
                Text += String.Format(" : {0}", pluginToPopulate.Name);
            }

            PopulateItems();
        }

        private void PopulateItems()
        {
            CategoryCollection categories = null;

            for (int i = 0; i < ConfigurableEntities.Count; i++)
            {
                ConfigurableEntityDescriptor descriptor = ConfigurableEntities[i];
                TabStripPage page = CreateAndRegisterEntityPage(categories, descriptor.Plugin, i);

                descriptor.Categories = (categories = new CategoryCollection());
                descriptor.Plugin.PopulateConfiguration(categories);

                for (int j = 0; j < categories.Count; j++)
                {
                    Category category = categories[j];

                    if (category == null)
                        continue;

                    PopulateCategoryPanel(page, category, descriptor.Plugin, j);
                }
            }
        }

        private void PopulateIsolatedPlugins()
        {
            PluginDescriptorReadOnlyCollection plugins = MirandaContext.Current.PluginManager.Plugins;

            try
            {
                SynchronizationHelper.BeginCollectionUpdate(plugins);

                foreach (PluginDescriptor descriptor in plugins)
                    if (descriptor.IsConfigurable)
                        ConfigurableEntities.Add(new ConfigurableEntityDescriptor((IConfigurablePlugin)descriptor.Plugin));
            }
            finally
            {
                SynchronizationHelper.EndUpdate(plugins);
            }
        }

        private void PopulateStandalonePlugins()
        {
            ModuleReadOnlyCollection modules = ModuleManager.Singleton.RegisteredModules;

            try
            {
                SynchronizationHelper.BeginCollectionUpdate(modules);

                foreach (Module module in modules)
                    if (module.StandalonePlugin.Descriptor.IsConfigurable)
                        ConfigurableEntities.Add(new ConfigurableEntityDescriptor((IConfigurablePlugin)module.StandalonePlugin));
            }
            finally
            {
                SynchronizationHelper.EndUpdate(modules);
            }
        }

        private void PopulateHyphenConfiguration()
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
                ConfigurableEntities.Add(new ConfigurableEntityDescriptor(RuntimeConfigurator.Singleton));
            else
                ConfigurableEntities.Add(new ConfigurableEntityDescriptor(PMConfigurator.Singleton));
        }

        #endregion

        #region Entity control creation

        private void PopulateCategoryPanel(TabStripPage entityPage, Category category, IConfigurablePlugin plugin, int index)
        {
            TabPanel categoryPanel = CreateCategoryPanel(category, index);
            entityPage.Controls.Add(categoryPanel);

            if (CanNavigateTo(plugin, category))
                RibbonPageSwitcher.SelectedTabStripPage = entityPage;

            Point nextLocation = new Point();

            foreach (CategoryItem item in category.Items)
            {
                RibbonButton btn = CreateButton(item, ref nextLocation);
                categoryPanel.Controls.Add(btn);

                if (CanNavigateTo(item))
                    RibbonButton_Click(btn, EventArgs.Empty);
            }
        }

        private RibbonButton CreateButton(CategoryItem item, ref Point nextLocation)
        {
            RibbonButton btn = new RibbonButton();
            btn.Text = item.Name;

            btn.ShowInfoTips = true;
            btn.InfoTitle = item.Name;
            btn.InfoComment = item.Description;

            if (item.Image == null)
                item.Image = VisualResources.Image_64x67_Configuration;

            btn.img = item.Image;
            btn.Size = btn.img.Size;

            if (nextLocation.IsEmpty)
                nextLocation = new Point(-btn.Size.Width, 6);

            btn.img_on = Properties.Resources.RibbonHover;
            btn.img_click = Properties.Resources.RibbonClick;

            nextLocation.Offset(btn.img.Size.Width + 5, 0);
            btn.Location = nextLocation;

            btn.Tag = item;
            btn.Click += RibbonButton_Click;

            return btn;
        }

        private TabPanel CreateCategoryPanel(Category category, int index)
        {
            TabPanel categoryPanel = new TabPanel();

            categoryPanel.BaseColor = System.Drawing.Color.FromArgb(215, 227, 242);
            categoryPanel.BaseColorOn = System.Drawing.Color.FromArgb(233, 239, 248);
            categoryPanel.Dock = System.Windows.Forms.DockStyle.Left;
            categoryPanel.ForeColor = System.Drawing.SystemColors.Desktop;
            categoryPanel.Location = new System.Drawing.Point(101, 3);
            categoryPanel.Opacity = 255;
            categoryPanel.Padding = new System.Windows.Forms.Padding(6, 3, 6, 0);
            categoryPanel.AutoSize = true;
            categoryPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            categoryPanel.Speed = 1;
            categoryPanel.TabIndex = index;
            categoryPanel.Caption = category.Name;

            return categoryPanel;
        }

        private TabStripPage CreateAndRegisterEntityPage(CategoryCollection categories, IConfigurablePlugin entity, int index)
        {
            TabStripPage entityPage = new TabStripPage();
            entityPage.BaseColor = System.Drawing.Color.FromArgb(215, 227, 242);
            entityPage.BaseColorOn = System.Drawing.Color.FromArgb(215, 227, 242);
            entityPage.Dock = System.Windows.Forms.DockStyle.Fill;
            entityPage.Opacity = 255;
            entityPage.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            entityPage.Size = new System.Drawing.Size(784, 99);
            entityPage.Speed = 1;
            entityPage.TabIndex = index;

            RibbonPageSwitcher.Controls.Add(entityPage);

            Tab entityTab = new Tab(entity.Name);
            entityTab.AutoSize = false;
            entityTab.Checked = true;
            entityTab.CheckState = CheckState.Checked;
            entityTab.ForeColor = System.Drawing.Color.FromArgb(44, 90, 154);
            entityTab.Margin = new Padding(6, 1, 0, 2);
            entityTab.Size = new Size(73, 23);
            entityTab.Text = entity.Name;
            entityTab.TabStripPage = entityPage;

            if (entity is IInternalConfigurator)
                entityTab.Image = VisualResources.Icon_16x16_Hyphen.ToBitmap();

            RibbonStrip.Items.Add(entityTab);

            return entityPage;
        }

        #endregion

        #region Entity control handling

        private bool CanDismissActiveControl()
        {
            if (ControlPanel.Controls.Count > 0 && ControlPanel.Controls[0] is CategoryItemControl)
                return !((CategoryItemControl)ControlPanel.Controls[0]).OnHide();
            else
                return true;
        }

        private void RibbonButton_Click(object sender, EventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            CategoryItem item = (CategoryItem)btn.Tag;

            bool firstTime = !item.ControlInitialized;
            CategoryItemControl control = item.Control;

            control.OnSelected();

            if (control.HasUI && !control.OnShow(firstTime))
            {
                if (!CanDismissActiveControl())
                    return;

                control.ConfigurationParameter = ConfigurationParameter;
                ControlPanel.Controls.Clear();

                control.Dock = DockStyle.Fill;
                ControlPanel.Controls.Add(control);
            }
        }

        #endregion

        #endregion
    }
}