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
using Virtuoso.Miranda.Plugins.Collections;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Native;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public static class EventManager
    {
        [CLSCompliant(false)]
        public static void CreateEventHook(string eventName, Callback callback, MirandaPlugin owner)
        {
            if (String.IsNullOrEmpty(eventName))
                throw new ArgumentNullException("eventName");

            if (callback == null)
                throw new ArgumentNullException("callback");

            if (owner == null)
                throw new ArgumentNullException("owner");

            if (!owner.Initialized)
                throw new InvalidOperationException(TextResources.ExceptionMsg_PluginNotInitialized);

            HookDescriptorCollection collection = owner.Descriptor.EventHooks;

            try
            {
                SynchronizationHelper.BeginPluginUpdate(owner);
                SynchronizationHelper.BeginCollectionUpdate(collection);

                HookDescriptor descriptor = HookDescriptor.SetUpAndStore(collection, eventName, owner.Descriptor, callback, HookType.EventHook);
                descriptor.RegisteredManually = true;

                HookManager.CreateHook(descriptor);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(owner);
                SynchronizationHelper.EndUpdate(collection);
            }
        }

        public static void RemoveEventHook(string eventName, MirandaPlugin owner)
        {
            if (String.IsNullOrEmpty(eventName))
                throw new ArgumentNullException("eventName");

            if (owner == null)
                throw new ArgumentNullException("owner");

            if (!owner.Initialized)
                throw new InvalidOperationException(TextResources.ExceptionMsg_PluginNotInitialized);

            HookDescriptorCollection collection = owner.Descriptor.EventHooks;

            try
            {
                SynchronizationHelper.BeginCollectionUpdate(collection);
                HookDescriptor descriptor = null;

                if ((descriptor = owner.Descriptor.EventHooks.Find(eventName)) == null)
                    return;

                HookManager.DestroyHook(descriptor);
                collection.Remove(descriptor);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(collection);
            }
        }

        [CLSCompliant(false)]
        public static EventHandle CreateEvent(string eventName, MirandaPlugin owner)
        {
            return CreateEvent(eventName, owner, null);
        }

        [CLSCompliant(false)]
        public static EventHandle CreateEvent(string eventName, MirandaPlugin owner, Callback defaultSubscriber)
        {
            if (String.IsNullOrEmpty(eventName))
                throw new ArgumentNullException("eventName");

            if (owner == null)
                throw new ArgumentNullException("owner");

            if (!owner.Initialized)
                throw new InvalidOperationException(TextResources.ExceptionMsg_PluginNotInitialized);

            if (ServiceManager.ServiceExists(eventName))
                throw new ArgumentException("eventName");

            EventHandle handle = new EventHandle(owner, eventName, MirandaContext.Current.PluginLink.NativePluginLink.CreateHookableEvent(eventName));

            if (defaultSubscriber != null)
                handle.SetDefaultSubscriber(defaultSubscriber);

            return handle;
        }

        public static void RemoveEvent(EventHandle eventHandle)
        {
            if (eventHandle == null)
                throw new ArgumentNullException("eventHandle");

            MirandaPluginLink link = MirandaContext.Current.PluginLink;

            if (eventHandle.MirandaHandle != IntPtr.Zero)
            {
                int result;
                if ((result = link.NativePluginLink.DestroyHookableEvent(eventHandle.MirandaHandle)) != 0)
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, "DestroyHookableEvent", result.ToString()));

                eventHandle.MirandaHandle = IntPtr.Zero;
                EventHandleCollection handles = eventHandle.Owner.Descriptor.EventHandles;

                try
                {
                    SynchronizationHelper.BeginCollectionUpdate(handles);
                    handles.Remove(eventHandle);
                }
                finally
                {
                    SynchronizationHelper.EndUpdate(handles);
                }
            }
        }
    }
}
