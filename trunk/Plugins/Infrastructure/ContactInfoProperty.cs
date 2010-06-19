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

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public enum ContactInfoProperty : byte
    {
        // Types of information you can retreive by setting the dwFlag in CONTACTINFO
        FirstName = 1,  // returns first eventName (string)
        LastName = 2,  // returns last eventName (string)
        Nick = 3,  // returns nick eventName (string)
        CustomNick = 4,  // returns custom nick name, clist name (string)
        Email = 5,  // returns email (string)
        City = 6,  // returns city (string)
        State = 7,  // returns state (string)
        Country = 8,  // returns country (string)
        Phone = 9, // returns phone (string)
        HomePage = 10, // returns homepage (string)
        About = 11, // returns about info (string)
        Gender = 12, // returns gender (byte,'M','F' character)
        Age = 13, // returns age (byte, 0==unspecified)
        FirstAndLastName = 14, // returns first eventName + last eventName (string)
        UniqueID = 15, // returns uniqueid, protocol username (must check type for type of return)
    }

    [Flags]
    internal enum ContactInfoPropertyFlags : byte
    {
        Unicode = 0x80,        
    }
}
