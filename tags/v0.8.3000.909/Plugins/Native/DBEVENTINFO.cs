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
    [StructLayout(LayoutKind.Sequential, Pack = 4), CLSCompliant(false), Serializable]
    public struct DBEVENTINFO
    {
        #region Fields

        private readonly int Size;        

        public IntPtr Module;
        public UInt32 Timestamp;
        public UInt32 Flags;
        public UInt16 EventType;
        public UInt32 BlobSize;

        [NonSerialized]
        public IntPtr BlobPtr;

        #endregion

        #region .ctors

        public DBEVENTINFO(int blobSize, IntPtr blobPtr)
        {            
            this.Module = IntPtr.Zero;
            this.Timestamp = this.Flags = this.EventType = 0;
            this.BlobSize = (uint)blobSize;
            this.BlobPtr = blobPtr;

            unsafe
            {
                this.Size = sizeof(DBEVENTINFO);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the ANSI-\0-UNICODE-\0\0 layout in the memory.
        /// </summary>
        /// <param name="data">String to layout.</param>
        /// <param name="pBlob">[OUT] Blob pointer to the resulting memory layout.</param>
        /// <returns>Blob size in bytes.</returns>
        /// <remarks>Message blob format: ansi\0unicode\0\0</remarks>
        public static int LayoutAnsiUniString(string data, out IntPtr pBlob)
        {
            int ansiBytesCount = Encoding.Default.GetByteCount(data);
            int unicodeBytesCount = Encoding.Unicode.GetByteCount(data);
            int terminatorBytesCount = 3;

            int totalBytes = ansiBytesCount + unicodeBytesCount + terminatorBytesCount;

            pBlob = Marshal.AllocHGlobal(totalBytes);
            IntPtr pAnsiEnd = new IntPtr(pBlob.ToInt64() + (long)ansiBytesCount);
            IntPtr pAnsiTermEnd = new IntPtr(pAnsiEnd.ToInt64() + 1L);

            Marshal.Copy(Encoding.Default.GetBytes(data), 0, pBlob, ansiBytesCount);
            Marshal.Copy(new char[] { '\0' }, 0, pAnsiEnd, 1);

            Marshal.Copy(Encoding.Unicode.GetBytes(data), 0, pAnsiTermEnd, unicodeBytesCount);
            Marshal.Copy(new char[] { '\0', '\0' }, 0, new IntPtr(pAnsiTermEnd.ToInt64() + (long)unicodeBytesCount), 2);

            return totalBytes;
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DBTIMETOSTRING
    {
        #region Fields

        public IntPtr Format;
        public IntPtr Output;
        public int MaxBytes;

        #endregion

        #region .ctors

        public DBTIMETOSTRING(string format)
        {
            Format = new UnmanagedStringHandle(format, StringEncoding.Ansi).IntPtr;
            Output = IntPtr.Zero;
            MaxBytes = 0;
        }

        public void Free()
        {
            if (Format != IntPtr.Zero)
                Marshal.FreeHGlobal(Format);
        }

        #endregion
    }

    //    typedef struct {
    //    char *szFormat;		//format string, as above
    //    char *szDest;		//place to put the output string
    //    int cbDest;			//maximum number of bytes to put in szDest
    //} DBTIMETOSTRING;

    //    typedef struct {
    //    int cbSize;       //size of the structure in bytes
    //    char *szModule;	  //pointer to eventName of the module that 'owns' this
    //                      //event, ie the one that is in control of the data format
    //    DWORD timestamp;  //seconds since 00:00, 01/01/1970. Gives us times until
    //                      //2106 unless you use the standard C library which is
    //                      //signed and can only do until 2038. In GMT.
    //    DWORD flags;	  //the omnipresent flags
    //    WORD eventType;	  //module-defined event type field
    //    DWORD cbBlob;	  //size of pBlob in bytes
    //    PBYTE pBlob;	  //pointer to buffer containing module-defined event data
    //} DBEVENTINFO;
}
