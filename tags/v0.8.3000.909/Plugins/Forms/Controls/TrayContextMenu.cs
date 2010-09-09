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
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Configuration.Forms;
using Virtuoso.Miranda.Plugins.Configuration;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Hyphen;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    internal sealed class TrayContextMenu : ContextMenuStrip
    {
        #region Fields

        private ToolStripMenuItem ManagePluginsITEM;

        #endregion

        #region .ctors

        public TrayContextMenu()
        {
            InitializeComponent();
        }

        #endregion

        #region Designer

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayContextMenu));
            this.ManagePluginsITEM = new System.Windows.Forms.ToolStripMenuItem();
            this.SuspendLayout();
            // 
            // ManagePluginsITEM
            // 
            this.ManagePluginsITEM.Image = ((System.Drawing.Image)(resources.GetObject("ManagePluginsITEM.Image")));
            this.ManagePluginsITEM.Name = "ManagePluginsITEM";
            this.ManagePluginsITEM.Size = new System.Drawing.Size(111, 22);
            this.ManagePluginsITEM.Text = "Options";
            this.ManagePluginsITEM.Click += new System.EventHandler(this.ManagePluginsITEM_Click);
            // 
            // TrayContextMenu
            // 
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ManagePluginsITEM});
            this.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.Size = new System.Drawing.Size(112, 26);
            this.ResumeLayout(false);

        }

        #endregion

        #region UI Handlers

        private void ManagePluginsITEM_Click(object sender, EventArgs e)
        {
            Loader.GetInstance().ManagePlugins();
        }

        #endregion
    }
}
