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
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins.Resources;
using System.Diagnostics;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    internal static class HookManager
    {
        public static void CreateHooks(params HookDescriptor[] hooks)
        {
            if (hooks == null)
                throw new ArgumentNullException("hooks");

            if (hooks.Length == 0)
                return;

            foreach (HookDescriptor hook in hooks)
                CreateHook(hook);
        }

        public static void CreateHook(HookDescriptor hook)
        {
            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(hook);

                switch (hook.HookType)
                {
                    case HookType.EventHook:
                        {
                            HookEvent(hook);
                            break;
                        }
                    case HookType.ServiceFunction:
                        {
                            CreateServiceFunction(hook);
                            break;
                        }
                    default:
                        throw new ArgumentException("descriptor");
                }
            }
            finally
            {
                SynchronizationHelper.EndUpdate(hook);
            }
        }

        private static void CreateServiceFunction(HookDescriptor hook)
        {
            if (!ServiceManager.ServiceExists(hook.Name))
            {
                hook.MirandaHandle = MirandaContext.Current.PluginLink.NativePluginLink.CreateServiceFunction(hook.Name, hook.Callback);

                if (hook.MirandaHandle == IntPtr.Zero)
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable1_ServiceFunctionCreationFailed, hook.Name));
            }
            else
            {
                if (!hook.Owner.ServiceFunctions.Exists(delegate(HookDescriptor _hook)
                {
                    return _hook.Equals(hook) && _hook.MirandaHandle != IntPtr.Zero;
                }))
                    throw new InvalidOperationException(String.Format(TextResources.ExceptionMsg_Formatable1_ServiceFunctionAlreadyExists, hook.Name));
            }
        }

        private static void HookEvent(HookDescriptor hook)
        {
            hook.MirandaHandle = MirandaContext.Current.PluginLink.NativePluginLink.HookEvent(hook.Name, hook.Callback);

            if (hook.MirandaHandle == IntPtr.Zero)
                throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable1_EventHookingFailed, hook.Name));
        }

        public static void DestroyHook(HookDescriptor hook)
        {
            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(hook);

                switch (hook.HookType)
                {
                    case HookType.EventHook:
                        {
                            UnhookEvent(hook);
                            break;
                        }
                    case HookType.ServiceFunction:
                        {
                            DestroyServiceFunction(hook);
                            break;
                        }
                    default:
                        throw new ArgumentException("descriptor");
                }
            }
            finally
            {
                SynchronizationHelper.EndUpdate(hook);
            }
        }

        private static void UnhookEvent(HookDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            if (descriptor.HookType != HookType.EventHook)
                throw new ArgumentOutOfRangeException("descriptor");

            if (descriptor.MirandaHandle == IntPtr.Zero)
                return;

            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(descriptor);

                int result = MirandaContext.Current.PluginLink.NativePluginLink.UnhookEvent(descriptor.MirandaHandle);
                Debug.Assert(result == 0);

                descriptor.MirandaHandle = IntPtr.Zero;
            }
            finally
            {
                SynchronizationHelper.EndUpdate(descriptor);
            }
        }

        private static void DestroyServiceFunction(HookDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            if (descriptor.HookType != HookType.ServiceFunction)
                throw new ArgumentOutOfRangeException("descriptor");

            if (descriptor.MirandaHandle == IntPtr.Zero)
                return;

            try
            {
                SynchronizationHelper.BeginDescriptorUpdate(descriptor);

                int result = MirandaContext.Current.PluginLink.NativePluginLink.DestroyServiceFunction(descriptor.MirandaHandle);
                Debug.Assert(result == 0);

                descriptor.MirandaHandle = IntPtr.Zero;
            }
            finally
            {
                SynchronizationHelper.EndUpdate(descriptor);
            }
        }
    }
}
