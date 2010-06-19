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
using System.Drawing;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    internal class ManagedMainMenuItem : ToolStripMenuItem
    {
        #region Fields

        private static readonly Random HandleGenerator = new Random();

        private int handle;
        private string service, popUpMenu;

        #endregion

        #region .ctors

        public ManagedMainMenuItem(string text, string popUpMenu, string service, Image image)
        {
            if (String.IsNullOrEmpty(text)) throw new ArgumentNullException("text");
            if (String.IsNullOrEmpty(service)) throw new ArgumentNullException("service");

            this.Name = (handle = HandleGenerator.Next()).ToString();

            this.Text = text;
            this.popUpMenu = popUpMenu;
            this.service = service;
            this.Image = image;

            this.Click += new EventHandler(ManagedMainMenuItem_Click);
        }

        #endregion

        #region Properties

        public string PopUpMenu
        {
            get { return popUpMenu; }
        }

        public string Service
        {
            get { return service; }
        }

        public int Handle
        {
            get { return handle; }
        }

        #endregion

        #region Methods

        private void ManagedMainMenuItem_Click(object sender, EventArgs e)
        {
            MirandaContext.Current.CallService(service);
        }

        #endregion
    }
}
