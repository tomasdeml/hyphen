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
using System.Drawing.Design;

namespace Virtuoso.Miranda.Plugins.Configuration.Forms.Controls
{
    public partial class CategoryItemHeader : UserControl
    {
        public CategoryItemHeader()
        {
            InitializeComponent();
        }

        [Browsable(true), Category("Appearance")]
        public Image Image
        {
            get { return ImagePBOX.Image; }
            set { ImagePBOX.Image = value; }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true), Browsable(true), Category("Appearance"), DefaultValue("Header")]
        public string HeaderText
        {
            get { return DescriptionLABEL.Text; }
            set { DescriptionLABEL.Text = value; }
        }

        [Browsable(true), Category("Appearance")]
        public Color Color
        {
            get { return panel1.GradientColor; }
            set { panel1.GradientColor = value; }
        }

        public override Color ForeColor
        {
            get
            {
                return DescriptionLABEL.ForeColor;
            }
            set
            {
                DescriptionLABEL.ForeColor = value;
            }
        }

        [Browsable(true), Category("Appearance")]
        public Font HeaderFont
        {
            get { return DescriptionLABEL.Font; }
            set { DescriptionLABEL.Font = value; }
        }

    }
}
