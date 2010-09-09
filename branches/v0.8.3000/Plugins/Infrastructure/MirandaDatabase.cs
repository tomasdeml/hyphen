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
using Virtuoso.Miranda.Plugins;
using System.Runtime.CompilerServices;
using Virtuoso.Miranda.Plugins.Native;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Resources;
using System.Threading;
using System.Collections.ObjectModel;
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Helpers;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public sealed class MirandaDatabase : EventPublisher
    {
        #region Constants

        private const string ME_DB_EVENT_ADDED = "DB/Event/Added",
                            ME_DB_EVENT_DELETED = "DB/Event/Deleted",
                            ME_DB_EVENT_FILTER_ADD = "DB/Event/FilterAdd",
                            ME_DB_CONTACT_ADDED = "DB/Contact/Added",
                            ME_DB_CONTACT_DELETED = "DB/Contact/Deleted",
                            ME_DB_CONTACT_SETTINGCHANGED = "DB/Contact/SettingChanged";

        private const string MS_DB_GETPROFILENAME = "DB/GetProfileName",
                            MS_DB_GETPROFILEPATH = "DB/GetProfilePath",
                            MS_DB_EVENT_ADD = "DB/Event/Add";

        private const string MS_DB_CONTACT_GETCOUNT = "DB/Contact/GetCount",
                            MS_DB_CONTACT_FINDFIRST = "DB/Contact/FindFirst",
                            MS_DB_CONTACT_FINDNEXT = "DB/Contact/FindNext";

        private const string MS_DB_EVENT_FINDFIRST = "DB/Event/FindFirst",
                             MS_DB_EVENT_FINDNEXT = "DB/Event/FindNext";

        #endregion

        #region .ctors

        internal MirandaDatabase() { }

        #endregion

        #region Event handlers

        private MirandaEventHandler<MirandaDatabaseEventArgs> EventAddedEventHandler,
            EventDeletedEventHandler, BeforeEventAddedEventHandler;

        private MirandaEventHandler<MirandaContactEventArgs> ContactAddedEventHandler,
            ContactDeletedEventHandler;

        private MirandaEventHandler<MirandaContactSettingEventArgs> ContactSettingChangedEventHandler;

        #endregion

        #region Events & Triggers

        private int RaiseDbEvent(MirandaEventHandler<MirandaDatabaseEventArgs> handler, bool fromPointer, UIntPtr wParam, IntPtr lParam)
        {
            if (handler == null)
                return 0;

            ContactInfo contactInfo = ContactInfo.FromHandle(wParam);
            DatabaseEventInfo eventInfo = fromPointer ? DatabaseEventInfo.FromPointer(lParam) : DatabaseEventInfo.FromHandle(lParam);
            MirandaDatabaseEventArgs eventArgs = new MirandaDatabaseEventArgs(contactInfo, eventInfo);

            bool retValue = InvokeChainCancelable<MirandaDatabaseEventArgs>(handler, eventArgs);
            return Convert.ToInt32(retValue);
        }

        public event MirandaEventHandler<MirandaDatabaseEventArgs> EventAdded
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<MirandaDatabaseEventArgs>>(ref EventAddedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_DB_EVENT_ADDED,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        return RaiseDbEvent(EventAddedEventHandler, false, wParam, lParam);
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<MirandaDatabaseEventArgs>>(ref EventAddedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_DB_EVENT_ADDED, EventAddedEventHandler);
            }
        }

        public event MirandaEventHandler<MirandaDatabaseEventArgs> EventDeleted
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<MirandaDatabaseEventArgs>>(ref EventDeletedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_DB_EVENT_DELETED,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        return RaiseDbEvent(EventDeletedEventHandler, false, wParam, lParam);
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<MirandaDatabaseEventArgs>>(ref EventDeletedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_DB_EVENT_DELETED, EventDeletedEventHandler);
            }
        }

        /// <summary>
        /// Return TRUE to filter out the event, FALSE to pass the message along. 
        /// </summary>
        public event MirandaEventHandler<MirandaDatabaseEventArgs> BeforeEventAdded
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<MirandaDatabaseEventArgs>>(ref BeforeEventAddedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_DB_EVENT_FILTER_ADD,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        return RaiseDbEvent(BeforeEventAddedEventHandler, true, wParam, lParam);
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<MirandaDatabaseEventArgs>>(ref BeforeEventAddedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_DB_EVENT_FILTER_ADD, BeforeEventAddedEventHandler);
            }
        }

        private int RaiseContactEvent(MirandaEventHandler<MirandaContactEventArgs> handler, UIntPtr wParam)
        {
            if (handler == null)
                return 0;

            ContactInfo contactInfo = GetContactInfo(wParam);
            MirandaContactEventArgs eventArgs = new MirandaContactEventArgs(contactInfo);

            bool retValue = InvokeChainCancelable<MirandaContactEventArgs>(handler, eventArgs);
            return Convert.ToInt32(retValue);
        }

        private static ContactInfo GetContactInfo(UIntPtr wParam)
        {
            if (wParam == UIntPtr.Zero)
                return ContactInfo.MeNeutral;
            else
                return ContactInfo.FromHandle(wParam);
        }

        private unsafe int RaiseContactSettingEvent(UIntPtr hContact, IntPtr pDbWriteSetting)
        {
            DBCONTACTWRITESETTING dbWriteSetting = *(DBCONTACTWRITESETTING*)pDbWriteSetting.ToPointer();
            ContactInfo contactInfo = GetContactInfo(hContact);

            string name = Translate.ToString(dbWriteSetting.Name, StringEncoding.Ansi);
            string moduleName = Translate.ToString(dbWriteSetting.Module, StringEncoding.Ansi);
            object value = null;

            if ((DatabaseSettingType)dbWriteSetting.Value.Type != DatabaseSettingType.Blob)
                value = DBCONTACTWRITESETTING.ExtractValue(pDbWriteSetting);
            else
                Debugger.Log(10, Loader.LogCategory, "Blob settings are not yet supported, the value will be null.");

            MirandaContactSettingEventArgs eventArgs = new MirandaContactSettingEventArgs(contactInfo, name, moduleName, value, (DatabaseSettingType)dbWriteSetting.Value.Type);

            bool retValue = InvokeChainCancelable<MirandaContactSettingEventArgs>(ContactSettingChangedEventHandler, eventArgs);
            return Convert.ToInt32(retValue);
        }

        public event MirandaEventHandler<MirandaContactEventArgs> ContactAdded
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<MirandaContactEventArgs>>(ref ContactAddedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_DB_CONTACT_ADDED,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        return RaiseContactEvent(ContactAddedEventHandler, wParam);
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<MirandaContactEventArgs>>(ref ContactAddedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_DB_CONTACT_ADDED, ContactAddedEventHandler);
            }
        }

        public event MirandaEventHandler<MirandaContactEventArgs> ContactDeleted
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<MirandaContactEventArgs>>(ref ContactDeletedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_DB_CONTACT_DELETED,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        return RaiseContactEvent(ContactDeletedEventHandler, wParam);
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<MirandaContactEventArgs>>(ref ContactDeletedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_DB_CONTACT_DELETED, ContactDeletedEventHandler);
            }
        }

        public event MirandaEventHandler<MirandaContactSettingEventArgs> ContactSettingChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<MirandaContactSettingEventArgs>>(ref ContactSettingChangedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_DB_CONTACT_SETTINGCHANGED,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        return RaiseContactSettingEvent(wParam, lParam);
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<MirandaContactSettingEventArgs>>(ref ContactSettingChangedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_DB_CONTACT_SETTINGCHANGED, ContactSettingChangedEventHandler);
            }
        }

        #endregion

        #region Properties

        #region Profile

        public string ProfileName
        {
            get
            {
                InteropBuffer buffer = InteropBufferPool.AcquireBuffer();

                try
                {
                    buffer.Lock();

                    int result = MirandaContext.Current.CallService(MS_DB_GETPROFILENAME, buffer.SizeAsUIntPtr, buffer.IntPtr);
                    Debug.Assert(result == 0);

                    if (result != 0) return null;
                    return Translate.ToString(buffer.IntPtr, StringEncoding.Ansi);
                }
                catch (Exception e)
                {
                    throw new MirandaException(TextResources.ExceptionMsg_ErrorWhileCallingMirandaService, e);
                }
                finally
                {
                    buffer.Unlock();
                    InteropBufferPool.ReleaseBuffer(buffer);
                }
            }
        }

        public string ProfilePath
        {
            get
            {
                InteropBuffer buffer = InteropBufferPool.AcquireBuffer();

                try
                {
                    buffer.Lock();

                    int result = MirandaContext.Current.CallService(MS_DB_GETPROFILEPATH, buffer.SizeAsUIntPtr, buffer.IntPtr);
                    Debug.Assert(result == 0);

                    if (result != 0) return null;
                    return Translate.ToString(buffer.IntPtr, StringEncoding.Ansi);
                }
                catch (Exception e)
                {
                    throw new MirandaException(TextResources.ExceptionMsg_ErrorWhileCallingMirandaService, e);
                }
                finally
                {
                    buffer.Unlock();
                    InteropBufferPool.ReleaseBuffer(buffer);
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Contacts

        /// <summary>
        /// Enumerates contact handles, excluding the Me contact.
        /// </summary>
        /// <returns>Contact handles.</returns>
        public IEnumerable<IntPtr> GetContactHandles()
        {
            MirandaContext context = MirandaContext.Current;
            Callback findNext = ServiceManager.GetService(MS_DB_CONTACT_FINDNEXT);

            UIntPtr handle = (UIntPtr)(uint)context.CallService(MS_DB_CONTACT_FINDFIRST);

            do
            {
                if (handle != UIntPtr.Zero)
                    yield return Translate.ToHandle(handle);
            }
            while ((handle = (UIntPtr)(uint)findNext(handle, IntPtr.Zero)) != UIntPtr.Zero);
        }

        public ReadOnlyCollection<ContactInfo> GetContacts()
        {
            return GetContacts(false);
        }

        public ReadOnlyCollection<ContactInfo> GetContacts(bool includeSelf)
        {
            MirandaContext context = MirandaContext.Current;
            Callback findNext = ServiceManager.GetService(MS_DB_CONTACT_FINDNEXT);

            List<ContactInfo> contacts = new List<ContactInfo>(context.CallService(MS_DB_CONTACT_GETCOUNT));

            if (includeSelf)
                contacts.Add(ContactInfo.MeNeutral);

            foreach (IntPtr handle in GetContactHandles())
                contacts.Add(ContactInfo.FromHandle(handle));

            return contacts.AsReadOnly();
        }

        public ContactInfo FindContact(string uuid)
        {
            return FindContact(uuid, ContactInfoProperty.UniqueID, StringEncoding.Ansi);
        }

        public ContactInfo FindContact(string searchValue, ContactInfoProperty searchCriterion, StringEncoding valueEncoding)
        {
            return FindContact(searchValue, searchCriterion, valueEncoding, StringComparison.Ordinal);
        }

        public ContactInfo FindContact(string searchValue, ContactInfoProperty searchCriterion, StringEncoding valueEncoding, StringComparison comparisonType)
        {
            if (searchValue == null)
                throw new ArgumentNullException("searchValues");

            foreach (IntPtr handle in GetContactHandles())
            {
                object value;
                ContactInfoPropertyType type;

                if (ContactInfo.GetProperty(handle, searchCriterion, out value, out type)
                    && searchValue.Equals(value.ToString(), comparisonType))
                    return ContactInfo.FromHandle(handle);
            }

            Debug.Assert(false);
            return null;
        }

        public ContactInfo[] FindContacts(params string[] uuids)
        {
            if (uuids == null)
                throw new ArgumentNullException("uuids");

            List<ContactInfo> results = new List<ContactInfo>(uuids.Length);

            foreach (string uuid in uuids)
            {
                ContactInfo contact = FindContact(uuid, ContactInfoProperty.UniqueID, StringEncoding.Ansi);

                if (contact != null)
                    results.Add(contact);
            }

            return results.ToArray();
        }

        #endregion

        #region Events

        public IntPtr AddEvent(ContactInfo associatedContact, object data, ISettingOwner owner, DatabaseEventType type, DatabaseEventProperties flags, DateTime? timestamp)
        {
            return AddEvent(associatedContact, data, owner, type, flags, timestamp, true);
        }

        public IntPtr AddEvent(ContactInfo associatedContact, object data, ISettingOwner owner, DatabaseEventType type, DatabaseEventProperties flags, DateTime? timestamp, bool throwOnFailure)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return AddEvent(associatedContact, data, owner.Name, type, flags, timestamp, throwOnFailure);
        }

        public IntPtr AddEvent(ContactInfo associatedContact, object data, string owner, DatabaseEventType type, DatabaseEventProperties flags, DateTime? timestamp, bool throwOnFailure)
        {
            if (associatedContact == null)
                throw new ArgumentNullException("associatedContact");

            if (String.IsNullOrEmpty(owner))
                throw new ArgumentNullException("owner");

            if (data == null)
                throw new ArgumentNullException("data");

            IntPtr pBlob = IntPtr.Zero;
            UnmanagedStructHandle<DBEVENTINFO> nativeStruct = UnmanagedStructHandle<DBEVENTINFO>.Empty;

            try
            {
                int totalBytes;

                if (data is string)
                {
                    totalBytes = DBEVENTINFO.LayoutAnsiUniString((string)data, out pBlob);
                }
                else if (data is byte[])
                {
                    byte[] dataBytes = (byte[])data;
                    totalBytes = dataBytes.Length;

                    pBlob = Marshal.AllocHGlobal(totalBytes);
                    Marshal.Copy(dataBytes, 0, pBlob, dataBytes.Length);
                }
                else
                    throw new ArgumentOutOfRangeException("data");

                DBEVENTINFO info = new DBEVENTINFO(0, IntPtr.Zero);
                info.Module = Translate.ToHandle(owner, StringEncoding.Ansi).IntPtr;
                info.BlobSize = (uint)totalBytes;
                info.BlobPtr = pBlob;
                info.EventType = (ushort)type;
                info.Flags = (uint)flags;
                info.Timestamp = Utilities.GetTimestamp(timestamp.HasValue ? timestamp.Value : DateTime.Now);

                nativeStruct = new UnmanagedStructHandle<DBEVENTINFO>(ref info, pBlob, info.Module);
                IntPtr eventHandle = (IntPtr)MirandaContext.Current.CallService(MS_DB_EVENT_ADD, associatedContact.MirandaHandle, nativeStruct.IntPtr);

                if (eventHandle == IntPtr.Zero && throwOnFailure)
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_DB_EVENT_ADD, eventHandle.ToString()));
                else
                    return eventHandle;
            }
            finally
            {
                nativeStruct.Free();
            }
        }

        public IEnumerable<IntPtr> GetEventHandles(ContactInfo owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return GetEventHandles(owner.MirandaHandle);
        }

        public IEnumerable<IntPtr> GetEventHandles(IntPtr ownerHandle)
        {
            Callback findNext = ServiceManager.GetService(MS_DB_EVENT_FINDNEXT);
            IntPtr pEvent = (IntPtr)MirandaContext.Current.CallService(MS_DB_EVENT_FINDFIRST, ownerHandle, IntPtr.Zero);

            while (pEvent != IntPtr.Zero)
            {
                yield return pEvent;
                pEvent = (IntPtr)findNext(Translate.ToHandle(pEvent), IntPtr.Zero);
            }
        }

        #endregion

        #endregion
    }
}
