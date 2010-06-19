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

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public sealed class ContactListEvent
    {
        #region Native fields

        private readonly int Size;
        internal IntPtr ContactHandle;
        internal IntPtr IconHandle;
        internal UInt32 Flags;
        internal IntPtr EventHandle;
        internal IntPtr lParam;
        internal string ServiceName;
        internal string Tooltip;

        #endregion

        #region .ctors

        internal ContactListEvent()
        {
            Size = Marshal.SizeOf(this);
        }

        public static ContactListEvent FromPointer(IntPtr pClistEvent)
        {
            if (pClistEvent == IntPtr.Zero)
                throw new ArgumentNullException("pClistEventHandle");

            return (ContactListEvent)Marshal.PtrToStructure(pClistEvent, typeof(ContactListEvent));
        }

        #endregion

        #region Managed properties
                
        public ContactInfo Contact
        {
            get { return ContactInfo.FromHandle(ContactHandle); }
        }

        public IntPtr LParam
        {
            get { return lParam; }
            internal set { lParam = value; }
        }

        #endregion
    }

    //    typedef struct {
    //    int cbSize;          //size in bytes of this structure
    //    HANDLE hContact;	 //handle to the contact to put the icon by
    //    HICON hIcon;		 //icon to flash
    //    DWORD flags;		 //...of course	
    //    union
    //    {
    //        HANDLE hDbEvent;	 //caller defined but should be unique for hContact
    //        char * lpszProtocol;
    //    };
    //    LPARAM lParam;		 //caller defined
    //    char *pszService;	 //name of the service to call on activation
    //    union {
    //        char  *pszTooltip;    //short description of the event to display as a
    //        TCHAR *ptszTooltip;    //tooltip on the system tray
    //    };
    //} CLISTEVENT;
}
