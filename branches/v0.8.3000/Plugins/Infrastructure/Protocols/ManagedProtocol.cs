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
using Virtuoso.Miranda.Plugins.Resources;
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Helpers;

namespace Virtuoso.Miranda.Plugins.Infrastructure.Protocols
{
    public sealed class ManagedProtocol : Protocol
    {
        #region Constants

        private const string MS_PROTO_ADDTOCONTACT = "Proto/AddToContact";

        private bool Registered;

        #endregion

        #region .ctors

        internal ManagedProtocol(string name, ProtocolType type) : base(name, type) { }

        #endregion

        #region Properties

        private PROTOCOLDESCRIPTOR nativeDescriptor;
        public PROTOCOLDESCRIPTOR NativeDescriptor
        {
            get { return nativeDescriptor; }
        }	

        #endregion

        #region Methods

        internal void Register()
        {
            if (Registered)
                throw new InvalidOperationException();

            PROTOCOLDESCRIPTOR descriptor = new PROTOCOLDESCRIPTOR(Name, Type);
            UnmanagedStructHandle<PROTOCOLDESCRIPTOR> nativeHandle = UnmanagedStructHandle<PROTOCOLDESCRIPTOR>.Empty;

            try
            {
                nativeHandle = new UnmanagedStructHandle<PROTOCOLDESCRIPTOR>(ref descriptor);
                int result = Context.CallService(MirandaServices.MS_PROTO_REGISTERMODULE, UIntPtr.Zero, nativeHandle.IntPtr);

                if (result != 0)
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, result.ToString()));

                this.nativeDescriptor = descriptor;
                Registered = true;
            }
            finally
            {
                nativeHandle.Free();
            }
        }

        internal void Unregister()
        {
            if (!Registered)
                throw new InvalidOperationException();

            // Currently, nothing else to do
        }
        
        public void AddToContact(ContactInfo contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            AddToContact(contact.MirandaHandle);
        }

        public void AddToContact(IntPtr contactHandle)
        {
            int result = Context.CallService(MS_PROTO_ADDTOCONTACT, contactHandle, NativeDescriptor.Name);

            if (result != 0)
                throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_PROTO_ADDTOCONTACT, result.ToString()));
        }

        #endregion
    }
}
