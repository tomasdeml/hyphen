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
using System.Threading;
using Virtuoso.Miranda.Plugins.Resources;
using System.Runtime.CompilerServices;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    /// <summary>
    /// Represents a late-binded master subscriber of Miranda events.
    /// </summary>
    internal static class LazyEventBinder
    {
        #region Fields

        private static readonly Dictionary<string, HookDescriptor> EventHandlerDescriptorsTable;

        #endregion

        #region .ctors

        static LazyEventBinder()
        {
            EventHandlerDescriptorsTable = new Dictionary<string, HookDescriptor>(3);
        }

        #endregion

        #region Management methods

        public static void AttachDelegate<T>(ref T destination, T value) where T : class
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("T");
            
            destination = Delegate.Combine(destination as Delegate, value as Delegate) as T;
        }

        public static void DetachDelegate<T>(ref T destination, T value) where T : class
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("T");

            destination = Delegate.Remove(destination as Delegate, value as Delegate) as T;
        }

        public static void HookMirandaEvent(string eventName, Callback callback)
        {
            lock (EventHandlerDescriptorsTable)
            {
                if (EventHandlerDescriptorsTable.ContainsKey(eventName))
                    return;

                HookDescriptor descriptor = HookDescriptor.SetUpAndStore(EventHandlerDescriptorsTable, eventName, MirandaPlugin.Hyphen.Singleton.Descriptor, callback, HookType.EventHook);
                HookManager.CreateHook(descriptor);
            }
        }

        public static void UnhookMirandaEvent(string eventName, Delegate callback)
        {
            if (callback != null)
                return;

            lock (EventHandlerDescriptorsTable)
            {
                if (!EventHandlerDescriptorsTable.ContainsKey(eventName)) 
                    return;

                int result = MirandaContext.Current.PluginLink.NativePluginLink.UnhookEvent(EventHandlerDescriptorsTable[eventName].MirandaHandle);
                Debug.Assert(result == 0);

                EventHandlerDescriptorsTable.Remove(eventName);
            }
        }

        #endregion
    }
}
