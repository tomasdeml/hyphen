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

namespace Virtuoso.Hyphen.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal sealed class PLUGININFOEX : PLUGININFO
    {
        public Guid UUID;

        public PLUGININFOEX() { }

        public PLUGININFOEX(Guid uuid)
        {
            this.UUID = uuid;
        }
    }
}

/*
 typedef struct {
	int cbSize;
	char *shortName;
	DWORD version;
	char *description;
	char *author;
	char *authorEmail;
	char *copyright;
	char *homepage;
	BYTE isTransient;
	int replacesDefaultModule;
	MUUID uuid;
} PLUGININFOEX;
 */
