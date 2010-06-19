﻿/***********************************************************************\
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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr MMI_MallocPrototype(int size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr MMI_ReallocPrototype(IntPtr ptr, int size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr MMI_FreePrototype(IntPtr ptr);

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct MM_INTERFACE
    {
        public int Size;
        public readonly MMI_MallocPrototype Malloc;
        public readonly MMI_ReallocPrototype Realloc;
        public readonly MMI_FreePrototype Free;
    }

    /* m_system.h
     * struct MM_INTERFACE
        {
	        int cbSize;
	        void* (*mmi_malloc) (size_t);
	        void* (*mmi_realloc) (void*, size_t);
	        void  (*mmi_free) (void*);
        }
    */
}