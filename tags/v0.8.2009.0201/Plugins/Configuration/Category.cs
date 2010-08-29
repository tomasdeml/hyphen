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
using System.Drawing;
using System.Windows.Forms;
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Miranda.Plugins.Configuration
{
    [Serializable]
    public sealed class Category
    {
        #region Fields
        
        private string name, description;
        private CategoryItemCollection items;                

        #endregion 

        #region .ctors

        public Category(string name, string description)
        {
            if (String.IsNullOrEmpty(name)) 
                throw new ArgumentNullException("name");

            if (String.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");

            this.items = new CategoryItemCollection();
            this.name = name;
            this.description = description;
        }

        #endregion

        #region Properties

        public string Description
        {
            get { return description; }
        }

        public string Name
        {
            get { return name; }
        }

        public CategoryItemCollection Items
        {
            get { return items; }
        }

        #endregion
    }
}
