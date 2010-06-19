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
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public static class ProtocolStatus
    {
        #region Fields

        private const string MS_AWAYMSG_SHOWAWAYMSG = "SRAway/GetMessage", 
                             MS_AWAYMSG_GETSTATUSMSG = "SRAway/GetStatusMessage";

        #endregion

        #region Methods

        public static bool ShowAwayMessage(ContactInfo contact)
        {
            int result = MirandaContext.Current.CallService(MS_AWAYMSG_SHOWAWAYMSG, contact.MirandaHandle, IntPtr.Zero);
            Debug.Assert(result == 0);

            return result == 0;
        }

        public static string GetStatusMessage(StatusMode status)
        {
            if (!Enum.IsDefined(typeof(StatusMode), status))
                throw new ArgumentOutOfRangeException("status");

            IntPtr statusPtr = IntPtr.Zero;

            try
            {
                statusPtr = (IntPtr)MirandaContext.Current.CallService(MS_AWAYMSG_GETSTATUSMSG, (UIntPtr)(ulong)status, IntPtr.Zero);
                if (statusPtr == IntPtr.Zero) return null;

                return Translate.ToString(statusPtr, Virtuoso.Miranda.Plugins.Native.StringEncoding.Ansi);
            }
            catch (Exception e)
            {
                throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_AWAYMSG_GETSTATUSMSG, 0), e);
            }
            finally
            {
                if (statusPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(statusPtr);
            }
        }

        #endregion
    }
}
