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
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Miranda.Plugins.Forms
{
    public class SingletonDialog : PluginDialog
    {
        #region Fields

        private static readonly Dictionary<string, SingletonDialog> visibleDialogs = new Dictionary<string, SingletonDialog>(1);
        protected static Dictionary<string, SingletonDialog> VisibleDialogs
        {
            get { return visibleDialogs; }
        }

        private string singletonName;
        protected string SingletonName
        {
            get { return singletonName; }
            private set { singletonName = value; }
        }

        #endregion

        #region .ctors

        protected SingletonDialog() : this(null) { }

        protected SingletonDialog(string name)
        {
            this.singletonName = String.IsNullOrEmpty(name) ? GetDefaultName(GetType()) : name;
        }

        #endregion

        #region Overrides

        internal override void RegisterDialog()
        {
            lock (visibleDialogs)
            {
                if (!visibleDialogs.ContainsKey(SingletonName))
                    visibleDialogs[SingletonName] = this;
            }

            base.RegisterDialog();
        }

        internal override void UnregisterDialog()
        {
            lock (visibleDialogs)
                visibleDialogs.Remove(SingletonName);

            base.UnregisterDialog();
        }

        #endregion

        #region Methods

        public static TForm GetSingleton<TForm>(bool createIfNeeded) where TForm : PluginDialog
        {
            return GetSingleton<TForm>(createIfNeeded, typeof(TForm).FullName);
        }

        public static TForm GetSingleton<TForm>(bool createIfNeeded, string name) where TForm : PluginDialog
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            lock (visibleDialogs)
            {
                if (!visibleDialogs.ContainsKey(name) || visibleDialogs[name].IsDisposed)
                {
                    if (createIfNeeded)
                        return (TForm)Activator.CreateInstance(typeof(TForm), true);
                    else
                        return null;
                }
                else
                    return visibleDialogs[name] as TForm;
            }
        }

        private delegate void ShowSingletonInvoker(bool modal);

        public void ShowSingleton(bool modal)
        {
            if (InvokeRequired)
                Invoke(new ShowSingletonInvoker(DoShowSingleton), modal);
            else
                DoShowSingleton(modal);
        }

        private void DoShowSingleton(bool modal)
        {
            if (Visible)
                Activate();
            else if (modal)
                ShowDialog();
            else
                Show();
        }

        public static string GetDefaultName(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.FullName;
        }

        #endregion
    }
}
