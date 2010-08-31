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
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace Virtuoso.Miranda.Plugins.Native
{
    internal sealed class IconImageCache
    {
        #region Fields

        private static IconImageCache singleton;

        private readonly Dictionary<IntPtr, Icon> IconCache;
        private readonly Dictionary<int, Icon> StreamedIconCache;
        private readonly Dictionary<Icon, Image> ImageCache;

        #endregion

        #region .ctors

        [MethodImpl(MethodImplOptions.Synchronized)]
        private IconImageCache()
        {
            IconCache = new Dictionary<IntPtr, Icon>(1);
            StreamedIconCache = new Dictionary<int, Icon>(1);
            ImageCache = new Dictionary<Icon, Image>(1);
        }

        #endregion

        #region Properties
        
        public static IconImageCache Singleton
        {
            get
            {
                return singleton ?? (singleton = new IconImageCache());
            }
        }

        #endregion

        #region GetXY methods
        
        public Icon GetIcon(IntPtr handle)
        {
            lock (IconCache)
            {
                Icon icon = null;
                IntPtr key = handle;

                if (IconCache.ContainsKey(key))
                    icon = IconCache[key];
                else
                    IconCache[key] = icon = Icon.FromHandle(handle);

                return icon;
            }
        }

        public Icon GetStreamedIcon(Stream stream)
        {
            lock (StreamedIconCache)
            {
                lock (IconCache)
                {
                    int streamHandle = ComputeStreamHandle(stream);

                    if (StreamedIconCache.ContainsKey(streamHandle))
                        return StreamedIconCache[streamHandle];

                    Icon icon = new Icon(stream);

                    StreamedIconCache[streamHandle] = icon;

                    if (!IconCache.ContainsKey(icon.Handle))
                        IconCache[icon.Handle] = icon;

                    return icon;
                }
            }
        }

        private static int ComputeStreamHandle(Stream stream)
        {
            lock (stream)
            {
                int streamHandle = 0, result;

                long prevPosition = stream.Position;
                stream.Position = 0;

                while ((result = stream.ReadByte()) != -1)
                    streamHandle += (byte)result;

                stream.Position = prevPosition;
                return streamHandle;
            }
        }

        public Image GetIconImage(IntPtr handle)
        {
            Image image = null;
            Icon icon = GetIcon(handle);

            lock (ImageCache)
            {
                if (ImageCache.ContainsKey(icon))
                    image = ImageCache[icon];
                else
                    ImageCache[icon] = image = icon.ToBitmap();

                return image;
            }
        }

        public bool IsCached(IntPtr handle)
        {
            lock (IconCache)
                return IconCache.ContainsKey(handle);
        }

        public bool IsCached(Stream stream)
        {
            lock (StreamedIconCache)
                return StreamedIconCache.ContainsKey(ComputeStreamHandle(stream));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            if (disposing)
            {
                lock (IconCache)
                {
                    lock (ImageCache)
                    {
                        lock (StreamedIconCache)
                        {
                            foreach (Icon icon in IconCache.Values)
                                icon.Dispose();

                            foreach (Icon icon in StreamedIconCache.Values)
                                icon.Dispose();

                            foreach (Image image in ImageCache.Values)
                                image.Dispose();

                            IconCache.Clear();
                            StreamedIconCache.Clear();
                            ImageCache.Clear();

                            singleton = null;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
