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

namespace Virtuoso.Miranda.Plugins.Infrastructure.Protocols
{
    public enum AckType : int
    {
        Message = 0,
        Url = 1,
        File = 2,
        Chat = 3,
        AwayMessage = 4,
        AuthorizationRequest = 5,
        Added = 6,
        GetInfo = 7,
        SetInfo = 8,
        Login = 9,
        Search = 10,
        NewUser = 11,
        Status = 12,
        Contacts = 13,	//send/recv of contacts
        Avatar = 14, //send/recv of avatars from a protocol            
    }

    public enum AckResult : int
    {
        Success = 0,
        Failure = 1,
    }
}
