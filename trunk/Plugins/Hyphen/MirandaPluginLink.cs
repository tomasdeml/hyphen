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
using Virtuoso.Hyphen.Native;
using System.Runtime.InteropServices;

namespace Virtuoso.Hyphen
{
    internal sealed class MirandaPluginLink
    {
        #region Fields

        private readonly NativePluginLink nativePluginLink;

        private readonly IntPtr nativePluginLinkPtr;
        public readonly CallServiceUnsafePrototype CallServiceUnsafe;

        #endregion

        #region .ctors

        private MirandaPluginLink(IntPtr nativeLinkPtr)
        {
            if (nativeLinkPtr == IntPtr.Zero)
                throw new ArgumentNullException("nativeLinkPtr");

            this.nativePluginLinkPtr = nativeLinkPtr;
            this.nativePluginLink = (NativePluginLink)Marshal.PtrToStructure(nativeLinkPtr, typeof(NativePluginLink));

            this.CallServiceUnsafe = (CallServiceUnsafePrototype)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(nativeLinkPtr, 9 * IntPtr.Size), typeof(CallServiceUnsafePrototype));
        }

        public static MirandaPluginLink FromPointer(IntPtr nativeLinkPtr)
        {
            return new MirandaPluginLink(nativeLinkPtr);
        }

        #endregion

        #region Properties

        public IntPtr NativePluginLinkPtr
        {
            get
            {
                return nativePluginLinkPtr;
            }
        }

        public NativePluginLink NativePluginLink
        {
            get
            {
                return nativePluginLink;
            }
        }

        #endregion
    }
}
