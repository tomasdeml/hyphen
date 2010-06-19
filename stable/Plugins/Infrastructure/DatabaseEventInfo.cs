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
using System.Diagnostics;
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    /// <summary>
    /// Represents database event information.
    /// </summary>
    public class DatabaseEventInfo : IMirandaObject
    {
        #region Constants

        private const string MS_DB_EVENT_GETBLOBSIZE = "DB/Event/GetBlobSize";

        /// <summary>
        /// DB/Event/Get
        /// Retrieves all the information stored in hDbEvent
        ///  wParam=(WPARAM)(HANDLE)hDbEvent
        ///  lParam=(LPARAM)(DBEVENTINFO*)&amp;dbe
        /// hDbEvent should have been returned by db/event/add or db/event/find*event
        /// Returns 0 on success or nonzero if hDbEvent is invalid
        /// Don't forget to set dbe.cbSize, dbe.pBlob and dbe.cbBlob before calling this
        /// service
        /// The correct value dbe.cbBlob can be got using db/event/getblobsize
        /// If successful, all the fields of dbe are filled. dbe.cbBlob is set to the
        /// actual number of bytes retrieved and put in dbe.pBlob
        /// If dbe.cbBlob is too small, dbe.pBlob is filled up to the size of dbe.cbBlob
        /// and then dbe.cbBlob is set to the required size of data to go in dbe.pBlob
        /// On return, dbe.szModule is a pointer to the database module's own internal list
        /// of modules. Look but don't touch.
        /// </summary>
        private const string MS_DB_EVENT_GET = "DB/Event/Get";

        private const string MS_DB_TIME_TIMESTAMPTOSTRING = "DB/Time/TimestampToString";

        /// <summary>
        /// DB/Event/GetContact
        /// Retrieves a handle to the contact that owns hDbEvent.
        ///  wParam=(WPARAM)(HANDLE)hDbEvent
        ///  lParam=0
        /// hDbEvent should have been returned by db/event/add or db/event/find*event
        /// NULL is a valid return value, meaning, as usual, the user.
        /// Returns (HANDLE)(-1) if hDbEvent is invalid, or the handle to the contact on
        /// success
        /// This service is exceptionally slow. Use only when you have no other choice at
        /// all.
        /// </summary>
        private const string MS_DB_EVENT_GETCONTACT = "DB/Event/GetContact";

        /// <summary>
        /// DB/Event/GetText (0.7.0+)
        /// Retrieves the event's text
        /// wParam=(WPARAM)0 (unused)
        /// lParam=(LPARAM)(DBEVENTGETTEXT*)egt - pointer to structure with parameters
        /// egt->dbei should be the valid database event read via MS_DB_EVENT_GET
        /// egt->datatype = DBVT_WCHAR or DBVT_ASCIIZ or DBVT_TCHAR. If a caller wants to
        /// suppress Unicode part of event in answer, add DBVTF_DENYUNICODE to this field.
        /// egt->codepage is any valid codepage, CP_ACP by default.
        /// Function returns a pointer to a string in the required format.
        /// This string should be freed by a call of mir_free
        /// </summary>
        private const string MS_DB_EVENT_GETTEXT = "DB/Event/GetText";

        #endregion

        #region .ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEventInfo"/> class.
        /// </summary>
        protected DatabaseEventInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEventInfo"/> class.
        /// </summary>
        /// <param name="mirandaHandle">Event handle.</param>
        protected DatabaseEventInfo(IntPtr eventHandle)
        {
            if (eventHandle == IntPtr.Zero)
                throw new ArgumentNullException("eventHandle");

            this.mirandaHandle = eventHandle;
            FromHandle(eventHandle, out type, out flags, out data, out owningModule, out timestamp);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DatabaseEventInfo"/> from an event handle.
        /// </summary>
        /// <param name="mirandaHandle">Event handle.</param>
        /// <returns>Database event info.</returns>
        public static DatabaseEventInfo FromHandle(IntPtr eventHandle)
        {
            return new DatabaseEventInfo(eventHandle);
        }

        #endregion

        #region Initializers

        /// <summary>
        /// Gets the event information based on its handle.
        /// </summary>
        /// <param name="eventHandle">Event handle.</param>
        /// <param name="type">[OUT] Event type.</param>
        /// <param name="flags">[OUT] Event flags.</param>
        /// <param name="data">[OUT] Event data.</param>
        /// <param name="owningModule">[OUT] Event related module.</param>
        /// <param name="timestamp">[OUT] Event timestamp.</param>
        public static void FromHandle(IntPtr eventHandle, out DatabaseEventType type, out DatabaseEventProperties flags, out string data, out Protocol owningModule, out DateTime timestamp)
        {
            InteropBuffer buffer = null;

            try
            {
                unsafe
                {
                    DBEVENTINFO dbEventInfo;
                    PrepareDbEventInfo(eventHandle, out dbEventInfo, out buffer);

                    GetEventInfo(ref dbEventInfo, eventHandle, buffer, out type, out flags, out data, out owningModule, out timestamp);
                }
            }
            catch (MirandaException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(TextResources.ExceptionMsg_CannotFinishMarshaling, e);
            }
            finally
            {
                if (buffer != null)
                {
                    buffer.Unlock();
                    InteropBufferPool.ReleaseBuffer(buffer);
                }
            }
        }

        /// <summary>
        /// Prepares the <see cref="DBEVENTINFO"/> for information extraction and the blob buffer.
        /// </summary>
        /// <param name="eventHandle">Event handle.</param>
        /// <param name="dbEventInfo">[OUT] DB event info to marshal data into.</param>
        /// <param name="buffer">[OUT] Locked Blob buffer.</param>
        private unsafe static void PrepareDbEventInfo(IntPtr eventHandle, out DBEVENTINFO dbEventInfo, out InteropBuffer buffer)
        {
            int blobSize = MirandaContext.Current.CallServiceUnsafe(MS_DB_EVENT_GETBLOBSIZE, eventHandle.ToPointer(), null);

            if (blobSize == -1)
                throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_DB_EVENT_GETBLOBSIZE, blobSize.ToString()));

            // Acquire a buffer for the blob
            buffer = InteropBufferPool.AcquireBuffer(blobSize);
            buffer.Lock();

            dbEventInfo = new DBEVENTINFO(blobSize, buffer.IntPtr);
        }

        /// <summary>
        /// Initializes the instance by marshaling data from a <see cref="DBEVENTINFO"/> pointer.
        /// </summary>
        /// <param name="pDbEventInfo"><see cref="DBEVENTINFO"/> pointer.</param>
        private void MarshalEventInfo(IntPtr pDbEventInfo)
        {
            DBEVENTINFO info = (DBEVENTINFO)Marshal.PtrToStructure(pDbEventInfo, typeof(DBEVENTINFO));

            // Get an appropriately size buffer for the blob
            InteropBuffer buffer = InteropBufferPool.AcquireBuffer((int)info.BlobSize);

            try
            {
                buffer.Lock();
                GetEventInfo(ref info, IntPtr.Zero, buffer, out type, out flags, out data, out owningModule, out timestamp);
            }
            finally
            {
                buffer.Unlock();
                InteropBufferPool.ReleaseBuffer(buffer);
            }
        }

        /// <summary>
        /// Get the event information from a <see cref="DBEVENTINFO"/> struct.
        /// </summary>
        /// <param name="dbEventInfo">[REF] <see cref="DBEVENTINFO"/> struct.</param>
        /// <param name="mirandaHandle">Event handle (the blob buffer will be populated if not null).</param>
        /// <param name="blobBuffer">Buffer to use for blob marshaling.</param>
        /// <param name="type">[OUT] Event type.</param>
        /// <param name="flags">[OUT] Event flags.</param>
        /// <param name="data">[OUT] Event data.</param>
        /// <param name="owningModule">[OUT] Event related module.</param>
        /// <param name="timestamp">[OUT] Event timestamp.</param>
        private static void GetEventInfo(ref DBEVENTINFO dbEventInfo, IntPtr eventHandle, InteropBuffer blobBuffer, out DatabaseEventType type, out DatabaseEventProperties flags, out string data, out Protocol owningModule, out DateTime timestamp)
        {
            MirandaContext context = MirandaContext.Current;

            unsafe
            {
                // If the event handle is set, we probably want to populate the blob buffer...
                if (eventHandle != IntPtr.Zero)
                    PopulateBlobBuffer(ref dbEventInfo, eventHandle);

                type = (DatabaseEventType)dbEventInfo.EventType;
                flags = (DatabaseEventProperties)dbEventInfo.Flags;
                data = GetEventData(ref dbEventInfo);
            }

            owningModule = GetEventModule(ref dbEventInfo);
            GetEventTimestamp(ref dbEventInfo, blobBuffer, out timestamp);
        }

        /// <summary>
        /// Populates the blob buffer set by the <see cref="DBEVENTINFO"/> parameter. 
        /// </summary>
        /// <param name="dbEventInfo">[REF] <see cref="DBEVENTINFO"/> struct identifiing the buffer.</param>
        /// <param name="eventHandle">Event handle.</param>
        /// <exception cref="MirandaException">Buffer could bet populated.</exception>
        private unsafe static void PopulateBlobBuffer(ref DBEVENTINFO dbEventInfo, IntPtr eventHandle)
        {
            int result;

            fixed (void* pDbEventInfo = &dbEventInfo)
                result = MirandaContext.Current.CallServiceUnsafe(MS_DB_EVENT_GET, eventHandle.ToPointer(), pDbEventInfo);

            if (result != 0)
                throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_DB_EVENT_GET, result.ToString()));
        }

        /// <summary>
        /// Gets the event timestamp.
        /// </summary>
        /// <param name="dbEventInfo">[REF] <see cref="DBEVENTINFO"/> struct.</param>
        /// <param name="blobBuffer">Buffer to reuse.</param>
        /// <param name="timestamp">[OUT] Timestamp.</param>
        private static void GetEventTimestamp(ref DBEVENTINFO dbEventInfo, InteropBuffer blobBuffer, out DateTime timestamp)
        {
            try
            {
                DBTIMETOSTRING timeToString = new DBTIMETOSTRING("s D");
                timeToString.MaxBytes = blobBuffer.Size;
                timeToString.Output = blobBuffer.IntPtr;

                unsafe
                {
                    MirandaContext.Current.CallServiceUnsafe(MS_DB_TIME_TIMESTAMPTOSTRING, (void*)dbEventInfo.Timestamp, &timeToString);
                }

                timestamp = DateTime.Parse(Translate.ToString(timeToString.Output, StringEncoding.Ansi));
            }
            catch (FormatException)
            {
                timestamp = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets the event module.
        /// </summary>
        /// <param name="dbEventInfo">[REF] <see cref=">DBEVENTINFO"/> struct.</param>
        /// <returns>Event module.</returns>
        private static Protocol GetEventModule(ref DBEVENTINFO dbEventInfo)
        {
            Protocol owningModule;
            bool moduleFound = false;

            if (dbEventInfo.Module != IntPtr.Zero)
                moduleFound = MirandaContext.Current.Protocols.TryGetValue(Translate.ToString(dbEventInfo.Module, StringEncoding.Ansi), out owningModule);
            else
                owningModule = Protocol.UnknownProtocol;

            if (!moduleFound)
                owningModule = Protocol.UnknownProtocol;

            return owningModule;
        }

        /// <summary>
        /// Gets the event data.
        /// </summary>
        /// <param name="dbEventInfo">[REF] <see cref="DBEVENTINFO"/> struct.</param>
        /// <returns>Event data.</returns>
        private unsafe static string GetEventData(ref DBEVENTINFO dbEventInfo)
        {
            string data;

            DBEVENTGETTEXT dbGetText = new DBEVENTGETTEXT();
            dbGetText.Codepage = 0;
            dbGetText.DataType = (int)DatabaseSettingType.UnicodeString;

            IntPtr pText;

            fixed (void* pDbEventInfo = &dbEventInfo)
            {
                dbGetText.DbEventInfoPtr = new IntPtr(pDbEventInfo);
                pText = (IntPtr)MirandaContext.Current.CallServiceUnsafe(MS_DB_EVENT_GETTEXT, null, &dbGetText);
            }

            if (pText != IntPtr.Zero)
            {
                data = Translate.ToString(pText, StringEncoding.Unicode);
                MirandaContext.Current.MirandaMemoryManager.Free(pText);
            }
            else
                throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_DB_EVENT_GETTEXT, "null"));

            return data;
        }

        #endregion

        #region Properties

        private IntPtr mirandaHandle;
        public IntPtr MirandaHandle
        {
            get { return mirandaHandle; }
        }

        private Protocol owningModule;
        public Protocol OwningModule
        {
            get { return owningModule; }
        }

        private DateTime timestamp;
        public DateTime Timestamp
        {
            get { return timestamp; }
        }

        private DatabaseEventProperties flags;
        public DatabaseEventProperties Flags
        {
            get { return flags; }
        }

        private DatabaseEventType type;
        public DatabaseEventType Type
        {
            get { return type; }
        }

        private string data;
        public string Data
        {
            get { return data; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Marshals the <see cref="DatabaseEventInfo"/> from a <see cref="DBEVENTINFO"/> struct pointer.
        /// </summary>
        /// <param name="pDbEventInfo"><see cref="DBEVENTINFO"/> struct pointer.</param>
        /// <returns>Event info.</returns>
        internal static DatabaseEventInfo FromPointer(IntPtr pDbEventInfo)
        {
            DatabaseEventInfo info = new DatabaseEventInfo();
            info.MarshalEventInfo(pDbEventInfo);

            return info;
        }

        /// <summary>
        /// Gets the handle of the contact owning this event. This method is very slow, use wisely.
        /// </summary>
        /// <returns>Associated contact handle.</returns>
        public IntPtr GetContactHandle()
        {
            return GetContactHandle(mirandaHandle);
        }

        /// <summary>
        /// Gets the handle of the contact owning this event. This method is very slow, use wisely.
        /// </summary>
        /// <param name="eventHandle">Event handle to get the contact handle for.</param>
        /// <returns>Associated contact handle.</returns>
        public static IntPtr GetContactHandle(IntPtr eventHandle)
        {
            if (eventHandle == IntPtr.Zero)
                throw new ArgumentNullException("eventHandle");

            int contactHandle = MirandaContext.Current.CallService(MS_DB_EVENT_GETCONTACT, eventHandle, IntPtr.Zero);

            if (contactHandle == -1)
                throw new ArgumentException(TextResources.ExceptionMsg_InvalidHandle, "eventHandle");

            return (IntPtr)contactHandle;
        }

        #endregion
    }
}
