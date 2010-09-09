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
using Virtuoso.Miranda.Plugins;

namespace Virtuoso.Hyphen.Native
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void AsyncFunctionCall(IntPtr ptr);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr CreateHookableEventPrototype([MarshalAs(UnmanagedType.LPStr)] string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int DestroyHookableEventPrototype(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate int NotifyEventHooksPrototype(IntPtr handle, UIntPtr wParam, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate IntPtr HookEventPrototype([MarshalAs(UnmanagedType.LPStr)] string name, Callback hook);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate IntPtr HookEventMessagePrototype([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr hwnd, uint msg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int UnhookEventPrototype(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate IntPtr CreateServiceFunctionPrototype([MarshalAs(UnmanagedType.LPStr)] string name, Callback service);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate IntPtr CreateTransientServiceFunctionPrototype([MarshalAs(UnmanagedType.LPStr)] string name, Callback service);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int DestroyServiceFunctionPrototype(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate int CallServicePrototype([MarshalAs(UnmanagedType.LPStr)] string name, UIntPtr wParam, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public unsafe delegate int CallServiceUnsafePrototype([MarshalAs(UnmanagedType.LPStr)] string serviceName, void* wParam, void* lParam);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate int ServiceExistsPrototype([MarshalAs(UnmanagedType.LPStr)] string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate int CallServiceSyncPrototype([MarshalAs(UnmanagedType.LPStr)] string name, UIntPtr wParam, IntPtr lParam);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int CallFunctionAsyncPrototype(AsyncFunctionCall function, IntPtr ptr);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl), CLSCompliant(false)]
    public delegate int SetHookDefaultForHookableEventPrototype(IntPtr handle, Callback hook);
}
