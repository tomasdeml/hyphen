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
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Hyphen;
using System.Runtime.CompilerServices;
using Virtuoso.Miranda.Plugins.Infrastructure.Protocols;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public class Protocol : ContextWorker, ISettingOwner
    {
        #region Constants

        internal const string PS_GETSTATUS = "/GetStatus";
        internal const string PS_SETSTATUS = "/SetStatus";
        internal const string PS_GETCAPS = "/GetCaps";
        internal const string PS_GETNAME = "/GetName";
        internal const string PS_LOADICON = "/LoadIcon";
        internal const string PSS_MESSAGE = "/SendMsg";

        private const string MS_PROTO_ISPROTOONCONTACT = "Proto/IsProtoOnContact";

        #endregion

        #region Fields

        private static readonly Protocol unknownProtocol = new Protocol();

        private readonly string name;
        private readonly ProtocolType type;

        #endregion

        #region .ctors

        internal Protocol()
        {
            this.name = String.Empty;
            this.type = ProtocolType.Other;
        }

        internal Protocol(string name, ProtocolType type)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (!Enum.IsDefined(typeof(ProtocolType), type))
                throw new ArgumentOutOfRangeException("type");

            this.name = name;
            this.type = type;
        }

        internal Protocol(ref PROTOCOLDESCRIPTOR descriptor)
        {
            if (descriptor.Name == IntPtr.Zero)
                throw new ArgumentException();

            this.name = Translate.ToString(descriptor.Name, StringEncoding.Ansi);
            this.type = (ProtocolType)descriptor.Type;
        }

        ~Protocol()
        {
            if (namePtr.IsValid)
                namePtr.Free();
        }

        #endregion

        #region Events

        private static MirandaEventHandler<ProtocolStatusChangeEventArgs> StatusChangedEventHandler;
        public static event MirandaEventHandler<ProtocolStatusChangeEventArgs> StatusChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                // Important to check to not hook the Ack for nothing
                if (value == null)
                    throw new ArgumentNullException("value");

                // First subscriber?
                if (StatusChangedEventHandler == null)
                    AckRouter.AckReceived += AckRouter_AckReceived;

                LazyEventBinder.AttachDelegate<MirandaEventHandler<ProtocolStatusChangeEventArgs>>(ref StatusChangedEventHandler, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<ProtocolStatusChangeEventArgs>>(ref StatusChangedEventHandler, value);

                // No more subscribers?
                if (StatusChangedEventHandler == null)
                    AckRouter.AckReceived -= AckRouter_AckReceived;
            }
        }

        private static bool AckRouter_AckReceived(object sender, AckEventArgs e)
        {
            if (e.Type == AckType.Status)
            {
                if (StatusChangedEventHandler != null)
                    StatusChangedEventHandler(e.Protocol, new ProtocolStatusChangeEventArgs(e.Protocol, (StatusMode)e.LParam));
            }

            return EventResult.HonourEventChain;
        }

        #endregion

        #region Methods

        private void CheckUnknown()
        {
            if (String.IsNullOrEmpty(name))
                throw new InvalidOperationException(TextResources.ExceptionMsg_CallInvalidForUnknownNetworkProtocol);
        }

        [CLSCompliant(false)]
        public int CallProtocolService(string serviceName, UIntPtr wParam, IntPtr lParam)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            CheckUnknown();
            return MirandaContext.Current.CallService(GetProtoServiceName(serviceName), wParam, lParam);
        }

        public string GetProtoServiceName(string service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return String.Format("{0}{1}", Name, service);
        }
        
        public bool HasInChain(ContactInfo contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            return HasInChain(contact.MirandaHandle);
        }

        public bool HasInChain(IntPtr contactHandle)
        {
            return Convert.ToBoolean(Context.CallService(MS_PROTO_ISPROTOONCONTACT, contactHandle, NamePtr));
        }

        #endregion

        #region Properties

        public static Protocol UnknownProtocol
        {
            get { return Protocol.unknownProtocol; }
        }

        public StatusMode Status
        {
            get
            {
                return (StatusMode)CallProtocolService(PS_GETSTATUS, UIntPtr.Zero, IntPtr.Zero);
            }
            set
            {
                CallProtocolService(PS_SETSTATUS, (UIntPtr)value, IntPtr.Zero);
            }
        }

        public string Name
        {
            get { return name; }
        }

        public ProtocolType Type
        {
            get { return type; }
        }

        public bool IsUnknown
        {
            get
            {
                return object.ReferenceEquals(this, UnknownProtocol);
            }
        }

        private UnmanagedStringHandle namePtr;
        protected virtual IntPtr NamePtr
        {
            get
            {
                if (!namePtr.IsValid)
                    namePtr = Translate.ToHandle(Name, StringEncoding.Ansi);

                return namePtr.IntPtr;
            }
        }

        #endregion
    }
}
