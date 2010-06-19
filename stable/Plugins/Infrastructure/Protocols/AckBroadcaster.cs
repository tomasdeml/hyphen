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
using System.Threading;
using System.Security;

namespace Virtuoso.Miranda.Plugins.Infrastructure.Protocols
{
    [SuppressUnmanagedCodeSecurity]
    public static class AckBroadcaster
    {
        #region Constants

        private const string MS_PROTO_BROADCASTACK = "Proto/BroadcastAck";

        #endregion

        #region Helpers

        private static ACKDATA BuildAckData(IntPtr pModuleName, AckType type, bool success, IntPtr contactHandle, IntPtr processHandle, IntPtr lParam)
        {
            ACKDATA ack = new ACKDATA(pModuleName, (int)type, success ? (int)AckResult.Success : (int)AckResult.Failure);
            ack.ContactHandle = contactHandle;
            ack.ProcessHandle = processHandle;
            ack.LParam = lParam;

            return ack;
        }

        private static unsafe int BroadcastAck(ACKDATA* ack)
        {
            return MirandaContext.Current.CallServiceUnsafe(MS_PROTO_BROADCASTACK, null, ack);
        }

        #endregion

        #region Methods

        public static int BroadcastAck(string moduleName, AckType type, bool success, IntPtr contactHandle, IntPtr processHandle, IntPtr lParam)
        {
            UnmanagedStringHandle pModuleName = UnmanagedStringHandle.Empty;

            try
            {
                pModuleName = new UnmanagedStringHandle(moduleName, StringEncoding.Ansi);
                return BroadcastAck(pModuleName.IntPtr, type, success, contactHandle, processHandle, lParam);
            }
            finally
            {
                pModuleName.Free();
            }
        }

        public static unsafe void BroadcastMessageAckAsync(IntPtr pModuleName, bool success, IntPtr contactHandle, int processCookie)
        {
            ACKDATA ack = BuildAckData(pModuleName, AckType.Message, success, contactHandle, (IntPtr)processCookie, IntPtr.Zero);            
            ThreadPool.QueueUserWorkItem(delegate(object ackObject)
            {
                ACKDATA _ack = (ACKDATA)ackObject;
                BroadcastAck(&_ack);
            }, ack);
        }

        public static unsafe int BroadcastAck(IntPtr pModuleName, AckType type, bool success, IntPtr contactHandle, IntPtr processHandle, IntPtr lParam)
        {
            ACKDATA ack = BuildAckData(pModuleName, type, success, contactHandle, processHandle, lParam);
            return BroadcastAck(&ack);
        }

        #endregion
    }
}
