/***********************************************************************\
 * Virtuoso.Miranda.Plugins (Hyphen)                                   *
 * Provides a managed wrapper for API of IM client Miranda.            *
 * Copyright (C) 2006-2008 virtuoso                                    *
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
using System.Reflection;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Miranda.Plugins.Resources;
using System.Diagnostics;

namespace Virtuoso.Miranda.Plugins
{
    partial class MirandaPlugin
    {
        /// <summary>
        /// Represents an unknown plugin. Hyphen will impersonate itself with this plugin when binding to Miranda's events.
        /// </summary>
        internal sealed class Hyphen : MirandaPlugin, IExceptionHandler, IExceptionReporter
        {
            #region Fields

            private static readonly Hyphen singleton = new Hyphen();

            #endregion

            #region .ctors

            private Hyphen()
            {
                PluginDescriptor.SetUp(this);
            }

            #endregion

            #region Properties

            public static Hyphen Singleton
            {
                get
                {
                    return singleton;
                }
            }

            public override string Name
            {
                get { return "Hyphen"; }
            }

            public override string Author
            {
                get { return "virtuoso"; }
            }

            public override string Description
            {
                get { return String.Empty; }
            }

            public override Uri HomePage
            {
                get { return new Uri("http://www.none.com"); }
            }

            public override Version Version
            {
                get { return Assembly.GetExecutingAssembly().GetName().Version; }
            }

            public override bool HasOptions
            {
                get { return false; }
            }

            #endregion

            #region IExceptionHandler Members

            public void HandleException(Exception e, PluginDescriptor descriptor)
            {
                ErrorDialog.PresentModal(e, this, TextResources.MsgBox_Text_HyphenCrashed, false);
            }

            #endregion

            #region IExceptionReporter Members

            void IExceptionReporter.ReportException(Exception e)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(String.Format("mailto:{0}?subject={1}&body={2}", "deml.tomas@seznam.cz", TextResources.MsgBox_Caption_HyphenCrashed, e.ToString()));
                startInfo.UseShellExecute = true;

                Process.Start(startInfo);
            }

            #endregion
        }
    }
}
