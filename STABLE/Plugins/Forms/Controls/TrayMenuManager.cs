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
using Virtuoso.Miranda.Plugins.Resources;
using System.ComponentModel;
using System.Drawing;
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    internal sealed class TrayMenuManager : IDisposable
    {
        #region Fields

        private readonly NotifyIcon TrayIcon;

        #endregion

        #region .ctors

        public TrayMenuManager()
        {
            TrayIcon = new NotifyIcon();

            TrayIcon.Text = TextResources.UI_ToolTip_HyphenTrayIcon;
            TrayIcon.Visible = true;
            TrayIcon.Icon = VisualResources.Icon_16x16_Hyphen;
            TrayIcon.ContextMenuStrip = new TrayContextMenu();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (TrayIcon != null)
                TrayIcon.Dispose();
        }

        #endregion
    }
}
