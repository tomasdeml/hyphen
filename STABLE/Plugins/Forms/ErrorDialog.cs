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
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.Diagnostics;
using System.Media;

namespace Virtuoso.Miranda.Plugins.Forms
{
    internal sealed partial class ErrorDialog : Form
    {
        #region Fields

        private IExceptionReporter Reporter;
        private Exception Exception;

        #endregion

        #region .ctors

        private ErrorDialog()
        {
            InitializeComponent();
        }

        public static DialogResult PresentModal(Exception e)
        {
            return PresentModal(e, null, null, false);
        }

        public static DialogResult PresentModal(Exception e, IExceptionReporter reporter)
        {
            return PresentModal(e, reporter, null, false);
        }

        public static DialogResult PresentModal(Exception e, string message, bool canCancel)
        {
            return PresentModal(e, null, message, canCancel);
        }

        public static DialogResult PresentModal(Exception e, IExceptionReporter reporter, string message, bool canCancel)
        {
            using (ErrorDialog dlg = new ErrorDialog())
            {
                return dlg.BindAndShow(e, reporter, message, canCancel);
            }
        }

        #endregion

        #region Methods

        private DialogResult BindAndShow(Exception exception, IExceptionReporter reporter, string message, bool canCancel)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            this.Exception = exception;
            this.MessageLABEL.Text = message ?? exception.Message;
                       
            PrepareReportLink(exception, reporter);
            DumpException(exception);
            
            CancelBTN.Visible = canCancel;
            OkBTN.Focus();

            return ShowDialog();
        }

        private void PrepareReportLink(Exception exception, IExceptionReporter reporter)
        {
            if (reporter == null)
            {
                SendReportLBTN.Enabled = false;
            }
            else
            {
                this.Reporter = reporter;
            }
        }

        private void DumpException(Exception e)
        {
            StringBuilder dump = new StringBuilder();

            if (e is IExceptionDumpController)
            {
                ((IExceptionDumpController)e).DumpException(e, dump);
            }

            dump.AppendFormat("=== Exception dump ==={0}{1}{0}{0}", Environment.NewLine, e.ToString());
            DetailsTBOX.Text = dump.ToString();
        }

        #endregion

        #region UI Handlers

        private void PluginErrorDialog_Shown(object sender, EventArgs e)
        {
            SystemSounds.Hand.Play();
        }

        private void SendReportLBTN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Reporter.ReportException(Exception);
        }

        #endregion  
    }
}