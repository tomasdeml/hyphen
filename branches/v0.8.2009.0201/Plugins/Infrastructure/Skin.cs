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

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    /// <summary>
    /// Manages Miranda skin entities, for example icons and sounds.
    /// </summary>
    /// <mHeader>m_skin.h</mHeader>
    public static class Skin
    {
        #region Constants

        private const string MS_SKIN_LOADICON = "Skin/Icons/Load",
                             MS_SKIN_LOADPROTOICON = "Skin/Icons/LoadProto";

        #endregion

        #region Icons

        public static class Icons
        {
            public const int Message = 100;
        }

        #endregion

        #region Methods

        public static IntPtr LoadIcon(int id)
        {
            return (IntPtr)MirandaContext.Current.CallService(MS_SKIN_LOADICON, (UIntPtr)(uint)id, IntPtr.Zero);
        }

        public static IntPtr LoadProtocolIcon(StatusMode status)
        {
            return LoadProtocolIcon((string)null, status);
        }

        public static IntPtr LoadProtocolIcon(Protocol protocol, StatusMode status)
        {
            return LoadProtocolIcon((protocol != null ? protocol.Name : null), status);
        }

        public static IntPtr LoadProtocolIcon(string protocolName, StatusMode status)
        {
            UnmanagedStringHandle protoNamePtr = UnmanagedStringHandle.Empty;

            try
            {
                protoNamePtr = new UnmanagedStringHandle(protocolName, StringEncoding.Ansi);
                return (IntPtr)MirandaContext.Current.CallService(MS_SKIN_LOADPROTOICON, protoNamePtr.IntPtr, (IntPtr)status);
            }
            finally
            {
                protoNamePtr.Free();
            }
        }

        #endregion
    }
}
