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
using Virtuoso.Miranda.Plugins.Helpers;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public enum StatusMode : int
    {        
        Offline = 40071,
        Online = 40072,
        Away = 40073,
        DND = 40074,
        NA = 40075,
        Occupied = 40076,

        [EnumValueFriendlyName("Free for chat")]
        FreeForChat = 40077,
        Invisible = 40078,

        [EnumValueFriendlyName("On the phone")]
        OnThePhone = 40079,

        [EnumValueFriendlyName("Out to lunch")]
        OutToLunch = 40080,

        Idle = 40081 /* do not use as a status */
    }
}
