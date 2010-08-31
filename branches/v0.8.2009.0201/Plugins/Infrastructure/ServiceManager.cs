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
using System.Diagnostics;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public static class ServiceManager
    {
        [CLSCompliant(false)]
        public static void CreateServiceFunction(string serviceName, Callback callback, MirandaPlugin owner)
        {
            if (String.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("serviceName");

            if (callback == null)
                throw new ArgumentNullException("callback");

            if (owner == null)
                throw new ArgumentNullException("owner");

            if (!owner.Initialized)
                throw new InvalidOperationException(TextResources.ExceptionMsg_PluginNotInitialized);

            HookDescriptorCollection collection = owner.Descriptor.ServiceFunctions;

            try
            {
                SynchronizationHelper.BeginPluginUpdate(owner);
                SynchronizationHelper.BeginCollectionUpdate(collection);

                HookDescriptor descriptor = HookDescriptor.SetUpAndStore(collection, serviceName, owner.Descriptor, callback, HookType.ServiceFunction);
                descriptor.RegisteredManually = true;

                HookManager.CreateHook(descriptor);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(owner);
                SynchronizationHelper.EndUpdate(collection);
            }
        }

        public static bool ServiceExists(string name)
        {
            return MirandaContext.Current.PluginLink.NativePluginLink.ServiceExists(name) != 0;
        }

        [CLSCompliant(false)]
        public static Callback GetService(string serviceName)
        {
            return CallbackWrapper.Create(serviceName);
        }

        public static void RemoveServiceFunction(MirandaPlugin owner, string eventName)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (String.IsNullOrEmpty(eventName))
                throw new ArgumentNullException("eventName");

            HookDescriptorCollection collection = owner.Descriptor.ServiceFunctions;

            try
            {
                SynchronizationHelper.BeginCollectionUpdate(collection);
                HookDescriptor descriptor = null;

                if ((descriptor = collection.Find(eventName)) == null)
                    return;

                HookManager.DestroyHook(descriptor);
                collection.Remove(descriptor);
            }
            finally
            {
                SynchronizationHelper.EndUpdate(collection);
            }
        }        
    }
}
