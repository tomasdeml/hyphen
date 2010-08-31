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
using Virtuoso.Miranda.Plugins.Resources;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing;
using Virtuoso.Hyphen.Mini;
using System.Windows.Forms;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public sealed class ContactList : EventPublisher
    {
        #region Constants

        private const string ME_CLC_SHOWINFOTIP = "CLC/ShowInfoTip",
                             ME_CLC_HIDEINFOTIP = "CLC/HideInfoTip",
                             MS_CLC_SETINFOTIPHOVERTIME = "CLC/SetInfoTipHoverTime",
                             MS_CLC_GETINFOTIPHOVERTIME = "CLC/GetInfoTipHoverTime",
                             ME_CLIST_STATUSMODECHANGE = "CList/StatusModeChange",
                             ME_CLIST_PREBUILDCONTACTMENU = "CList/PreBuildContactMenu",
                             ME_CLIST_DOUBLECLICKED = "CList/DoubleClicked";

        private const string MS_CLIST_ADDEVENT = "CList/AddEvent",
                             MS_CLIST_REMOVEEVENT = "Clist/RemoveEvent";

        #endregion

        #region Fields

        private static readonly object SyncObject = new object();

        private EventHandler<ContactListEventArgs<ContactListInfoTip>> InfoTipShowEventHandler,
                                                                   InfoTipHideEventHandler;

        private EventHandler<ProtocolStatusChangeEventArgs> ProtocolStatusChangeEventHandler;
        private EventHandler<ContactListEventArgs<ContactInfo>> ContactMenuShowingEventHandler;
        private MirandaEventHandler<ContactListEventArgs<ContactInfo>> ContactDoubleClickedEventHandler;

        private bool ContactSelectionTrackingEnabled;
        private ContactInfo selectedContact;

        private static readonly Random Random = new Random();

        #endregion

        #region .ctors

        internal ContactList() { }

        #endregion

        #region Properties

        public ContactInfo SelectedContact
        {
            get
            {
                return selectedContact;
            }
        }

        public byte? Transparency
        {
            get
            {
                object alpha = ContactInfo.MeNeutral.ReadSetting("Alpha", "CList", DatabaseSettingType.Byte);

                if (alpha != null)
                    return (byte)alpha;
                else
                    return null;
            }
            set
            {
                ContactInfo.MeNeutral.WriteSetting("Alpha", "CList", value.GetValueOrDefault(byte.MaxValue), DatabaseSettingType.Byte);
            }
        }

        public bool TransparencyEnabled
        {
            get
            {
                object enabled = ContactInfo.MeNeutral.ReadSetting("Transparent", "CList", DatabaseSettingType.Byte);

                if (enabled != null)
                    return Convert.ToBoolean((byte)enabled);
                else
                    return false;
            }
            set
            {
                ContactInfo.MeNeutral.WriteSetting("Transparent", "CList", Convert.ToByte(value), DatabaseSettingType.Byte);
            }
        }

        #endregion

        #region Events

        private void FireInfoTipEvent(EventHandler<ContactListEventArgs<ContactListInfoTip>> e, IntPtr lParam)
        {
            if (e == null)
                return;

            ContactListInfoTip infoTip = (ContactListInfoTip)Marshal.PtrToStructure(lParam, typeof(ContactListInfoTip));
            ContactListEventArgs<ContactListInfoTip> eArgs = new ContactListEventArgs<ContactListInfoTip>(infoTip);

            e(this, eArgs);
        }

        public event EventHandler<ContactListEventArgs<ContactListInfoTip>> InfoTipShow
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<EventHandler<ContactListEventArgs<ContactListInfoTip>>>(ref InfoTipShowEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_CLC_SHOWINFOTIP,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        FireInfoTipEvent(InfoTipShowEventHandler, lParam);
                        return (int)CallbackResult.Success;
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<EventHandler<ContactListEventArgs<ContactListInfoTip>>>(ref InfoTipShowEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_CLC_SHOWINFOTIP, this.InfoTipShowEventHandler);
            }
        }

        public event EventHandler<ContactListEventArgs<ContactListInfoTip>> InfoTipHide
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<EventHandler<ContactListEventArgs<ContactListInfoTip>>>(ref InfoTipHideEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_CLC_HIDEINFOTIP,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        FireInfoTipEvent(InfoTipHideEventHandler, lParam);
                        return (int)CallbackResult.Success;
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<EventHandler<ContactListEventArgs<ContactListInfoTip>>>(ref InfoTipHideEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_CLC_HIDEINFOTIP, this.InfoTipHideEventHandler);
            }
        }

        public event EventHandler<ProtocolStatusChangeEventArgs> ProtocolStatusChange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<EventHandler<ProtocolStatusChangeEventArgs>>(ref ProtocolStatusChangeEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_CLIST_STATUSMODECHANGE,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        string protocolName = lParam == IntPtr.Zero ? null : Translate.ToString(lParam, StringEncoding.Ansi);
                        Protocol protocol = null;

                        if (!String.IsNullOrEmpty(protocolName))
                            protocol = new Protocol(protocolName, ProtocolType.Protocol);

                        ProtocolStatusChangeEventArgs eArgs = new ProtocolStatusChangeEventArgs(protocol, Translate.ToStatus(wParam));

                        if (ProtocolStatusChangeEventHandler != null)
                            ProtocolStatusChangeEventHandler(this, eArgs);

                        return (int)CallbackResult.Success;
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<EventHandler<ProtocolStatusChangeEventArgs>>(ref ProtocolStatusChangeEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_CLIST_STATUSMODECHANGE, ProtocolStatusChangeEventHandler);
            }
        }

        public event EventHandler<ContactListEventArgs<ContactInfo>> ContactMenuShowing
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<EventHandler<ContactListEventArgs<ContactInfo>>>(ref ContactMenuShowingEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_CLIST_PREBUILDCONTACTMENU,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        if (ContactMenuShowingEventHandler != null)
                            ContactMenuShowingEventHandler(this, new ContactListEventArgs<ContactInfo>(ContactInfo.FromHandle(wParam)));

                        return (int)CallbackResult.Success;
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<EventHandler<ContactListEventArgs<ContactInfo>>>(ref ContactMenuShowingEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_CLIST_PREBUILDCONTACTMENU, ContactMenuShowingEventHandler);
            }
        }

        public event MirandaEventHandler<ContactListEventArgs<ContactInfo>> ContactDoubleClicked
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                LazyEventBinder.AttachDelegate<MirandaEventHandler<ContactListEventArgs<ContactInfo>>>(ref ContactDoubleClickedEventHandler, value);
                LazyEventBinder.HookMirandaEvent(ME_CLIST_DOUBLECLICKED,
                    delegate(UIntPtr wParam, IntPtr lParam)
                    {
                        bool retValue = InvokeChainCancelable<ContactListEventArgs<ContactInfo>>(ContactDoubleClickedEventHandler, new ContactListEventArgs<ContactInfo>(ContactInfo.FromHandle(wParam)));
                        return Convert.ToInt32(retValue);
                    });
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                LazyEventBinder.DetachDelegate<MirandaEventHandler<ContactListEventArgs<ContactInfo>>>(ref ContactDoubleClickedEventHandler, value);
                LazyEventBinder.UnhookMirandaEvent(ME_CLIST_DOUBLECLICKED, ContactDoubleClickedEventHandler);
            }
        }

        #endregion

        #region Methods

        #region Menu

        public void AddMenuItem(MirandaPlugin owner, MenuItemDeclarationAttribute item)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (item == null)
                throw new ArgumentNullException("item");

            string serviceName = item.IsContactMenuItem ? MirandaServices.MS_CLIST_ADDCONTACTMENUITEM : MirandaServices.MS_CLIST_ADDMAINMENUITEM;

            UnmanagedStructHandle<CLISTMENUITEM> nativeHandle = UnmanagedStructHandle<CLISTMENUITEM>.Empty;
            CLISTMENUITEM nativeItem = new CLISTMENUITEM(owner, item);

            try
            {
                nativeHandle = new UnmanagedStructHandle<CLISTMENUITEM>(ref nativeItem);

                IntPtr handle = (IntPtr)MirandaContext.Current.CallService(serviceName, UIntPtr.Zero, nativeHandle.IntPtr,
                    (owner is StandalonePlugin && !item.IsAdditional));

                item.MirandaHandle = handle;
                Debug.Assert(handle != IntPtr.Zero);
            }
            finally
            {
                nativeHandle.Free();
            }
        }

        public bool ModifyMenuItem(MirandaPlugin owner, MenuItemDeclarationAttribute menuItem, string text)
        {
            return ModifyMenuItem(owner, menuItem, text, MenuItemProperties.None, null, 0, true);
        }

        public bool ModifyMenuItem(MirandaPlugin owner, MenuItemDeclarationAttribute menuItem, MenuItemProperties flags)
        {
            return ModifyMenuItem(owner, menuItem, null, flags, null, 0, true);
        }

        public bool ModifyMenuItem(MirandaPlugin owner, MenuItemDeclarationAttribute menuItem, HotKeys hotKey)
        {
            return ModifyMenuItem(owner, menuItem, null, MenuItemProperties.None, null, hotKey, true);
        }

        public bool ModifyMenuItem(MirandaPlugin owner, MenuItemDeclarationAttribute menuItem, Icon icon)
        {
            return ModifyMenuItem(owner, menuItem, null, MenuItemProperties.None, icon, 0, true);
        }

        public bool ModifyMenuItem(MirandaPlugin owner, MenuItemDeclarationAttribute menuItem, string text, MenuItemProperties flags, Icon icon, HotKeys hotKey)
        {
            return ModifyMenuItem(owner, menuItem, text, flags, icon, hotKey, true);
        }

        public bool ModifyMenuItem(MirandaPlugin owner, MenuItemDeclarationAttribute menuItem, string text, MenuItemProperties flags, Icon icon, HotKeys hotKey, bool updateItemDescriptor)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (menuItem == null)
                throw new ArgumentNullException("menuItem");

            if (menuItem.MirandaHandle == IntPtr.Zero)
                throw new ArgumentException("Invalid menu item handle.");

            UnmanagedStructHandle<CLISTMENUITEM> nativeHandle = UnmanagedStructHandle<CLISTMENUITEM>.Empty;

            try
            {
                SynchronizationHelper.BeginMenuItemUpdate(menuItem);

                CLISTMENUITEM nativeItem = new CLISTMENUITEM(owner, menuItem);
                MenuItemModifyFlags modifyFlags = MenuItemModifyFlags.None;

                if (text != null)
                {
                    modifyFlags |= MenuItemModifyFlags.CMIM_NAME;
                    nativeItem.Text = text;

                    if (updateItemDescriptor) menuItem.Text = text;
                }
                if (flags != MenuItemProperties.KeepCurrent)
                {
                    modifyFlags |= MenuItemModifyFlags.CMIM_FLAGS;
                    nativeItem.Flags = (uint)flags;

                    if (updateItemDescriptor) menuItem.Flags = flags;
                }
                if (icon != null)
                {
                    modifyFlags |= MenuItemModifyFlags.CMIM_ICON;
                    nativeItem.Icon = icon.Handle;
                }
                if (hotKey != 0)
                {
                    modifyFlags |= MenuItemModifyFlags.CMIM_HOTKEY;
                    nativeItem.HotKey = (uint)hotKey;
                    if (updateItemDescriptor) menuItem.HotKey = hotKey;
                }

                nativeItem.Flags |= (uint)modifyFlags;

                nativeHandle = new UnmanagedStructHandle<CLISTMENUITEM>(ref nativeItem);
                bool result = MirandaContext.Current.CallService(MirandaServices.MS_CLIST_MODIFYMENUITEM, (UIntPtr)(uint)menuItem.MirandaHandle, nativeHandle.IntPtr) == 0
                    ? true : false;

                Debug.Assert(result);
                return result;
            }
            catch (Exception e)
            {
                throw new MirandaException(TextResources.ExceptionMsg_ErrorWhileCallingMirandaService + e.Message, e);
            }
            finally
            {
                nativeHandle.Free();
                SynchronizationHelper.EndUpdate(menuItem);
            }
        }

        #endregion

        #region UI

        public bool ShowBaloonTip(string title, string text, string protocol, ToolTipIcon icon, int timeout)
        {
            UnmanagedStructHandle<MIRANDASYSTRAYNOTIFY> nativeHandle = UnmanagedStructHandle<MIRANDASYSTRAYNOTIFY>.Empty;

            try
            {
                MIRANDASYSTRAYNOTIFY msn = new MIRANDASYSTRAYNOTIFY(title, text, icon);
                msn.Timeout = (uint)timeout;
                msn.Protocol = protocol;

                nativeHandle = new UnmanagedStructHandle<MIRANDASYSTRAYNOTIFY>(ref msn);
                int result = MirandaContext.Current.CallService(MirandaServices.MS_CLIST_SYSTRAY_NOTIFY, UIntPtr.Zero, nativeHandle.IntPtr);

                bool retValue = (result == 0);
                Debug.Assert(retValue);

                return retValue;
            }
            finally
            {
                nativeHandle.Free();
            }
        }

        public bool SetInfoTipHoverTime(int time)
        {
            int result = MirandaContext.Current.CallService(MS_CLC_SETINFOTIPHOVERTIME, (UIntPtr)(uint)time, IntPtr.Zero);
            Debug.Assert(result == 0);

            return result == 0;
        }

        public int GetInfoTipHoverTime()
        {
            return MirandaContext.Current.CallService(MS_CLC_GETINFOTIPHOVERTIME);
        }

        public void EnableContactSelectionTracking()
        {
            lock (SyncObject)
            {
                if (ContactSelectionTrackingEnabled)
                    return;

                ContactMenuShowing += HandleContactSelection;
                ContactSelectionTrackingEnabled = true;

                MirandaContext.Current.IsolatedModePluginsUnloading += delegate { ContactMenuShowing -= HandleContactSelection; };
            }
        }

        private void HandleContactSelection(object sender, ContactListEventArgs<ContactInfo> e)
        {
            lock (SyncObject)
                selectedContact = e.EventData;
        }

        #endregion

        #region Events

        public IntPtr AddEvent(ContactInfo contact, Icon icon, string serviceToCall, ContactListEventProperties properties, string toolTip)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            return AddEvent(contact, icon, serviceToCall, IntPtr.Zero, contact.MirandaHandle, properties, toolTip);
        }

        public IntPtr AddEvent(ContactInfo contact, Icon icon, string serviceToCall, IntPtr lParamToPass, IntPtr eventToken, ContactListEventProperties properties, string toolTip)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            if (icon == null)
                throw new ArgumentNullException("icon");

            ContactListEvent clistEvent = new ContactListEvent();
            clistEvent.ContactHandle = contact.MirandaHandle;
            clistEvent.EventHandle = eventToken;
            clistEvent.Flags = (uint)properties;
            clistEvent.IconHandle = icon.Handle;
            clistEvent.LParam = lParamToPass;
            clistEvent.ServiceName = serviceToCall;
            clistEvent.Tooltip = toolTip;

            UnmanagedStructHandle<ContactListEvent> nativeStruct = UnmanagedStructHandle<ContactListEvent>.Empty;

            try
            {
                nativeStruct = new UnmanagedStructHandle<ContactListEvent>(ref clistEvent);
                IntPtr eventHandle = (IntPtr)MirandaContext.Current.CallService(MS_CLIST_ADDEVENT, UIntPtr.Zero, nativeStruct.IntPtr);

                if (eventHandle != IntPtr.Zero)
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_CLIST_ADDEVENT, eventHandle.ToString()));

                return clistEvent.EventHandle;
            }
            finally
            {
                nativeStruct.Free();
            }
        }

        public bool RemoveEvent(ContactInfo contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            return RemoveEvent(contact, contact.MirandaHandle);
        }

        public bool RemoveEvent(ContactInfo contact, IntPtr eventToken)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            return !Convert.ToBoolean(MirandaContext.Current.CallService(MS_CLIST_REMOVEEVENT, contact.MirandaHandle, eventToken));
        }

        #endregion

        #endregion
    }
}
