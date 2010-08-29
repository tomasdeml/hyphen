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
using Virtuoso.Hyphen;
using System.Runtime.CompilerServices;
using Virtuoso.Miranda.Plugins.Collections;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [CLSCompliant(false)]
    public sealed class EventHandle : MirandaObject
    {
        #region Fields

        private readonly MirandaPlugin owner;
        internal MirandaPlugin Owner
        {
            get { return owner; }
        }

        private readonly string eventName;
        public string EventName
        {
            get { return eventName; }
        } 

        #endregion

        #region .ctors

        internal EventHandle(MirandaPlugin owner, string eventName, IntPtr handle)
        {
            if (handle == IntPtr.Zero) 
                throw new ArgumentNullException("handle");

            if (owner == null) 
                throw new ArgumentNullException("owner");

            if (eventName == null) 
                throw new ArgumentNullException("eventName");

            this.owner = owner;
            this.MirandaHandle = handle;
            this.eventName = eventName;

            List<EventHandle> eventHandles = owner.Descriptor.EventHandles;

            try
            {
                SynchronizationHelper.BeginCollectionUpdate(eventHandles);
                eventHandles.Add(this);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(eventHandles);
            }
        }

        #endregion        

        #region Methods

        public int FireEvent()
        {
            return FireEvent(UIntPtr.Zero, IntPtr.Zero);
        }

        public int FireEvent(UIntPtr wParam, IntPtr lParam)
        {
            MirandaPluginLink link = MirandaContext.Current.PluginLink;

            lock (link)
                return link.NativePluginLink.NotifyEventHooks(MirandaHandle, wParam, lParam);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetDefaultSubscriber(Callback subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException("subscriber");

            MirandaPluginLink link = MirandaContext.Current.PluginLink;

            lock (link)
            {
                int result;

                if ((result = link.NativePluginLink.SetHookDefaultForHookableEvent(MirandaHandle, subscriber)) != 0)
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, "SetHookDefaultForHookableEvent", result.ToString()));
            }
        }

        #endregion
    }
}
