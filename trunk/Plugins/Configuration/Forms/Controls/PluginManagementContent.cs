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
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Configuration.Forms.Controls
{
    internal sealed partial class PluginManagementContent : CategoryItemControl
    {
        #region .ctors

        private PluginManagementContent()
        {
            InitializeComponent();

            PluginsLVIEW.Hide();
            DescriptionLABEL.Text = TextResources.UI_Label_SelectPluginToDisplayDescr;

            MirandaContext.Current.PluginManager.PluginStateChange += PluginManager_PluginStateChange;
        }

        private void PluginManager_PluginStateChange(object sender, PluginStateChangeEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(delegate { OnShow(true); }));
            else
                OnShow(true);
        }

        #endregion

        #region UI Handlers

        protected internal override void Close()
        {
            MirandaContext.Current.PluginManager.PluginStateChange -= PluginManager_PluginStateChange;
        }

        protected internal override bool OnShow(bool firstTime)
        {
            if (!firstTime)
                return false;

            PluginDescriptorReadOnlyCollection plugins = MirandaContext.Current.PluginManager.Plugins;

            try
            {
                SynchronizationHelper.BeginCollectionUpdate(plugins);

                PluginsLVIEW.Items.Clear();
                DisablePluginBTN.Enabled = EnablePluginBTN.Enabled = HomePageLBTN.Enabled = false;

                if (plugins.Count == 0)
                {
                    PluginsLVIEW.Enabled = false;
                    PluginsLVIEW.Visible = true;
                }
                else
                {
                    PublishPlugins(plugins);
                }
            }
            finally
            {
                SynchronizationHelper.EndUpdate(plugins);
            }

            CommitListViewChanges();
            return false;
        }

        private void CommitListViewChanges()
        {
            NameColumn.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            AuthorColumn.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            VersionColumn.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            StatusColumn.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);

            PluginsLVIEW.Show();
        }

        #endregion

        #region Management

        private void PublishPlugins(PluginDescriptorReadOnlyCollection plugins)
        {
            PluginsLVIEW.BeginUpdate();

            foreach (PluginDescriptor descriptor in plugins)
            {
                try
                {
                    ListViewItem item = new ListViewItem(new string[] { descriptor.Plugin.Name, descriptor.Plugin.Author,
                            descriptor.Plugin.Version.ToString(), descriptor.PluginState.ToString() });

                    switch (descriptor.PluginState)
                    {
                        case PluginState.Enabled:
                            item.Group = PluginsLVIEW.Groups["EnabledGroup"];
                            break;
                        case PluginState.Disabled:
                            item.Group = PluginsLVIEW.Groups["DisabledByUserGroup"];
                            break;
                        case PluginState.CrashDisabled:
                            item.Group = PluginsLVIEW.Groups["DisabledByCrashGroup"];
                            break;
                    }

                    item.Tag = descriptor;
                    PluginsLVIEW.Items.Add(item);
                }
                finally
                {
                    PluginsLVIEW.EndUpdate();
                }
            }
        }

        private void PluginsLVIEW_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                PluginDescriptor descriptor = (PluginDescriptor)e.Item.Tag;
                PluginsLVIEW.Tag = descriptor;

                DescriptionLABEL.Text = descriptor.Plugin.Description;

                EnablePluginBTN.Enabled = !(descriptor.PluginState == PluginState.Enabled);
                DisablePluginBTN.Enabled = !EnablePluginBTN.Enabled;

                HomePageLBTN.Enabled = descriptor.Plugin.HomePage != null;
            }
            else
            {
                PluginsLVIEW.Tag = null;

                DisablePluginBTN.Enabled = EnablePluginBTN.Enabled = HomePageLBTN.Enabled = false;
                DescriptionLABEL.Text = TextResources.UI_Label_SelectPluginToDisplayDescr;
            }
        }

        private void DisablePluginBTN_Click(object sender, EventArgs e)
        {
            if (PluginsLVIEW.Tag is PluginDescriptor)
                ((PluginDescriptor)PluginsLVIEW.Tag).SetPluginState(PluginState.Disabled, true);
        }

        private void EnablePluginBTN_Click(object sender, EventArgs e)
        {
            if (PluginsLVIEW.Tag is PluginDescriptor)
                ((PluginDescriptor)PluginsLVIEW.Tag).SetPluginState(PluginState.Enabled, true);
        }

        private void HomePageLBTN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs eArgs)
        {
            try
            {
                if (PluginsLVIEW.Tag is PluginDescriptor)
                    Process.Start(((PluginDescriptor)PluginsLVIEW.Tag).Plugin.HomePage.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(TextResources.ExceptionMsg_CannotOpenHomePage + e.Message, TextResources.UI_Caption_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
