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
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Miranda.Plugins.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct CONTACTINFO
    {
        #region Fields

        private readonly int Size;

        public byte Flag;
        public IntPtr ContactHandle;
        public IntPtr Module;
        public byte Type;

        public IntPtr Value;

        #endregion

        #region .ctors

        public CONTACTINFO(IntPtr contactHandle, IntPtr module)
        {
            this.ContactHandle = contactHandle;
            this.Flag = 0;
            this.Module = module;
            this.Type = 0;
            this.Value = IntPtr.Zero;
            this.Size = Marshal.SizeOf(typeof(CONTACTINFO));
        }

        #endregion
    }

    /*typedef struct {
	int cbSize;
	BYTE dwFlag;
	HANDLE hContact;
	char *szProto;
	BYTE type;
	union {
		BYTE bVal;
		WORD wVal;
		DWORD dVal;
		char *pszVal;
		WORD cchVal;
	};
} CONTACTINFO;*/
}
