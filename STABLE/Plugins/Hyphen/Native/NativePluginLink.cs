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
using Virtuoso.Hyphen.Native;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security;

namespace Virtuoso.Hyphen.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi), SuppressUnmanagedCodeSecurity]
    internal sealed class NativePluginLink
    {
        public readonly CreateHookableEventPrototype CreateHookableEvent;
        public readonly DestroyHookableEventPrototype DestroyHookableEvent;
        public readonly NotifyEventHooksPrototype NotifyEventHooks;
        public readonly HookEventPrototype HookEvent;
        public readonly HookEventMessagePrototype HookEventMessage;
        public readonly UnhookEventPrototype UnhookEvent;
        public readonly CreateServiceFunctionPrototype CreateServiceFunction;
        public readonly CreateTransientServiceFunctionPrototype CreateTransientServiceFunction;
        public readonly DestroyServiceFunctionPrototype DestroyServiceFunction;
        public readonly CallServicePrototype CallService;
        public readonly ServiceExistsPrototype ServiceExists;
        public readonly CallServiceSyncPrototype CallServiceSync;
        public readonly CallFunctionAsyncPrototype CallFunctionAsync;
        public readonly SetHookDefaultForHookableEventPrototype SetHookDefaultForHookableEvent;

        //see modules.h for what all this stuff is
        /*typedef struct {
	        HANDLE (*CreateHookableEvent)(const char *);
	        int (*DestroyHookableEvent)(HANDLE);
	        int (*NotifyEventHooks)(HANDLE,WPARAM,LPARAM);
	        HANDLE (*CreateEventHook)(const char *,MIRANDAHOOK);
	        HANDLE (*HookEventMessage)(const char *,HWND,UINT);
	        int (*RemoveEventHook)(HANDLE);
	        HANDLE (*CreateServiceFunction)(const char *,MIRANDASERVICE);
	        HANDLE (*CreateTransientServiceFunction)(const char *,MIRANDASERVICE);
	        int (*RemoveServiceFunction)(HANDLE);
	        int (*CallService)(const char *,WPARAM,LPARAM);
	        int (*ServiceExists)(const char *);		  //v0.1.0.1+
	        int (*CallServiceSync)(const char *,WPARAM,LPARAM);		//v0.3.3+
	        int (*CallFunctionAsync) (void (__stdcall *)(void *), void *);	//v0.3.4+
	        int (*SetHookDefaultForHookableEvent) (HANDLE, MIRANDAHOOK); // v0.3.4 (2004/09/15)
        } PLUGINLINK;
        */  
    }
}
