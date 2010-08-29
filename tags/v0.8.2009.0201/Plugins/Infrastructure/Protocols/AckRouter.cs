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
using System.Runtime.CompilerServices;

namespace Virtuoso.Miranda.Plugins.Infrastructure.Protocols
{
    public static class AckRouter
    {
        #region Constants

        /// <summary>
        /// Call the next service in the chain for this send operation.
        ///   wParam=wParam
        ///   lParam=lParam
        /// The return value should be returned immediately
        /// wParam and lParam should be passed as the parameters that your service was
        /// called with. wParam must remain untouched but lParam is a CCSDATA structure
        /// that can be copied and modified if needed.
        /// Typically, the last line of any chaining protocol function is
        /// return CallService(MS_PROTO_CHAINSEND,wParam,lParam);
        /// </summary>
        private const string MS_PROTO_CHAINSEND = "Proto/ChainSend";

        /// <summary>
        /// Call the next service in the chain for this receive operation
        ///   wParam=wParam
        ///   lParam=lParam
        /// The return value should be returned immediately
        /// wParam and lParam should be passed as the parameters that your service was
        /// called with. wParam must remain untouched but lParam is a CCSDATA structure
        /// that can be copied and modified if needed.
        /// When being initiated by the network-access protocol module, wParam should be
        /// zero.
        /// Thread safety: ms_proto_chainrecv is completely thread safe since 0.1.2.0
        /// Calls to it are translated to the main thread and passed on from there. The
        /// function will not return until all callees have returned, irrepective of
        /// differences between threads the functions are in.
        /// </summary>
        private const string MS_PROTO_CHAINRECV = "Proto/ChainRecv";

        #endregion

        #region Fields

        private const string ME_PROTO_ACK = "Proto/Ack";
        private static MirandaEventHandler<AckEventArgs> AckReceivedEventHandler;

        #endregion

        #region Events

        public static event MirandaEventHandler<AckEventArgs> AckReceived
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<AckEventArgs>>(ref AckReceivedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_PROTO_ACK,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        AckEventArgs e = AckEventArgs.FromACKDATA(lParam);

                        EventPublisher.InvokeChainCancelable<AckEventArgs>(AckReceivedEventHandler, null, e);
                        return (int)CallbackResult.Success;
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<AckEventArgs>>(ref AckReceivedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_PROTO_ACK, AckReceivedEventHandler);
            }
        }

        #endregion

        #region Methods

        public static int ChainSend(ContactChainData chainData)
        {
            if (chainData == null)
                throw new ArgumentNullException("chainData");

            return ChainSend(chainData.WParam, chainData.CcsDataPtr);
        }

        public static int ChainSend(UIntPtr wParam, IntPtr lParam)
        {
            return MirandaContext.Current.CallService(MS_PROTO_CHAINSEND, wParam, lParam);
        }

        public static int ChainReceive(ContactChainData chainData)
        {
            if (chainData == null)
                throw new ArgumentNullException("chainData");

            return ChainReceive(chainData.WParam, chainData.CcsDataPtr);
        }

        public static int ChainReceive(UIntPtr wParam, IntPtr lParam)
        {
            return MirandaContext.Current.CallService(MS_PROTO_CHAINRECV, wParam, lParam);
        }

        #endregion
    }
}
