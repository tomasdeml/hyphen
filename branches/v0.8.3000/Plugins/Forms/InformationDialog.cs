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
using System.Media;

namespace Virtuoso.Miranda.Plugins.Forms
{
    public partial class InformationDialog : Form
    {
        public const string NewLineToken = "[n]";

        private InformationDialog()
        {
            InitializeComponent();
        }

        public static void PresentModal(string caption, string information, Image icon)
        {
            if (String.IsNullOrEmpty(caption)) 
                throw new ArgumentNullException("caption");

            if (String.IsNullOrEmpty(information)) 
                throw new ArgumentNullException("information");

            using (InformationDialog dlg = new InformationDialog())
            {
                dlg.DialogHeader.HeaderText = dlg.Text = caption;
                dlg.InformationLABEL.Text = information.Replace(NewLineToken, Environment.NewLine);
                
                if (icon != null)
                    dlg.DialogHeader.Image = icon;

                dlg.ShowDialog();
            }
        }

        private void InformationDialog_Shown(object sender, EventArgs e)
        {
            SystemSounds.Asterisk.Play();
        }
    }
}