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
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    internal struct MIRANDASYSTRAYNOTIFY
    {
        #region Fields

        private readonly int Size;

        public string Protocol;
        public string Title;
        public string Text;
        public uint Flags;
        public uint Timeout;

        #endregion

        #region .ctors

        public MIRANDASYSTRAYNOTIFY(string title, string text, System.Windows.Forms.ToolTipIcon flags)
        {
            this.Protocol = String.Empty;
            this.Title = title;
            this.Text = text;
            this.Flags = (uint)flags;
            this.Timeout = 10 * 1000;

            this.Size = Marshal.SizeOf(typeof(MIRANDASYSTRAYNOTIFY));
        }

        #endregion
    }

    /*typedef struct {
	int cbSize;			// sizeof(MIRANDASYSTRAY)
	char *szProto;		// protocol to show under (may have no effect)
	char *szInfoTitle;	// only 64chars of it will be used
	char *szInfo;		// only 256chars of it will be used
	DWORD dwInfoFlags;	// see NIIF_* stuff
	UINT uTimeout;		// how long to show the tip for
} MIRANDASYSTRAYNOTIFY;*/
}
