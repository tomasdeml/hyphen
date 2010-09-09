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
    internal static class UUID
    {
        #region Hyphen

        #region UUID

        private static readonly Guid hyphenUUID = new Guid("A9CB92EC-A8C9-493a-8763-77EB1DBA8228");
        public static Guid HyphenUUID
        {
            get { return UUID.hyphenUUID; }
        }

        #endregion

        #region Interfaces

        private static readonly Guid HyphenInterfaceUUID = new Guid("9E54961E-D2A2-4939-A23E-FF07F0A27D79");

        private static IntPtr hyphenInterfaceUUIDs;
        public static IntPtr HyphenInterfaceUUIDs
        {
            get
            {
                if (hyphenInterfaceUUIDs == IntPtr.Zero)
                {
                    int uuidSize = Marshal.SizeOf(typeof(Guid));
                    hyphenInterfaceUUIDs = Marshal.AllocHGlobal(2 * uuidSize);

                    byte[] uuidBytes = HyphenInterfaceUUID.ToByteArray();
                    Marshal.Copy(uuidBytes, 0, hyphenInterfaceUUIDs, uuidBytes.Length);

                    // MIID_LAST
                    uuidBytes = Last.ToByteArray();
                    Marshal.Copy(uuidBytes, 0, new IntPtr(hyphenInterfaceUUIDs.ToInt64() + uuidSize), uuidBytes.Length);
                }

                return hyphenInterfaceUUIDs;
            }
        }

        #endregion

        #endregion

        #region Miranda

        public static Guid Last
        {
            get
            {
                return Guid.Empty;
            }
        }

        private static Guid protocolUUID = new Guid(0x2a3c815e, 0xa7d9, 0x424b, 0xba, 0x30, 0x2, 0xd0, 0x83, 0x22, 0x90, 0x85);
        public static Guid ProtocolUUID
        {
            get { return protocolUUID; }
        }

        #endregion
    }
}
