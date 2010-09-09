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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Security;

namespace Virtuoso.Miranda.Plugins.Configuration.Forms.Controls
{
    public partial class CategoryItemControl : UserControl
    {
        #region .ctors

        protected CategoryItemControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CategoryItemControl
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "CategoryItemControl";
            this.Size = new System.Drawing.Size(792, 400);
            this.ResumeLayout(false);

        }

        #endregion

        #region Properties

        private bool dirty;
        protected internal bool IsDirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        private CategoryItem parentItem;
        internal CategoryItem ParentItem
        {
            get { return parentItem; }
        }

        private string configurationParameter;
        protected internal string ConfigurationParameter
        {
            get { return configurationParameter; }
            internal set { configurationParameter = value; }
        }
        
        #endregion

        #region Virtuals

        /// <summary>
        /// Gets a value indicating whether the control has an UI to show.
        /// </summary>
        protected internal virtual bool HasUI { get { return true; } }

        /// <summary>
        /// Occurs when a control is selected and before it is prepared to be shown.
        /// </summary>
        protected internal virtual void OnSelected() { }

        /// <summary>
        /// Occurs when a control is about to be shown. The control is shown when the user clicks on its item.
        /// </summary>
        /// <param name="firstTime">TRUE if the control is being requested for the first time; FALSE if it is requested repeatedly.</param>
        /// <returns>TRUE to cancel the display; FALSE to continue.</returns>
        protected internal virtual bool OnShow(bool firstTime) { return false; }

        /// <summary>
        /// Occurs when a control is about to be hidden. The control is hidden when the user clicks on another item.
        /// </summary>        
        /// <returns>TRUE to cancel the dismissal; FALSE to continue.</returns>
        protected internal virtual bool OnHide() { return false; }

        /// <summary>
        /// Occurs when the user dismisses the configuration dialog via OK button and the control is dirty. The control should save its settings now.
        /// </summary>
        protected internal virtual void Save() { }

        /// <summary>
        /// Occurs when the user dismisses the configuration dialog via OK or CANCEL button.
        /// </summary>
        protected internal virtual void Close() { }

        /// <summary>
        /// Marks the control dirty.
        /// </summary>
        protected virtual void SetControlDirtyHandler(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        #endregion

        #region Methods

        internal void Initialize(CategoryItem parentItem)
        {
            this.parentItem = parentItem;
        }

        protected void CloseDialog()
        {
            ParentForm.Close();
        }

        #endregion
    }
}
