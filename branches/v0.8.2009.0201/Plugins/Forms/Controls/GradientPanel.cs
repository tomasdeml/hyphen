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
using System.Drawing.Drawing2D;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    [ToolboxBitmap(typeof(Panel))]
    public sealed class GradientPanel : Panel
    {
        public GradientPanel() { }

        private Color gradientColor;
        public Color GradientColor
        {
            get { return gradientColor; }
            set { gradientColor = value; Refresh(); }
        }

        private float rotation;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (e.ClipRectangle.IsEmpty) return;

            using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, BackColor, GradientColor, Rotation))
                e.Graphics.FillRectangle(brush, ClientRectangle);
            
            base.OnPaint(e);
        }
    }
}
