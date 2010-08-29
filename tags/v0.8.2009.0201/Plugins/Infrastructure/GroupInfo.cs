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
using Virtuoso.Miranda.Plugins.Native;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public sealed class GroupInfo : MirandaItem
    {
        #region Fields

        private const string MS_CLIST_GROUPGETNAME = "CList/GroupGetName";

        #endregion

        #region .ctors

        internal GroupInfo(IntPtr handle) : base(handle, ItemType.Group) { }

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                IntPtr pName = (IntPtr)MirandaContext.Current.CallService(MS_CLIST_GROUPGETNAME, MirandaHandle, IntPtr.Zero);
                Debug.Assert(pName != IntPtr.Zero);

                if (pName != IntPtr.Zero)
                    return Marshal.PtrToStringAnsi(pName);
                else
                    return null;
            }
        }

        public unsafe bool IsExpanded
        {
            get
            {
                int expanded = 0;

                IntPtr pName = (IntPtr)MirandaContext.Current.CallServiceUnsafe(MS_CLIST_GROUPGETNAME, MirandaHandle.ToPointer(), &expanded);
                Debug.Assert(pName != IntPtr.Zero);

                if (pName != IntPtr.Zero)
                    return Convert.ToBoolean(expanded);
                else
                    return false;
            }
        }

        #endregion
    }
}
