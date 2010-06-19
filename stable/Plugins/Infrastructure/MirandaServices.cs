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
using Virtuoso.Miranda.Plugins.Native;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    internal static class MirandaServices
    {
        public const string MS_CLIST_ADDMAINMENUITEM = "CList/AddMainMenuItem",
                            MS_CLIST_ADDCONTACTMENUITEM = "CList/AddContactMenuItem",
                            MS_CLIST_MODIFYMENUITEM = "CList/ModifyMenuItem",
                            MS_CLIST_SYSTRAY_NOTIFY = "Miranda/Systray/Notify",
                            MS_CONTACT_GETCONTACTINFO = "Miranda/Contact/GetContactInfo",

                            MS_PROTO_GETCONTACTBASEPROTO = "Proto/GetContactBaseProto",
                            MS_PROTO_ENUMPROTOCOLS = "Proto/EnumProtocols",
                            MS_PROTO_CALLCONTACTSERVICE = "Proto/CallContactService",
                            MS_PROTO_REGISTERMODULE = "Proto/RegisterModule",                           
                            PS_MESSAGE = "/SendMsg",
                            MS_SYSTEM_GETVERSION = "Miranda/System/GetVersion";
    }
}
