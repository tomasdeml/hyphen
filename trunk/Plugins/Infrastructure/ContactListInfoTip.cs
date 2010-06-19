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
using System.Runtime.InteropServices;
using System.Drawing;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ContactListInfoTip
    {
        #region Native fields

        private int size, isTreeFocused, isGroup;
        private IntPtr itemPtr;

        private Point point;
        private Rectangle rectangle;

        #endregion

        #region Properties

        public bool IsTreeFocused
        {
            get { return Convert.ToBoolean(isTreeFocused); }
        }

        public bool IsGroup
        {
            get { return Convert.ToBoolean(isGroup); }
        }

        public MirandaItem Item
        {
            get
            {
                return IsGroup ? (MirandaItem)new GroupInfo(itemPtr) : (MirandaItem)ContactInfo.FromHandle(itemPtr);
            }
        }

        public Point Point
        {
            get { return point; }
        }

        public Rectangle Rectangle
        {
            get { return rectangle; }
        }

        #endregion
    }

    /*
     * typedef struct {
	    int cbSize;
	    int isTreeFocused;   //so the plugin can provide an option
	    int isGroup;     //0 if it's a contact, 1 if it's a group
	    HANDLE hItem;	 //handle to group or contact
	    POINT ptCursor;
	    RECT rcItem;
    } CLCINFOTIP;*/
}
