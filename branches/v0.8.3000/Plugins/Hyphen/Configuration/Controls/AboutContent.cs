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
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Configuration.Forms.Controls;
using System.Diagnostics;
using System.Reflection;

namespace Virtuoso.Hyphen.Configuration.Controls
{
    internal sealed partial class AboutContent : CategoryItemControl
    {
        private AboutContent()
        {
            InitializeComponent();
        }

        protected internal override bool OnShow(bool firstTime)
        {
            if (firstTime)
            {
                Assembly assembly = GetType().Assembly;

                VersionLABEL.Text = String.Format("v{0}", assembly.GetName().Version);
                HomepageLINK.Text =
                    ((AssemblyCopyrightAttribute)
                     assembly.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false)[0]).Copyright;
            }

            return false;
        }

        private void HomepageLINK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo(Loader.GetInstance().PluginInfo.HomePage);
            info.UseShellExecute = true;

            Process.Start(info);
        }
    }
}
