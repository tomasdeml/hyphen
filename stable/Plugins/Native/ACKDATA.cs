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

namespace Virtuoso.Miranda.Plugins.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ACKDATA
    {
        private readonly int Size;
        public IntPtr ModuleName;
        public IntPtr ContactHandle;
        public int Type;
        public int Result;
        public IntPtr ProcessHandle;
        public IntPtr LParam;

        public ACKDATA(IntPtr moduleName, int type, int result)
        {
            this.ModuleName = moduleName;
            this.Type = type;
            this.Result = result;
            
            this.ContactHandle = IntPtr.Zero;
            this.ProcessHandle = IntPtr.Zero;
            this.LParam = IntPtr.Zero;

            this.Size = Marshal.SizeOf(typeof(ACKDATA));
        }
    }

    //a general network 'ack'
    //wParam=0
    //lParam=(LPARAM)(ACKDATA*)&ack
    //Note that just because definitions are here doesn't mean they will be sent.
    //Read the documentation for the function you are calling to see what replies
    //you will receive.
    /*typedef struct {
        int cbSize;
        const char *szModule;  //the name of the protocol module which initiated this ack
        HANDLE hContact;
        int type;     //an ACKTYPE_ constant
        int result; 	//an ACKRESULT_ constant
        HANDLE hProcess;   //a caller-defined process code
        LPARAM lParam;	   //caller-defined extra info
    } ACKDATA;*/
}
