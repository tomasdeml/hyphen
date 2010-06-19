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
using System.Drawing;
using System.Drawing.Imaging;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    public class CommandButton : Button
    {
        public CommandButton()
        {
            FlatStyle = FlatStyle.Standard;
            ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            TextAlign = ContentAlignment.MiddleLeft;
            TextImageRelation = TextImageRelation.ImageBeforeText;
            FlatAppearance.BorderSize = 3;
            FlatAppearance.BorderColor = SystemColors.GradientActiveCaption;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CommandButton
            // 
            this.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.FlatAppearance.BorderSize = 2;
            this.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ButtonShadow;
            this.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResumeLayout(false);

        }
    }
}
