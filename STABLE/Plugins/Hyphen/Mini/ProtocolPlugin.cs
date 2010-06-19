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
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.Runtime.InteropServices;
using System.Drawing;
using Virtuoso.Miranda.Plugins;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins.Infrastructure.Protocols;

namespace Virtuoso.Hyphen.Mini
{
    /// <summary>
    /// Represents a base class for managed Miranda protocols.
    /// </summary>
    public abstract class ProtocolPlugin : StandalonePlugin
    {
        #region Fields

        private IntPtr namePtr;
        private int NameCapacity;

        #endregion

        #region .ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolPlugin"/> class.
        /// </summary>
        protected ProtocolPlugin() { }

        #endregion

        #region API overrides

        /// <summary>
        /// Gets the plugin interfaces.
        /// </summary>
        public override Guid[] PluginInterfaces
        {
            get { return new Guid[] { Virtuoso.Miranda.Plugins.Native.UUID.ProtocolUUID }; }
        }

        /// <summary>
        /// Initializes the instance and creates essential protocol services.
        /// </summary>
        /// <param name="pPluginLink"></param>
        internal override void LoadInternal(IntPtr pPluginLink)
        {
            ThisProtocol = new ManagedProtocol(Name, this.ProtocolType);
            ThisProtocol.Register();

            ProtocolStatus = StatusMode.Offline;

            ServiceManager.CreateServiceFunction(ThisProtocol.GetProtoServiceName(Protocol.PS_GETNAME), PSGetName, this);
            ServiceManager.CreateServiceFunction(ThisProtocol.GetProtoServiceName(Protocol.PS_GETSTATUS), PSGetStatus, this);
            ServiceManager.CreateServiceFunction(ThisProtocol.GetProtoServiceName(Protocol.PS_SETSTATUS), PSSetStatus, this);
            ServiceManager.CreateServiceFunction(ThisProtocol.GetProtoServiceName(Protocol.PS_LOADICON), PSLoadIcon, this);
            ServiceManager.CreateServiceFunction(ThisProtocol.GetProtoServiceName(Protocol.PS_GETCAPS), PSGetCaps, this);
            ServiceManager.CreateServiceFunction(ThisProtocol.GetProtoServiceName(Protocol.PSS_MESSAGE), PSSSendMessage, this);

            base.LoadInternal(pPluginLink);
        }

        /// <summary>
        /// Unloads the protocol.
        /// </summary>
        internal override void UnloadInternal()
        {
            base.UnloadInternal();
            ThisProtocol.Unregister();
        }

        #endregion

        #region Services

        /// <summary>
        /// Gets a human-readable name for the protocol.
        /// </summary>
        /// <param name="capacity">The number of characters in the buffer.</param>
        /// <param name="pBuffer">Buffer pointer.</param>
        /// <returns>Returns 0 on success, nonzero on failure.</returns>
        private unsafe int PSGetName(UIntPtr capacity, IntPtr pBuffer)
        {
            if (namePtr == IntPtr.Zero)
            {
                byte[] nameBytes = Encoding.Default.GetBytes(Name ?? String.Empty);
                NameCapacity = nameBytes.Length + 1;

                namePtr = Marshal.AllocHGlobal(NameCapacity);
                Marshal.Copy(nameBytes, 0, namePtr, nameBytes.Length);

                *(((byte*)namePtr) + nameBytes.Length) = 0;
            }

            uint count = capacity.ToUInt32();

            for (long i = 0; i < count && i < NameCapacity; i++)
                *(byte*)(pBuffer.ToInt64() + i) = *(byte*)(namePtr.ToInt64() + i);

            return 0;
        }

        /// <summary>
        /// Gets the status mode that a protocol is currently in.
        /// </summary>
        /// <param name="wParam">Not used.</param>
        /// <param name="lParam">Not used.</param>
        /// <returns>Returns the status mode.</returns>
        private int PSGetStatus(UIntPtr wParam, IntPtr lParam)
        {
            return (int)ProtocolStatus;
        }

