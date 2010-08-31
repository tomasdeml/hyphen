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
    internal struct CCSDATA
    {
        #region Fields

        public readonly IntPtr ContactHandle;
        public IntPtr ServiceNamePtr;

        public UIntPtr WParam;
        public IntPtr LParam;

        #endregion

        #region .ctors

        public CCSDATA(ContactInfo contact, string serviceName)
        {
            this.ContactHandle = contact.MirandaHandle;
            this.ServiceNamePtr = new UnmanagedStringHandle(serviceName, StringEncoding.Ansi).IntPtr;

            this.WParam = UIntPtr.Zero;
            this.LParam = IntPtr.Zero;
        }

        public void Free()
        {
            if (ServiceNamePtr != IntPtr.Zero)
                Marshal.FreeHGlobal(ServiceNamePtr);
        }

        #endregion
    }
}
