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
using System.Threading;
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Forms
{
    internal sealed partial class FusionProgressDialog : Form
    {
        #region Delegates

        public delegate void WorkerDelegate();

        #endregion

        #region Fields

        private WorkerDelegate Worker;

        #endregion

        #region .ctors

        public FusionProgressDialog(WorkerDelegate del)
        {
            InitializeComponent();

            Worker = del;
            Shown += FusionProgressDialog_Shown;
        }

        public static void ShowDialog(WorkerDelegate del)
        {
            if (del == null)
                throw new ArgumentNullException("del");

            PluginDialog.ExecuteOnSTAThread(delegate(object delegateObj)
            {
                using (FusionProgressDialog dlg = new FusionProgressDialog((WorkerDelegate)delegateObj))
                    dlg.ShowDialog();
            }, del);
        }

        #endregion

        #region UI Handlers

        private void FusionProgressDialog_Shown(object sender, EventArgs e)
        {
            FusionWorker.RunWorkerAsync();
        }

        private void FusionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Worker != null)
                Worker();
        }

        private void FusionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}