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
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Native
{
    public enum StringEncoding
    {
        Ansi,
        Unicode,
        MirandaDefault
    }

    public struct UnmanagedStringHandle : IUnmanagedMemoryHandle
    {
        #region Fields

        private IntPtr intPtr;
        public static readonly UnmanagedStringHandle Empty = new UnmanagedStringHandle();

        private string originalString;
        private StringEncoding encoding;

        #endregion

        #region .ctors

        public UnmanagedStringHandle(string str, StringEncoding encoding)
        {
        reEval:
            switch (encoding)
            {
                case StringEncoding.Unicode:
                    this.intPtr = Marshal.StringToHGlobalUni(str);
                    break;
                case StringEncoding.Ansi:
                    this.intPtr = Marshal.StringToHGlobalAnsi(str);
                    break;
                default:
                    encoding = MirandaEnvironment.MirandaStringEncoding;

                    if (encoding == StringEncoding.MirandaDefault) 
                        throw new ArgumentException(TextResources.ExceptionMsg_CannotDetectMirandaDefaultStringEncoding);

                    goto reEval;
            }

            this.originalString = str;
            this.encoding = encoding;
        }

        #endregion

        #region Properties

        public string OriginalString
        {
            get { return originalString; }
        }

        public StringEncoding Encoding
        {
            get
            {
                return encoding;
            }
        }

        public IntPtr IntPtr
        {
            get { return intPtr; }
        }

        public static implicit operator IntPtr(UnmanagedStringHandle operand)
        {
            return operand.IntPtr;
        }

        [CLSCompliant(false)]
        public static implicit operator UIntPtr(UnmanagedStringHandle operand)
        {
            return Translate.ToHandle(operand.IntPtr);
        }

        public int Size
        {
            get
            {
                if (!IsValid)
                    return 0;

                switch (encoding)
                {
                    case StringEncoding.Ansi:
                        return System.Text.Encoding.Default.GetByteCount(originalString);
                    case StringEncoding.Unicode:
                        return System.Text.Encoding.Unicode.GetByteCount(originalString);
                    default:
                        return -1;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return (intPtr != IntPtr.Zero);
            }
        }

        #endregion

        #region Methods

        public void Free()
        {
            if (IsValid)
            {
                Marshal.FreeHGlobal(intPtr);
                intPtr = IntPtr.Zero;
            }
        }

        void IDisposable.Dispose()
        {
            Free();
        }

        #endregion
    }
}
