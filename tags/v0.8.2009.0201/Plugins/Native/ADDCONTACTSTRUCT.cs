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

namespace Virtuoso.Miranda.Plugins.Native
{
    [StructLayout( LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct ADDCONTACTSTRUCT
    {
        public int HandleType;
        public IntPtr Handle;
        public string Protocol;
    }

    /*
     * typedef struct{
	        int handleType;						//one of the HANDLE_ constants
	        HANDLE handle;						//hDbEvent if acs.handleType==HANDLE_EVENT, hContact if acs.handleType==HANDLE_CONTACT, ignored if acs.handleType==HANDLE_SEARCHRESULT
	        const char *szProto;				//ignored if acs.handleType!=HANDLE_SEARCHRESULT
	        PROTOSEARCHRESULT *psr;				//ignored if acs.handleType!=HANDLE_SEARCHRESULT
        }ADDCONTACTSTRUCT;
     */
}
