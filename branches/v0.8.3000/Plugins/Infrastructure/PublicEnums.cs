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
    public enum DatabaseSettingType : byte
    {
        Deleted = 0,    //this setting just got deleted, no other values are valid
        Byte = 1,	  //bVal and cVal are valid
        UInt16 = 2,	  //wVal and sVal are valid
        UInt32 = 4,	  //dVal and lVal are valid
        AsciiString = 255,	  //pszVal is valid
        Blob = 254,	  //cpbVal and pbVal are valid
        UTF8String = 253,   //pszVal is valid
        UnicodeString = 252,   //pszVal is valid
    }

    public enum ProtocolType : int
    {
        Ignore = 50,  // added during v0.3.3
        Protocol = 1000,
        Encryption = 2000,
        Filter = 3000,
        Translation = 4000,
        Other = 10000   //avoid using this if at all possible
    }

    internal enum ProtocolFlagsKind : int
    {
        /// <summary>
        /// The network capabilities that the protocol supports.
        /// </summary>
        Capabilities = 1,

        /// <summary>
        /// The status modes that the protocol supports.
        /// </summary>
        StatusModes = 2,

        /// <summary>
        /// The status modes that the protocol supports away-style messages for. Uses the <see cref="StatusModes"/> flags.
        /// </summary>
        AwayStatusModes = 3
    }

    [Flags, CLSCompliant(false)]
    public enum ProtocolCapabilities : uint
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Supports IM sending.
        /// </summary>
        IMSend = 0x00000001,
        /// <summary>
        /// Supports IM receiving.
        /// </summary>
        IMReceive = 0x00000002,
        IM = (IMSend | IMReceive),
        /// <summary>
        /// Aupports separate URL sending.
        /// </summary>
        UrlSend = 0x00000004,
        /// <summary>
        /// Supports separate URL receiving.
        /// </summary>
        UrlReceive = 0x00000008,
        Url = (UrlSend | UrlReceive),
        /// <summary>
        /// Supports file sending.
        /// </summary>
        FileSend = 0x00000010,
        /// <summary>
        /// Supports file receiving.
        /// </summary>
        FileReceive = 0x00000020,
        File = (FileSend | FileReceive),
        /// <summary>
        /// Supports broadcasting away messages.
        /// </summary>
        ModeMessageSend = 0x00000040,
        /// <summary>
        /// Supports reading others' away messages.
        /// </summary>
        ModeMessageReceive = 0x00000080,
        ModeMessage = (ModeMessageSend | ModeMessageReceive),
        /// <summary>
        /// Contact lists are stored on the server, not locally. See notes below.
        /// </summary>
        ServerContactList = 0x00000100,
        /// <summary>
        /// Will get authorisation requests for some or all contacts.
        /// </summary>
        AuthorizationRequired = 0x00000200,
        /// <summary>
        /// Will get 'you were added' notifications.
        /// </summary>
        Added = 0x00000400,
        /// <summary>
        /// Has an invisible list.
        /// </summary>
        VisibleList = 0x00000800,
        /// <summary>
        /// Has a visible list for when in invisible mode.
        /// </summary>
        InvisibleList = 0x00001000,
        /// <summary>
        /// Supports setting different status modes to each contact.
        /// </summary>
        IndividualStatus = 0x00002000,
        /// <summary>
        /// the protocol is extensible and Supports plugin-defined messages.
        /// </summary>
        Extensible = 0x00004000,
        /// <summary>
        /// Supports direct (not server mediated) communication between clients.
        /// </summary>
        P2P = 0x00008000,
        /// <summary>
        /// Supports creation of new user IDs.
        /// </summary>
        NewUser = 0x00010000,
        /// <summary>
        /// Has a realtime chat capability.
        /// </summary>
        Chat = 0x00020000,
        /// <summary>
        /// Supports replying to a mode message request with different text depending on the contact requesting.
        /// </summary>
        IndividualModeMessage = 0x00040000,
        /// <summary>
        /// Supports a basic user searching facility.
        /// </summary>
        BasicSearch = 0x00080000,
        /// <summary>
        /// Supports one or more protocol-specific extended search schemes.
        /// </summary>
        ExtendedSearch = 0x00100000,
        /// <summary>
        /// Supports renaming of incoming files as they are transferred.
        /// </summary>
        CanRenameFile = 0x00200000,
        /// <summary>
        /// Can resume broken file transfers.
        /// </summary>
        FileResume = 0x00400000,
        /// <summary>
        /// Can add search results to the contact list.
        /// </summary>
        AddSearches = 0x00800000,
        /// <summary>
        /// Can send contacts to other users.
        /// </summary>
        ContactSend = 0x01000000,
        /// <summary>
        /// Can receive contacts from other users.
        /// </summary>
        ContactReceive = 0x02000000,
        Contact = (ContactSend | ContactReceive),
        /// <summary>
        /// Can change our user information stored on server.
        /// </summary>
        ChangeInfo = 0x04000000,
        /// <summary>
        /// Supports a search by e-mail feature.
        /// </summary>
        SearchByEmail = 0x08000000,
        /// <summary>
        /// Set if the uniquely identifying field of the network is the e-mail address.
        /// </summary>
        UserIDIsEmail = 0x10000000,
        /// <summary>
        /// Supports searching by nick/first/last names.
        /// </summary>
        SearchByName = 0x20000000,
        /// <summary>
        /// Has a dialog box to allow searching all the possible fields.
        /// </summary>
        ExtendedSearchUI = 0x40000000,
        /// <summary>
        /// The unique user IDs for this protocol are numeric.
        /// </summary>
        NumericUserID = 0x80000000,
    }

    [Flags]
    public enum ProtocolStatusModes : int
    {
        Online = 0x00000001,   //an unadorned online mode
        Invisible = 0x00000002,
        ShortAway = 0x00000004,   //Away on ICQ, BRB on MSN
        LongAway = 0x00000008,   //NA on ICQ, Away on MSN
        LightDND = 0x00000010,   //Occupied on ICQ, Busy on MSN
        HeavyDND = 0x00000020,   //DND on ICQ
        FreeForChat = 0x00000040,
        OutToLunch = 0x00000080,
        OnThePhone = 0x00000100,
        Idle = 0x00000200   //added during 0.3.4 (2004/09/13)
    }

    public enum DatabaseEventType : short
    {
        Message = 0,
        Url = 1,
        Contacts = 2,	//v0.1.2.2+
        Added = 1000,  //v0.1.1.0+: these used to be module-
        AuthorizationRequest = 1001,  //specific codes, hence the module-
        File = 1002,  //specific limit has been raised to 2000
    }

    [Flags]
    public enum DatabaseEventProperties : int
    {
        None = 0,

        First = 1,    //this is the first event in the chain;
        //internal only: *do not* use this flag
        Sent = 2,    //this event was sent by the user. If not set this
        //event was received.
        Read = 4,    //event has been read by the user. It does not need
        //to be processed any more except for history.

        Rtl = 8,    //event contains the right-to-left aligned text
        Utf8 = 16    //event contains a text in utf-8
    }

    [Flags]
    public enum MenuItemProperties : int
    {
        KeepCurrent = -1,
        None = 0,
        Grayed = 1,
        Checked = 2,
        Hidden = 4,
        OnlineOnly = 8,
        OfflineOnly = 16,
        NotOnListOnly = 32,
        NonNotOnListOnly = 64
    }

    [Flags]
    public enum HotKeys : int
    {
        Alt = 0x0001,
        Ctrl = 0x0002,
        Shift = 0x0004,
        WinKey = 0x0008
    }

    public enum ContactInfoPropertyType : int
    {
        Unknown = 0,
        Byte = 1,
        UInt16 = 2,
        UInt32 = 3,
        String = 4
    }

    public static class CallbackResult
    {
        public const int Success = 0;
        public const int Failure = -1;
    }

    public static class EventResult
    {
        public const bool HonourEventChain = false;
        public const bool BreakEventChain = true;
    }

    [Flags]
    public enum ContactListEventProperties : int
    {
        None = 0,

        /// <summary>
        /// Flashes the icon even if the user is occupied, and puts the event at the top of the queue.
        /// </summary>
        Urgent = 1,

        /// <summary>
        /// The icon will not flash for ever, only a few times. This is for eg online alert.
        /// </summary>
        Minor = 2,

        //    #define CLEF_URGENT    1	//flashes the icon even if the user is occupied,
        //                            //and puts the event at the top of the queue
        //#define CLEF_ONLYAFEW  2	//the icon will not flash for ever, only a few
        //                            //times. This is for eg online alert
        //#define CLEF_UNICODE   4	//set pszTooltip as unicode

        //#define CLEF_PROTOCOLGLOBAL   8		//set event globally for protocol, hContact has to be NULL, 
        //                                    //lpszProtocol the protocol ID name to be set

        //#if defined( _UNICODE )
        //    #define CLEF_TCHAR       CLEF_UNICODE      //will use TCHAR* instead of char*
        //#else
        //    #define CLEF_TCHAR       0      //will return char*, as usual
        //#endif
    }
}
