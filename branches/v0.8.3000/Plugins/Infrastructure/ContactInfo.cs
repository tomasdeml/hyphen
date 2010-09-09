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
using System.Runtime.InteropServices;
using System.Diagnostics;
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Resources;
using System.Runtime.Serialization;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [Serializable]
    public class ContactInfo : MirandaItem, ISerializable
    {
        #region Enums

        [Flags]
        private enum ContactDisplayNameOptions : int
        {
            /// <summary>
            /// Will return char*, as usual.
            /// </summary>
            Ansi = 0,

            /// <summary>
            /// Will never return the user's custom name.
            /// </summary>
            NoMyHandle = 1,

            /// <summary>
            /// Will return TCHAR* instead of char*.
            /// </summary>
            Unicode = 2,

            /// <summary>
            /// Will not use the cache.
            /// </summary>
            NoCache = 4
        }

        #endregion

        #region Contants

        private const string MS_DB_CONTACT_IS = "DB/Contact/Is",
                            MS_PROTO_GETCONTACTBASEPROTO = "Proto/GetContactBaseProto",
                            MS_DB_CONTACT_WRITESETTING = "DB/Contact/WriteSetting",
                            MS_DB_CONTACT_GETSETTING = "DB/Contact/GetSetting",
                            MS_DB_CONTACT_GETSETTINGSTATIC = "DB/Contact/GetSettingStatic",
                            MS_DB_CONTACT_DELETESETTING = "DB/Contact/DeleteSetting",

                            MS_DB_CONTACT_ADD = "DB/Contact/Add",
                            MS_DB_CONTACT_DELETE = "DB/Contact/Delete",

                            MS_CLIST_GETCONTACTDISPLAYNAME = "CList/GetContactDisplayName",

                            SETTING_STATUS = "Status",

                            MS_MSG_SENDMESSAGE = "SRMsg/SendCommand",
                            MS_MSG_SENDMESSAGE_2 = "SRMsg/LaunchMessageWindow";

        #endregion

        #region Fields

        private static readonly object SyncObject = new object();
        private static readonly ContactInfo meNeutral = new ContactInfo();

        private readonly Protocol owningModule;

        private object value;
        private ContactInfoPropertyType valueType;

        #endregion

        #region .ctors

        private ContactInfo()
            : base(IntPtr.Zero, ItemType.Contact)
        {
            this.owningModule = Protocol.UnknownProtocol;
        }

        protected ContactInfo(SerializationInfo info, StreamingContext context)
            : this((IntPtr)info.GetInt64("MirandaHandle"))
        {
        }

        [CLSCompliant(false), Obsolete("Will be removed in the future, use FromHandle(UIntPtr) instead.")]
        public ContactInfo(UIntPtr contactHandle) : this(Translate.ToHandle(contactHandle)) { }

        [Obsolete("Will be removed in the future, use FromHandle(IntPtr) instead.")]
        public ContactInfo(IntPtr contactHandle)
            : base(contactHandle, ItemType.Contact)
        {
            MirandaContext context = MirandaContext.Current;

            if (contactHandle != IntPtr.Zero && context.CallService(MS_DB_CONTACT_IS, contactHandle, IntPtr.Zero) == 0)
                throw new ArgumentException("Contact not found in Miranda database.");

            IntPtr protoNamePtr = GetModuleNamePtr(contactHandle);

            if (protoNamePtr != IntPtr.Zero)
            {
                string protoName = Translate.ToString(protoNamePtr, StringEncoding.Ansi);
                MirandaContext.Current.Protocols.TryGetValue(protoName, out owningModule);
            }

            if (owningModule == null)
            {
                owningModule = Protocol.UnknownProtocol;

                if (contactHandle != IntPtr.Zero)
                    Log.DebuggerWrite(5, Loader.LogCategory, "Unable to obtain contact's protocol");
            }

            this.MirandaHandle = contactHandle;
        }

        public static ContactInfo FromHandle(IntPtr contactHandle)
        {
            return new ContactInfo(contactHandle);
        }

        [CLSCompliant(false)]
        public static ContactInfo FromHandle(UIntPtr contactHandle)
        {
            return new ContactInfo(contactHandle);
        }

        #endregion

        #region Properties

        public ContactInfoPropertyType PropertyType
        {
            get { return valueType; }
        }

        public object PropertyValue
        {
            get { return this.value; }
        }

        public Protocol OwningModule
        {
            get { return owningModule; }
        }

        public bool IsSelf
        {
            get
            {
                return (MirandaHandle == IntPtr.Zero);
            }
        }

        public static ContactInfo MeNeutral
        {
            get
            {
                return meNeutral;
            }
        }

        public StatusMode? Status
        {
            get
            {
                if (String.IsNullOrEmpty(OwningModule.Name))
                    return null;

                object obj = ReadSetting(SETTING_STATUS, DatabaseSettingType.UInt16);

                if (obj != null)
                    return (StatusMode)(UInt16)obj; //(StatusMode)Enum.Parse(typeof(StatusMode), (Int16) obj);
                else
                    return null;
            }
        }

        public string DisplayName
        {
            get
            {
                return GetDisplayName(MirandaHandle);
            }
        }

        private object uniqueID;
        public object UniqueID
        {
            get
            {
                lock (SyncObject)
                {
                    if (uniqueID == null)
                    {
                        ContactInfoPropertyType type;
                        GetUniqueID(MirandaHandle, out uniqueID, out type);
                    }

                    return uniqueID;
                }
            }
        }

        #endregion

        #region Methods

        #region Settings/Write

        public bool WriteSetting(string name, object value, DatabaseSettingType saveAs)
        {
            return WriteSetting(name, OwningModule, value, saveAs);
        }

        public bool WriteSetting(string name, ISettingOwner owner, object value, DatabaseSettingType saveAs)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return WriteSetting(name, owner.Name, value, saveAs);
        }

        public bool WriteSettingAsBlob(string name, ISettingOwner owner, byte[] blob)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return WriteSettingAsBlob(name, owner.Name, blob);
        }

        public bool WriteSettingAsBlob(string name, string owner, byte[] blob)
        {
            if (blob == null)
                throw new ArgumentNullException("blob");

            throw new NotImplementedException();

            //return WriteSetting(name, owner, blob, DatabaseSettingType.Blob);
        }

        public unsafe bool WriteSetting(string name, string owner, object value, DatabaseSettingType saveAs)
        {
            if (String.IsNullOrEmpty(owner))
                throw new ArgumentNullException("owner");

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            UnmanagedStringHandle valueHandle = UnmanagedStringHandle.Empty;
            UnmanagedStringHandle nameHandle = Translate.ToHandle(name, StringEncoding.Ansi);
            UnmanagedStringHandle moduleNameHandle = Translate.ToHandle(owner, StringEncoding.Ansi);

            IntPtr blobPtr = IntPtr.Zero;

            try
            {
                DBCONTACTWRITESETTING dbSetting = new DBCONTACTWRITESETTING();
                dbSetting.Name = nameHandle;
                dbSetting.Module = moduleNameHandle;
                dbSetting.Value = new DBVARIANT();
                dbSetting.Value.Type = (byte)saveAs;

                switch (saveAs)
                {
                    case DatabaseSettingType.AsciiString:
                        valueHandle = new UnmanagedStringHandle(value.ToString(), StringEncoding.Ansi);
                        dbSetting.Value.Text.TextPtr = valueHandle.IntPtr;
                        break;
                    case DatabaseSettingType.UnicodeString:
                    case DatabaseSettingType.UTF8String:
                        valueHandle = new UnmanagedStringHandle(value.ToString(), StringEncoding.Unicode);
                        dbSetting.Value.Text.TextPtr = valueHandle.IntPtr;
                        break;
                    
                    case DatabaseSettingType.Byte:
                        dbSetting.Value.Primitives.Byte = Convert.ToByte(value);
                        break;
                    case DatabaseSettingType.UInt16:
                        dbSetting.Value.Primitives.Word = Convert.ToUInt16(value);
                        break;
                    case DatabaseSettingType.UInt32:
                        dbSetting.Value.Primitives.DWord = Convert.ToUInt32(value);
                        break;
                    case DatabaseSettingType.Blob:
                        throw new NotImplementedException();

                        /*byte[] blob = value as byte[];

                        if (blob == null)
                            throw new ArgumentException("value");

                        blobPtr = Marshal.AllocHGlobal(blob.Length);
                        Marshal.Copy(blob, 0, blobPtr, blob.Length);

                        dbSetting.Value.Blob.BlobPtr = blobPtr;
                        dbSetting.Value.Blob.Size = (UInt16)blob.Length;
                        break;*/
                    default:
                        throw new ArgumentOutOfRangeException("saveAs");
                }

                bool result = MirandaContext.Current.CallServiceUnsafe(MS_DB_CONTACT_WRITESETTING, MirandaHandle.ToPointer(), &dbSetting) == 0;
                Debug.Assert(result);

                return result;
            }
            catch (FormatException fE)
            {
                throw new ArgumentOutOfRangeException("value", fE);
            }
            catch (Exception e)
            {
                throw new MirandaException(TextResources.ExceptionMsg_ErrorWhileCallingMirandaService, e);
            }
            finally
            {
                valueHandle.Free();
                nameHandle.Free();
                moduleNameHandle.Free();

                if (blobPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(blobPtr);
            }
        }

        #endregion

        #region Settings/Read

        public object ReadSetting(string name, DatabaseSettingType readAs)
        {
            return ReadSetting(MirandaHandle, name, readAs);
        }

        public static object ReadSetting(IntPtr contactHandle, string name, DatabaseSettingType readAs)
        {
            IntPtr pOwnerName = GetModuleNamePtr(contactHandle);

            if (pOwnerName == IntPtr.Zero)
                throw new InvalidOperationException(TextResources.ExceptionMsg_OwnerUnknown);

            return ReadSetting(contactHandle, name, Marshal.PtrToStringAnsi(pOwnerName), readAs);
        }

        public object ReadSetting(string name, ISettingOwner owner, DatabaseSettingType readAs)
        {
            return ReadSetting(MirandaHandle, name, owner, readAs);
        }

        public static object ReadSetting(IntPtr contactHandle, string name, ISettingOwner owner, DatabaseSettingType readAs)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return ReadSetting(contactHandle, name, owner.Name, readAs);
        }

        public object ReadSetting(string name, string owner, DatabaseSettingType readAs)
        {
            return ReadSetting(MirandaHandle, name, owner, readAs);
        }

        public static object ReadSetting(IntPtr contactHandle, string name, string owner, DatabaseSettingType readAs)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            InteropBuffer buffer = InteropBufferPool.AcquireBuffer();

            try
            {
                DBCONTACTGETSETTING dbSetting = new DBCONTACTGETSETTING();
                dbSetting.Name = name;

                DBVARIANT dbVariant = new DBVARIANT();
                dbVariant.Type = (byte)readAs;

                buffer.Lock();

                if (readAs != DatabaseSettingType.Blob)
                {
                    dbVariant.Text.TextPtr = buffer.IntPtr;
                    dbVariant.Text.TextBufferSize = (ushort)buffer.Size;
                }
                else
                {
                    throw new NotImplementedException();
                    //dbVariant.Blob.BlobPtr = buffer.IntPtr;
                }

                return ReadSettingInternal(contactHandle, owner, ref dbSetting, ref dbVariant);

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

        private static object ReadSettingInternal(IntPtr contactHandle, string owner, ref DBCONTACTGETSETTING setting, ref DBVARIANT variant)
        {
            UnmanagedStructHandle<DBCONTACTGETSETTING> dbSettingHandle = UnmanagedStructHandle<DBCONTACTGETSETTING>.Empty;
            UnmanagedStructHandle<DBVARIANT> dbVariantHandle = UnmanagedStructHandle<DBVARIANT>.Empty;

            try
            {
                dbVariantHandle = new UnmanagedStructHandle<DBVARIANT>(ref variant);

                setting.Module = owner;
                setting.DbVariantPtr = dbVariantHandle.IntPtr;

                dbSettingHandle = new UnmanagedStructHandle<DBCONTACTGETSETTING>(ref setting);

                int result = MirandaContext.Current.CallService(MS_DB_CONTACT_GETSETTINGSTATIC, contactHandle, dbSettingHandle.IntPtr);
                Debug.Assert(result != 2, "Deleted setting encountered");

                if (result != 0)
                    return null;

                dbVariantHandle.MarshalBack(out variant);
                return Translate.ValueFromVariant(ref variant);
            }
            finally
            {
                dbVariantHandle.Free();
                dbSettingHandle.Free();
            }
        }

        #endregion

        #region Settings/Delete

        public bool DeleteSetting(string name, ISettingOwner owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return DeleteSetting(name, owner.Name);
        }

        public bool DeleteSetting(string name, string owner)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (String.IsNullOrEmpty(owner))
                throw new ArgumentNullException("owner");

            DBCONTACTGETSETTING dbGetSetting = new DBCONTACTGETSETTING();
            dbGetSetting.Name = name;
            dbGetSetting.Module = owner;

            UnmanagedStructHandle<DBCONTACTGETSETTING> dbSettingHandle = new UnmanagedStructHandle<DBCONTACTGETSETTING>(ref dbGetSetting);

            try
            {
                return MirandaContext.Current.CallService(MS_DB_CONTACT_DELETESETTING, MirandaHandle, dbSettingHandle.IntPtr) == 0;
            }
            finally
            {
                dbSettingHandle.Free();
            }
        }

        #endregion

        #region Converters

        public static TId[] GetUniqueIDs<TId>(params ContactInfo[] contacts)
        {
            if (contacts == null)
                throw new ArgumentNullException("contacts");

            if (contacts.Length == 0)
                return new TId[0];

            return Array.ConvertAll<ContactInfo, TId>(contacts, delegate(ContactInfo contact)
            {
                if (contact != null)
                    return contact.UniqueIdAs<TId>();
                else
                    return default(TId);
            });
        }

        public static string[] GetDisplayNames(params string[] uuids)
        {
            if (uuids == null)
                throw new ArgumentNullException("uuids");

            if (uuids.Length == 0)
                return new string[0];

            return Array.ConvertAll<string, string>(uuids, delegate(string uuid)
            {
                if (uuid == null)
                    return null;

                ContactInfo contact = MirandaContext.Current.MirandaDatabase.FindContact(uuid);

                if (contact != null)
                    return contact.DisplayName;
                else
                    return null;
            });
        }

        #endregion

        #region Helpers

        private static IntPtr GetModuleNamePtr(IntPtr contactHandle)
        {
            return (IntPtr)MirandaContext.Current.CallService(MS_PROTO_GETCONTACTBASEPROTO, contactHandle, IntPtr.Zero);
        }

        public static string GetDisplayName(IntPtr contactHandle)
        {
            ContactDisplayNameOptions options = ContactDisplayNameOptions.Unicode;

            IntPtr pName = (IntPtr)MirandaContext.Current.CallService(MS_CLIST_GETCONTACTDISPLAYNAME, contactHandle, (IntPtr)options);

            if (pName == IntPtr.Zero)
                return null;
            else
                return Translate.ToString(pName, StringEncoding.Unicode);
        }

        public static bool GetUniqueID(IntPtr contactHandle, out object uuid, out ContactInfoPropertyType uuidType)
        {
            return GetProperty(contactHandle, ContactInfoProperty.UniqueID, out uuid, out uuidType);
        }

        public static bool GetProperty(IntPtr contactHandle, ContactInfoProperty property, out object value, out ContactInfoPropertyType valueType)
        {
            CONTACTINFO contactInfo = new CONTACTINFO(contactHandle, GetModuleNamePtr(contactHandle));
            contactInfo.Flag = (byte)((byte)property | (byte)ContactInfoPropertyFlags.Unicode);

            unsafe
            {
                int result = MirandaContext.Current.CallServiceUnsafe(MirandaServices.MS_CONTACT_GETCONTACTINFO, null, &contactInfo);

                if (result != 0)
                {
                    value = null;
                    valueType = ContactInfoPropertyType.Unknown;

                    return false;
                }
            }

            switch (valueType = (ContactInfoPropertyType)contactInfo.Type)
            {
                case ContactInfoPropertyType.Byte:
                    value = Convert.ToByte(contactInfo.Value.ToInt32());
                    break;
                case ContactInfoPropertyType.String:
                    value = Translate.ToString(contactInfo.Value, StringEncoding.Unicode);
                    break;
                case ContactInfoPropertyType.UInt16:
                    value = Convert.ToUInt16(contactInfo.Value.ToInt32());
                    break;
                case ContactInfoPropertyType.UInt32:
                    value = contactInfo.Value.ToInt32();
                    break;
                default:
                    value = null;
                    valueType = ContactInfoPropertyType.Unknown;
                    return false;
            }

            return true;
        }

        #endregion

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MirandaHandle", MirandaHandle.ToInt64());
        }

        #endregion

        public TId UniqueIdAs<TId>()
        {
            object uuid = UniqueID;
            return uuid == null ? default(TId) : (TId)Convert.ChangeType(uuid, typeof(TId));
        }

        public override string ToString()
        {
            return !Protocol.UnknownProtocol.Equals(OwningModule) ?
                String.Format("{0} ({1})", DisplayName, OwningModule.Name) : DisplayName;
        }

        public static bool operator ==(ContactInfo first, ContactInfo second)
        {
            if (object.ReferenceEquals(first, second))
                return true;
            else if (object.ReferenceEquals(first, null))
                return false;
            else
                return first.Equals(second);
        }

        public static bool operator !=(ContactInfo first, ContactInfo second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            ContactInfo other = obj as ContactInfo;

            if (object.ReferenceEquals(other, null))
                return false;
            else
                return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return MirandaHandle.ToInt32();
        }

        public static ContactInfo CreateContact()
        {
            IntPtr hContact = (IntPtr)MirandaContext.Current.CallService(MS_DB_CONTACT_ADD);

            if (hContact == IntPtr.Zero)
                throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_DB_CONTACT_ADD, hContact.ToString()));

            return ContactInfo.FromHandle(hContact);
        }

        public bool Delete()
        {
            if (IsSelf)
                throw new InvalidOperationException();

            return (0 == MirandaContext.Current.CallService(MS_DB_CONTACT_DELETE, MirandaHandle, IntPtr.Zero));
        }

        public bool GetProperty(ContactInfoProperty property, StringEncoding stringEncoding)
        {
            return GetProperty(MirandaHandle, property, out value, out valueType);
        }

        public int CallContactService(string serviceName)
        {
            return CallContactService(serviceName, UIntPtr.Zero, IntPtr.Zero);
        }

        [CLSCompliant(false)]
        public int CallContactService(string serviceName, UIntPtr wParam, IntPtr lParam)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            CCSDATA ccsData = new CCSDATA(this, serviceName);
            ccsData.WParam = wParam;
            ccsData.LParam = lParam;

            try
            {
                unsafe
                {
                    return MirandaContext.Current.CallServiceUnsafe(MirandaServices.MS_PROTO_CALLCONTACTSERVICE, null, &ccsData);
                }
            }
            finally
            {
                ccsData.Free();
            }
        }

        public void SendMessage(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            UnmanagedStringHandle nativeHandle = UnmanagedStringHandle.Empty;

            try
            {
                nativeHandle = new UnmanagedStringHandle(message, StringEncoding.Ansi);
                CallContactService(MirandaServices.PS_MESSAGE, UIntPtr.Zero, nativeHandle.IntPtr);
            }
            finally
            {
                nativeHandle.Free();
            }
        }

        public void OpenMessageWindow()
        {
            OpenMessageWindow(null);
        }

        public void OpenMessageWindow(string message)
        {
            UnmanagedStringHandle messageHandle = UnmanagedStringHandle.Empty;

            try
            {
                if (!String.IsNullOrEmpty(message))
                    messageHandle = new UnmanagedStringHandle(message, StringEncoding.Ansi);

                int result = (int)CallbackResult.Success;

                if (result != MirandaContext.Current.CallService(MS_MSG_SENDMESSAGE, MirandaHandle, messageHandle.IntPtr))
                    if ((int)CallbackResult.Success != (result = MirandaContext.Current.CallService(MS_MSG_SENDMESSAGE_2, MirandaHandle, messageHandle.IntPtr)))
                        throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_MSG_SENDMESSAGE, result.ToString()));
            }
            finally
            {
                messageHandle.Free();
            }
        }

        #endregion        
    }
}
