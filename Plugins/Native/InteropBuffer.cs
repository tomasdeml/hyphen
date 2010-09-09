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
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Native
{
    // Every method touching a memory location or returning its address requires the buffer to be suspended!
    [CLSCompliant(false)]
    public sealed class InteropBuffer : IUnmanagedMemoryHandle
    {
        #region Fields

        private const string LogCategory = Loader.LogCategory + "::InteropBuffer";

        private readonly int size;
        private IntPtr sizeAsIntPtr;
        private UIntPtr sizeAsUIntPtr;

        private readonly object SyncObject;
        
        private IntPtr intPtr;
        private volatile int Owner;

        private volatile bool reserved;

        #endregion

        #region .ctors

        internal InteropBuffer(int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("size");

            SyncObject = new object();
            Log.DebuggerWrite(0, LogCategory, "InteropBuffer SyncObject initialized");

            this.size = size;

            intPtr = Marshal.AllocHGlobal(size);
            Log.DebuggerWrite(0, LogCategory, "InteropBuffer memory allocated (" + size + " B)");
        }

        ~InteropBuffer()
        {
            Dispose(false);
        }

        private void CheckLock()
        {
            if (Owner == 0) throw new InvalidOperationException(TextResources.ExceptionMsg_InteropBufferNotLocked);
        }

        #endregion   

        #region IUnmanagedMemoryHandle Members

        public IntPtr IntPtr
        {
            get
            {
                lock (SyncObject)
                {
                    // Touches memory, lock needed
                    CheckLock();

                    if (intPtr == IntPtr.Zero)
                        throw new ObjectDisposedException("InteropBuffer");

                    return intPtr;
                }
            }
        }
        
        void IUnmanagedMemoryHandle.Free()
        {
            lock (SyncObject)
            {
                CheckLock();
                Dispose(true);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((IUnmanagedMemoryHandle)this).Free();
        }

        internal void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            if (intPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(intPtr);
                intPtr = IntPtr.Zero;

                Log.DebuggerWrite(0, LogCategory, "InteropBuffer memory released");
            }
        }

        #endregion

        #region Properties

        public bool Locked
        {
            get
            {
                return Owner != 0;
            }
        }

        public int Size
        {
            get
            {
                return size;
            }
        }

        public IntPtr SizeAsIntPtr
        {
            get
            {
                if (sizeAsIntPtr == IntPtr.Zero)
                    sizeAsIntPtr = new IntPtr(size);

                return sizeAsIntPtr;
            }
        }

        public UIntPtr SizeAsUIntPtr
        {
            get
            {
                if (sizeAsUIntPtr == UIntPtr.Zero)
                    sizeAsUIntPtr = (UIntPtr)(ulong)size;

                return sizeAsUIntPtr;
            }
        }

        internal bool Reserved
        {
            get
            {
                return reserved;
            }
            set
            {
                reserved = value;
            }
        }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return intPtr.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            InteropBuffer other = obj as InteropBuffer;
            if (other == null) return false;

            return (GetHashCode() == other.GetHashCode());
        }

        public void Lock()
        {
            Log.DebuggerWrite(0, LogCategory, "Attempting to lock InteropBuffer for thread id " + Thread.CurrentThread.ManagedThreadId);

            Monitor.Enter(SyncObject);
            Owner = Thread.CurrentThread.ManagedThreadId;

            Log.DebuggerWrite(0, LogCategory, "InteropBuffer locked for thread id " + Owner);
        }

        public void Unlock()
        {
            Log.DebuggerWrite(0, LogCategory, "Attempting to unlock InteropBuffer of thread id " + Thread.CurrentThread.ManagedThreadId);
            CheckLock();

            if (Owner == Thread.CurrentThread.ManagedThreadId)
            {
                Monitor.Exit(SyncObject);

                Log.DebuggerWrite(0, LogCategory, "InteropBuffer unlocked by thread id " + this.Owner);
                Owner = 0;
            }
            else
                throw new InvalidOperationException(TextResources.ExceptionMsg_InvalidCrossThreadInteropBufferUnlock);
        }

        public void Zero()
        {
            lock (SyncObject)
            {
                // Touches memory, lock needed
                CheckLock();

                for (int i = 0; i < Size; i++)
                    Marshal.WriteByte(intPtr, i, 0);
            }
        }

        public static implicit operator IntPtr(InteropBuffer buffer)
        {
            return buffer.intPtr;
        }

        #endregion
    }
}
