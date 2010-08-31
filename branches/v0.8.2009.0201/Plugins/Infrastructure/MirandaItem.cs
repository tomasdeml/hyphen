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

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public enum ItemType : int
    {
        Unspecified,
        Contact,
        Group
    }

    public class MirandaItem : MirandaObject
    {
        #region Fields

        private static readonly Type ItemTypeType = typeof(ItemType);

        private ItemType type;

        #endregion

        #region .ctors

        protected MirandaItem(IntPtr handle, ItemType type) : this(handle)
        {
            if (!Enum.IsDefined(ItemTypeType, type)) 
                throw new ArgumentOutOfRangeException("type");

            if (type == ItemType.Unspecified) 
                throw new ArgumentException("Unspecified type is not supported.");

            this.type = type;
        }

        internal MirandaItem(IntPtr handle)
        {
            this.MirandaHandle = handle;
        }

        #endregion

        #region Properties

        public ItemType Type
        {
            get
            {
                return this.type;
            }
        }

        #endregion
    }
}
