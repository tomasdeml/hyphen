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
    internal enum DbVariantValue : byte
    {
        DBVT_DELETED = 0,    //this setting just got deleted, no other values are valid
        DBVT_BYTE = 1,	  //bVal and cVal are valid
        DBVT_WORD = 2,	  //wVal and sVal are valid
        DBVT_DWORD = 4,  //dVal and lVal are valid
        DBVT_ASCIIZ = 255,	  //pszVal is valid
        DBVT_BLOB = 254,	  //cpbVal and pbVal are valid
        DBVT_UTF8 = 253,   //pszVal is valid
        DBVT_WCHAR = 252,   //pszVal is valid
        DBVTF_VARIABLELENGTH = 0x80
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 12)]
    internal struct DBVARIANT
    {
        [FieldOffset(0)]
        public byte Type;

        [FieldOffset(4)]
        public DBVARIANT_PRIMITIVE Primitives;

        [FieldOffset(4)]
        public DBVARIANT_TEXT Text;

        [FieldOffset(4)]
        public DBVARIANT_BLOB Blob;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    internal struct DBVARIANT_PRIMITIVE
    {
        [FieldOffset(0)]
        public byte Byte;

        [FieldOffset(0)]
        public char Char;

        [FieldOffset(0)]
        public UInt16 Word;

        [FieldOffset(0)]
        public short Short;

        [FieldOffset(0)]
        public UInt32 DWord;

        [FieldOffset(0)]
        public int Integer;
    }
    
    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    internal struct DBVARIANT_TEXT
    {
        [FieldOffset(0)]
        public IntPtr TextPtr;

        [FieldOffset(4)]
        public UInt16 TextBufferSize;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    internal struct DBVARIANT_BLOB
    {
        [FieldOffset(0)]
        public UInt16 Size;

        [FieldOffset(2)]
        public IntPtr BlobPtr;
    }

    //    typedef struct {
    //    BYTE type;
    //    union {
    //        BYTE bVal; char cVal;
    //        WORD wVal; short sVal;
    //        DWORD dVal; long lVal;
    //        struct {
    //            union {
    //                char *pszVal;
    //                TCHAR *ptszVal;
    //                WCHAR *pwszVal;
    //            };
    //            WORD cchVal;   //only used for db/contact/getsettingstatic
    //        };
    //        struct {
    //            WORD cpbVal;
    //            BYTE *pbVal;
    //        };
    //    };
    //} DBVARIANT;
}
