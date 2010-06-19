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
using System.Runtime.CompilerServices;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Miranda.Plugins.Resources;
using System.Diagnostics;
using Virtuoso.Hyphen.Mini;
using System.Windows.Forms;

namespace Virtuoso.Miranda.Plugins
{
    internal sealed class DefaultExceptionHandler : IExceptionHandler, IExceptionReporter
    {
        #region Properties

        private MirandaPlugin plugin;
        public MirandaPlugin Plugin
        {
            get { return plugin; }
        }

        #endregion

        #region .ctors

        private DefaultExceptionHandler(MirandaPlugin plugin)
        {
            this.plugin = plugin;
        }

        public static DefaultExceptionHandler Create()
        {
            return Create(null);
        }

        public static DefaultExceptionHandler Create(MirandaPlugin plugin)
        {
            return new DefaultExceptionHandler(plugin);
        }

        #endregion

        #region Methods

        public void HandleException(Exception e, PluginDescriptor descriptor)
        {
            IExceptionReporter reporter = plugin as IExceptionReporter ?? (plugin == null ? (IExceptionReporter)null : (IExceptionReporter)this);
            ErrorDialog.PresentModal(e, reporter, String.Format(TextResources.MsgBox_Formatable1_Text_ModuleError, plugin), false);
        }

        void IExceptionReporter.ReportException(Exception e)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;

                if (plugin is StandalonePlugin)
                {
                    startInfo.FileName = String.Format("mailto:{0}?subject={1}&body={2}", ((StandalonePlugin)plugin).AuthorEmail, String.Format(TextResources.UI_Formatable1_Text_PluginCrashed, plugin), e.ToString());
                }
                else
                {
                    startInfo.FileName = plugin.HomePage.ToString();
                }

                Process.Start(startInfo);
            }
            catch
            {
                MessageBox.Show(TextResources.MsgBox_Text_UnableToReportError, TextResources.MsgBox_Caption_Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion
    }
}
