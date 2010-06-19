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

namespace Virtuoso.Miranda.Plugins.Infrastructure.Protocols
{    
    public sealed class AckEventArgs : MirandaEventArgs
    {
        #region Properties

        private Protocol protocol;
        public Protocol Protocol
        {
            get { return protocol; }
        }

        private ContactInfo contact;
        public ContactInfo Contact
        {
            get { return contact; }
        }

        private AckType type;
        public AckType Type
        {
            get { return type; }
        }

        private AckResult result;
        public AckResult Result
        {
            get { return result; }
        }

        private IntPtr processHandle;
        public IntPtr ProcessHandle
        {
            get { return processHandle; }
        }

        private IntPtr lParam;
        public IntPtr LParam
        {
            get { return lParam; }
        }

        #endregion

        #region .ctors

        public AckEventArgs() { }
        
        internal unsafe static AckEventArgs FromACKDATA(IntPtr pAckData)
        {
            if (pAckData == IntPtr.Zero)
                throw new ArgumentNullException("pAckData");

            ACKDATA ackData = *(ACKDATA*)pAckData.ToPointer();
            AckEventArgs ackArgs = new AckEventArgs();

            ackArgs.contact = ContactInfo.FromHandle(ackData.ContactHandle);
            ackArgs.lParam = ackData.LParam;
            ackArgs.processHandle = ackData.ProcessHandle;            
            ackArgs.result = (AckResult)ackData.Result;
            ackArgs.type = (AckType)ackData.Type;

            if (ackData.ModuleName != IntPtr.Zero)
                ackArgs.protocol = MirandaContext.Current.Protocols[Translate.ToString(ackData.ModuleName, StringEncoding.Ansi)];

            return ackArgs;
        }

        #endregion
    }
}
