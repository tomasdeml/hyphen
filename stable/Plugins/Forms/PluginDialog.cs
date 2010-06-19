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
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Virtuoso.Miranda.Plugins.Forms
{
    public class PluginDialog : RemotableForm
    {
        #region Fields

        private static readonly List<PluginDialog> ActiveDialogs = new List<PluginDialog>();

        #endregion

        #region .ctors

        protected PluginDialog() { }

        #endregion

        #region Overrides

        protected override void OnShown(EventArgs e)
        {
            RegisterDialog();
            base.OnShown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            UnregisterDialog();
            base.OnClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            UnregisterDialog();
            base.Dispose(disposing);
        }

        #endregion

        #region Properties

        protected MirandaContext Context
        {
            get { return MirandaContext.Current; }
        }

        #endregion

        #region Methods

        internal virtual void RegisterDialog()
        {
            lock (ActiveDialogs)
                ActiveDialogs.Add(this);
        }

        /// <summary>
        /// Remove the dialog from the active dialog tracking list.
        /// </summary>
        internal virtual void UnregisterDialog()
        {
            lock (ActiveDialogs)
                ActiveDialogs.Remove(this);
        }

        public static void CloseDialogs(PluginDescriptor owner, bool force)
        {
            foreach (PluginDialog dialog in UnregisterAndGetActiveDialogs(owner))
            {
                try
                {
                    if (dialog.InvokeRequired)
                        dialog.Invoke(new MethodInvoker(delegate { dialog.Dispose(); }));
                    else
                        dialog.Dispose();
                }
                catch { if (!force) throw; }
            }
        }

        /// <summary>
        /// Gathers active dialogs of the plugin and unregisters them.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        /// <returns>Unregistered dialogs to dispose.</returns>
        private static List<PluginDialog> UnregisterAndGetActiveDialogs(PluginDescriptor plugin)
        {
            Assembly pluginAssembly = plugin.Plugin.GetType().Assembly;
            List<PluginDialog> dialogsToRemove = new List<PluginDialog>(2);

            lock (ActiveDialogs)
            {
                foreach (PluginDialog dialog in ActiveDialogs)
                {
                    // Account only undisposed dialogs from plugin's assembly
                    if (dialog.IsDisposed || dialog.GetType().Assembly != pluginAssembly)
                        continue;

                    dialogsToRemove.Add(dialog);
                }

                foreach (PluginDialog dialog in dialogsToRemove)
                    dialog.UnregisterDialog();
            }

            return dialogsToRemove;
        }

        public static void ExecuteOnSTAThread(ParameterizedThreadStart threadStart)
        {
            ExecuteOnSTAThread(threadStart, null);
        }

        public static void ExecuteOnSTAThread(ParameterizedThreadStart threadStart, object state)
        {
            if (threadStart == null)
                throw new ArgumentNullException("threadStart");

            Thread thread = new Thread(delegate(object _state)
                {
                    try
                    {
                        Application.ThreadException += Application_ThreadException;
                        threadStart(_state);
                    }
                    finally
                    {
                        Application.ThreadException -= Application_ThreadException;
                    }
                });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;

            thread.Start(state);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            DefaultExceptionHandler.Create().HandleException(e.Exception, null);
        }

        #endregion
    }
}
