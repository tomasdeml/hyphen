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
using System.Runtime.CompilerServices;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Native
{
    // Impl note: should be static, but UnitTests' private accessors dislike that...
    [CLSCompliant(false)]
    public sealed class InteropBufferPool
    {
        #region Fields

        public const int DefaultMaximumAvailableBufferSize = 260;
        private const int DefaultBuffersCount = 2;

        private static int maximumAvailableBufferSize = DefaultMaximumAvailableBufferSize;

        public static int MaximumAvailableBufferSize
        {
            get { return InteropBufferPool.maximumAvailableBufferSize; }
        }       

        private static int NextAvailableBufferIndex;     
        private static InteropBuffer[] Buffers;   

        private static volatile bool Disposed;

        #endregion

        #region .ctors & .dctors

        private InteropBufferPool() { }

        #endregion

        #region Methods

        internal static void Dispose()
        {
            if (Disposed || Buffers == null) 
                return;

            lock (Buffers)
            {
                Disposed = true;

                for (int i = 0; i < Buffers.Length; i++)
                {
                    try
                    {
                        Buffers[i].Dispose(true);
                        Buffers[i] = null;
                    }
                    catch { }
                }
            }
        }

        internal static void Refresh()
        {
            Dispose();

            Buffers = null;
            Disposed = false;

            VerifyPoolConsistency();
        }

        private static void VerifyPoolConsistency()
        {
            if (Disposed) 
                throw new ObjectDisposedException("InteropBufferPool");

            if (Buffers == null)
            {
                NextAvailableBufferIndex = 0;
                maximumAvailableBufferSize = DefaultMaximumAvailableBufferSize;

                Buffers = new InteropBuffer[DefaultBuffersCount];

                for (int i = 0; i < DefaultBuffersCount; i++)
                    Buffers[i] = new InteropBuffer(maximumAvailableBufferSize);
            }
        }

        public static InteropBuffer AcquireBuffer()
        {
            return AcquireBuffer(maximumAvailableBufferSize);
        }

        public static InteropBuffer AcquireBuffer(int size)
        {
            if (size <= 0) 
                throw new ArgumentOutOfRangeException("size");

            VerifyPoolConsistency();

            lock (Buffers)
            {
                InteropBuffer buffer = null;

                if (NextAvailableBufferIndex > Buffers.Length - 1 || size > maximumAvailableBufferSize)
                    buffer = new InteropBuffer(size);
                else
                {
                    int index = Array.FindIndex<InteropBuffer>(Buffers, delegate(InteropBuffer _buffer)
                    {
                        if (_buffer.Size >= size)
                            return true;
                        else
                            return false;
                    });

                    if (index == -1 || index < NextAvailableBufferIndex)
                        buffer = new InteropBuffer(size);
                    else
                    {
                        buffer = Buffers[index];
                        NextAvailableBufferIndex++;
                    }
                }

                buffer.Reserved = true;
                return buffer;
            }
        }

        public static void ReleaseBuffer(InteropBuffer buffer)
        {
            if (buffer == null) 
                return;

            if (buffer.Locked) 
                throw new InvalidOperationException(TextResources.ExceptionMsg_InteropBufferNotUnlocked);

            if (!buffer.Reserved) 
                throw new ArgumentException();

            VerifyPoolConsistency();

            lock (Buffers)
            {
                if (Array.IndexOf(Buffers, buffer) == -1)
                {
                    if (buffer.Size > maximumAvailableBufferSize)
                    {
                        if (NextAvailableBufferIndex <= Buffers.Length - 1)
                        {
                            Buffers[NextAvailableBufferIndex].Dispose(true);
                            Buffers[NextAvailableBufferIndex] = buffer;

                            maximumAvailableBufferSize = buffer.Size;
                        }
                    }
                    else
                        buffer.Dispose(true);
                }
                else
                    NextAvailableBufferIndex--;

                buffer.Reserved = false;
            }
        }

        #endregion
    }
}
