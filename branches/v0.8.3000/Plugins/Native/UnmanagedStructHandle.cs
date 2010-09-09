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

namespace Virtuoso.Miranda.Plugins.Native
{
    public enum MarshalKind
    {
        Copy,
        PinBlittable
    }

    public struct UnmanagedStructHandle<T> : IUnmanagedMemoryHandle
    {
        #region Fields

        public readonly static UnmanagedStructHandle<T> Empty = new UnmanagedStructHandle<T>();

        private Type ActualType;
        private MarshalKind MarshalKind;

        private GCHandle GcHandle;
        private IntPtr intPtr;

        private IntPtr SinglePressure;
        private IntPtr[] Pressure;        

        #endregion

        #region .ctors

        public UnmanagedStructHandle(ref T structure)
            : this(ref structure, MarshalKind.Copy, null)
        { }

        public UnmanagedStructHandle(ref T structure, IntPtr pressure)
            : this(ref structure, MarshalKind.Copy, null)
        {
            this.SinglePressure = pressure;
        }

        public UnmanagedStructHandle(ref T structure, params IntPtr[] pressure)
            : this(ref structure, MarshalKind.Copy, pressure)
        { }

        public UnmanagedStructHandle(ref T structure, MarshalKind marshalKind)
            : this(ref structure, marshalKind, null)
        { }

        public UnmanagedStructHandle(ref T structure, MarshalKind marshalKind, IntPtr pressure)
            : this(ref structure, marshalKind, null)
        {
            this.SinglePressure = pressure;
        }

        public UnmanagedStructHandle(ref T structure, MarshalKind marshalKind, params IntPtr[] pressure)
        {
            // The structure is of a reference type and is null...
            if (!typeof(T).IsValueType && object.ReferenceEquals(structure, null))
                throw new ArgumentNullException("structure");

            this.SinglePressure = IntPtr.Zero;
            this.Pressure = pressure;
            this.MarshalKind = marshalKind;
            this.ActualType = structure.GetType();

            switch (marshalKind)
            {
                case MarshalKind.Copy:
                    this.intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ActualType));
                    Marshal.StructureToPtr((object)structure, this.intPtr, false);
                    this.GcHandle = new GCHandle();
                    break;
                case MarshalKind.PinBlittable:
                    this.GcHandle = GCHandle.Alloc((object)structure, GCHandleType.Pinned);
                    this.intPtr = this.GcHandle.AddrOfPinnedObject();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("marshalKind");
            }
        }

        #endregion

        #region Operator overloads

        public static implicit operator IntPtr(UnmanagedStructHandle<T> operand)
        {
            return operand.IntPtr;
        }

        [CLSCompliant(false)]
        public static implicit operator UIntPtr(UnmanagedStructHandle<T> operand)
        {
            return Translate.ToHandle(operand.IntPtr);
        }

        #endregion

        #region Properties

        public IntPtr IntPtr
        {
            get
            {
                return intPtr;
            }
        }

        #endregion

        #region Methods

        public void MarshalBack(out T destination)
        {
            if (MarshalKind == MarshalKind.PinBlittable)
                destination = (T)GcHandle.Target;
            else
                destination = (T)Marshal.PtrToStructure(IntPtr, ActualType);
        }

        public override int GetHashCode()
        {
            return intPtr.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UnmanagedStructHandle<T>)) 
                return false;

            return Equals((UnmanagedStructHandle<T>)obj);
        }

        public bool Equals(UnmanagedStructHandle<T> other)
        {
            return (other.intPtr == this.intPtr && other.Pressure == this.Pressure && other.SinglePressure == this.SinglePressure);
        }

        #endregion

        void IDisposable.Dispose()
        {
            Free();
        }

        public void Free()
        {
            try
            {
                if (IntPtr != IntPtr.Zero)
                {
                    FreePressure();

                    if (MarshalKind == MarshalKind.PinBlittable)
                        GcHandle.Free();
                    else
                        Marshal.DestroyStructure(IntPtr, ActualType);

                    intPtr = IntPtr.Zero;
                    ActualType = null;
                }
            }
            catch (Exception e)
            {
                Log.DebuggerWrite(100, GetType().FullName, "Unable to free a struct handle: " + e.ToString());
            }
        }

        private void FreePressure()
        {
            if (SinglePressure != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(SinglePressure);
                SinglePressure = IntPtr.Zero;
            }

            if (Pressure != null)
            {
                for (int i = 0; i < Pressure.Length; i++)
                {
                    IntPtr ptr = Pressure[i];

                    if (ptr != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptr);
                }

                Pressure = null;
            }
        }
    }
}