        /// <summary>
        /// Changes the protocol's status mode.
        /// </summary>
        /// <param name="newStatusRaw">New status values.</param>
        /// <param name="lParam">Not used.</param>
        /// <returns>Returns 0 on success, nonzero on failure.</returns>
        /// <remarks>
        /// Will send an ack with: 
        /// type=ACKTYPE_STATUS, result=ACKRESULT_SUCCESS, hProcess=(HANDLE)previousMode, lParam=newMode.
        /// </remarks>
        private int PSSetStatus(UIntPtr newStatusRaw, IntPtr lParam)
        {
            StatusMode prevStatus = ProtocolStatus;
            StatusMode newStatus = (StatusMode)newStatusRaw;

            ProtocolStatus = newStatus;
            AckBroadcaster.BroadcastAck(ThisProtocol.NativeDescriptor.Name, AckType.Status, true, IntPtr.Zero, (IntPtr)(int)prevStatus, Translate.ToHandle(newStatusRaw));

            return 0;
        }

        /// <summary>
        /// Loads one of the protocol-specific icons
        /// </summary>
        /// <param name="whichIcon">Which icon (currently ignored).</param>
        /// <param name="lParam">Not used.</param>
        /// <returns>Icon handle (HICON).</returns>
        private int PSLoadIcon(UIntPtr whichIcon, IntPtr lParam)
        {
            if (ProtocolIcon == null)
                return SystemIcons.Application.Handle.ToInt32();
            else
                return ProtocolIcon.Handle.ToInt32();
        }

        /// <summary>
        /// Gets the capability flags of the module.
        /// </summary>
        /// <param name="flagsNum">Flags category.</param>
        /// <param name="lParam">Not used.</param>
        /// <returns></returns>
        private int PSGetCaps(UIntPtr flagsNum, IntPtr lParam)
        {
            switch ((ProtocolFlagsKind)flagsNum)
            {
                case ProtocolFlagsKind.Capabilities:
                    return (int)SupportedCapabilities;
                case ProtocolFlagsKind.StatusModes:
                    return (int)SupportedStatusModes;
                case ProtocolFlagsKind.AwayStatusModes:
                    return (int)SupportedAwayStatusModes;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Sends an instant message.
        /// </summary>
        /// <param name="flags">Flags.</param>
        /// <param name="pCcsData">Contact-Chain-Send data pointer.</param>
        /// <returns>Returns a hProcess corresponding to the one in the ack event.</returns>
        /// <remarks>
        /// Will send an ack when the message actually gets sent type=ACKTYPE_MESSAGE, result=success/failure, lParam=0.
        /// </remarks>
        private unsafe int PSSSendMessage(UIntPtr flags, IntPtr pCcsData)
        {
            return SendMessage(new ContactChainData(pCcsData));
        }

        #endregion

        #region Properties

        private ManagedProtocol thisProtocol;
        /// <summary>
        /// Gets the managed protocol descriptor for this module.
        /// </summary>
        public ManagedProtocol ThisProtocol
        {
            get { return thisProtocol; }
            internal set { thisProtocol = value; }
        }

        protected IntPtr NamePtr
        {
            get { return namePtr; }
        }

        #endregion

        #region Abstracts

        /// <summary>
        /// Gets the protocol type.
        /// </summary>
        protected internal abstract ProtocolType ProtocolType { get; }

        /// <summary>
        /// Gets the protocol icon.
        /// </summary>
        protected internal abstract Icon ProtocolIcon { get; }

        /// <summary>
        /// Gets the protocol supported capabilities.
        /// </summary>
        protected internal abstract ProtocolCapabilities SupportedCapabilities { get; }

        #endregion

        #region Virtuals

        /// <summary>
        /// Gets the status modes the protocol supports an away message for.
        /// </summary>
        protected internal virtual ProtocolStatusModes SupportedAwayStatusModes { get { return (ProtocolStatusModes)0; } }

        /// <summary>
        /// Gets the protocol supported status modes.
        /// </summary>
        protected internal virtual ProtocolStatusModes SupportedStatusModes { get { return (ProtocolStatusModes)0; } }

        /// <summary>
        /// Gets or sets the protocol status.
        /// </summary>
        protected internal virtual StatusMode ProtocolStatus { get { return StatusMode.Offline; } set { } }

        protected virtual int SendMessage(ContactChainData msgData) { return 0; }

        #endregion
    }
}
