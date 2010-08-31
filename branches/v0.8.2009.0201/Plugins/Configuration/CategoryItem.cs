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
using Virtuoso.Miranda.Plugins.Configuration.Forms.Controls;
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Miranda.Plugins.Configuration
{
    [Serializable]
    public sealed class CategoryItem
    {
        #region Delegates

        public delegate void Command(CategoryItem item);

        #endregion

        #region Fields

        private string name, description;        
        private CategoryItemControl control;     
        private Type controlType;
        private Image image;

        private bool isExpertOption;

        #endregion

        #region .ctors

        public CategoryItem(string name, string description, Command command) 
            : this(name, description)
        {
            if (command == null) 
                throw new ArgumentNullException("command");

            control = new CategoryItemCommandControl(this, command);
        }

        public CategoryItem(string name, string description, Type itemOptionsType) 
            : this(name, description)
        {
            if (itemOptionsType == null)
                throw new ArgumentNullException("itemOptionsType");

            if (!itemOptionsType.IsSubclassOf(typeof(CategoryItemControl)))
                throw new ArgumentException("Type must derive from the CategoryItemOptionsControl class.", "itemOptionsType");

            this.controlType = itemOptionsType;            
        }

        private CategoryItem(string name, string description)
        {
            if (String.IsNullOrEmpty(name)) 
                throw new ArgumentNullException("name");

            if (String.IsNullOrEmpty(description)) 
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }              

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }        

        internal CategoryItemControl Control
        {
            get
            {
                if (control == null)
                {
                    control = (CategoryItemControl)Activator.CreateInstance(controlType, true);
                    control.Initialize(this);
                }

                return control;
            }
        }

        internal bool ControlInitialized
        {
            get
            {
                return control != null;
            }
        }

        public bool IsExpertOption
        {
            get { return isExpertOption; }
            set { isExpertOption = value; }
        }	

        #endregion
    }
}
