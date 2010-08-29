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

namespace Virtuoso.Miranda.Plugins.Configuration.Forms.Controls
{
    internal sealed class CategoryItemCommandControl : CategoryItemControl
    {
        #region Fields

        private CategoryItem.Command Command;

        #endregion

        #region .ctors

        public CategoryItemCommandControl(CategoryItem item, CategoryItem.Command command)
        {
            if (command == null) 
                throw new ArgumentNullException("command");

            this.Command = command;
        }

        #endregion

        #region Overrides

        protected internal override void OnSelected()
        {
            Command(ParentItem);
        }

        protected internal override bool HasUI
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
