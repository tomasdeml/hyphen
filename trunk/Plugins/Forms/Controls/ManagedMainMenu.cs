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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Native;
using System.Drawing;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Miranda.Plugins.Forms.Controls;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    // Impl note: must be public, remoting rejects non-public method calls.
    public sealed class ManagedMainMenu : ContextMenuStrip
    {
        #region Delegates

        private delegate void AddMainMenuItemInvoker(ManagedMainMenu menu, ManagedMainMenuItem item);
        private delegate void ModifyMenuItemInvoker(ManagedMainMenu menu, ref CLISTMENUITEM itemData, string handle);

        #endregion

        #region Fields

        private static readonly Random HandleGenerator = new Random();

        private static readonly AddMainMenuItemInvoker AddMainMenuItemDelegate = new AddMainMenuItemInvoker(AddMainMenuItem);
        private static readonly ModifyMenuItemInvoker ModifyMenuItemDelegate = new ModifyMenuItemInvoker(ModifyMenuItem);

        private static readonly ToolStripMenuItem EmptyItem;
        private readonly int EmptyItemIndex;
        
        #endregion

        #region .ctors & .dctors

        static ManagedMainMenu()
        {
            EmptyItem = new ToolStripMenuItem(TextResources.UI_Label_Empty);
            EmptyItem.Visible = false;
        }

        internal ManagedMainMenu()
        {
            RenderMode = ToolStripRenderMode.System;
            EmptyItemIndex = Items.Add(EmptyItem);
        }

        #endregion

        #region Methods

        #region Overrides

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion

        #region UI

        internal void ShowUnderCursor()
        {
            if (Items.Count == 1)
                Items[EmptyItemIndex].Visible = true;
            else
                Items[EmptyItemIndex].Visible = false;

            Show(Cursor.Position);
        }

        #endregion

        #region Interceptors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu"></param>
        /// <remarks>
        /// This method is static to not let the execution run in default AppDomain but in a domain of the menu itself.
        /// </remarks>
        internal static void RegisterInterceptors(ManagedMainMenu menu)
        {
            MirandaContext context = MirandaContext.Current;

            context.ServiceCallInterceptors.Register(MirandaServices.MS_CLIST_ADDMAINMENUITEM, menu.AddMainMenuItemServiceInterceptor);
            context.ServiceCallInterceptors.Register(MirandaServices.MS_CLIST_MODIFYMENUITEM, menu.ModifyMenuItemInterceptor);
        }

        internal static void UnregisterInterceptors(ManagedMainMenu menu)
        {
            MirandaContext context = MirandaContext.Current;

            context.ServiceCallInterceptors.Unregister(MirandaServices.MS_CLIST_ADDMAINMENUITEM);
            context.ServiceCallInterceptors.Unregister(MirandaServices.MS_CLIST_MODIFYMENUITEM);
        }

        private int AddMainMenuItemServiceInterceptor(UIntPtr wParam, IntPtr lParam)
        {
            CLISTMENUITEM itemData = (CLISTMENUITEM)Marshal.PtrToStructure(lParam, typeof(CLISTMENUITEM));
            ManagedMainMenuItem menuItem = null;
            Image itemImage = null;
            
            if (itemData.Icon != IntPtr.Zero)
                itemImage = IconImageCache.Singleton.GetIconImage(itemData.Icon);

            menuItem = new ManagedMainMenuItem(itemData.Text, itemData.PopUpMenu, itemData.Service, itemImage);

            if (InvokeRequired)
                Invoke(AddMainMenuItemDelegate, this, menuItem);
            else
                AddMainMenuItemDelegate(this, menuItem);

            return menuItem.Handle;
        }

        private static void AddMainMenuItem(ManagedMainMenu menu, ManagedMainMenuItem item)
        {
            if (!String.IsNullOrEmpty(item.PopUpMenu))
            {
                string popupName = item.PopUpMenu;
                ToolStripMenuItem popupItem = null;

                ToolStripItem[] popupItems = menu.Items.Find(popupName, false);

                if (popupItems.Length > 0)
                    popupItem = (ToolStripMenuItem)popupItems[0];
                else
                {
                    popupItem = new ToolStripMenuItem(popupName);
                    popupItem.Name = popupName;

                    menu.Items.Add(popupItem);
                }

                popupItem.DropDownItems.Add(item);
            }
            else
                menu.Items.Add(item);
        }

        private int ModifyMenuItemInterceptor(UIntPtr wParam, IntPtr lParam)
        {
            try
            {
                CLISTMENUITEM itemData = (CLISTMENUITEM)Marshal.PtrToStructure(lParam, typeof(CLISTMENUITEM));
                string handle = wParam.ToString();

                if (!Items.ContainsKey(handle))
                {
                    if (String.IsNullOrEmpty(itemData.PopUpMenu) ||
                        !Items.ContainsKey(itemData.PopUpMenu) || 
                        !((ToolStripMenuItem)Items[itemData.PopUpMenu]).DropDownItems.ContainsKey(handle))
                            return MirandaContext.Current.CallService(MirandaServices.MS_CLIST_MODIFYMENUITEM, wParam, lParam, true);
                }

                if (InvokeRequired)
                    Invoke(ModifyMenuItemDelegate, this, itemData, handle);
                else
                    ModifyMenuItemDelegate(this, ref itemData, handle);

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        private static void ModifyMenuItem(ManagedMainMenu menu, ref CLISTMENUITEM itemData, string handle)
        {
            ManagedMainMenuItem item = (ManagedMainMenuItem)menu.Items.Find(handle, itemData.PopUpMenu != null)[0];
            MenuItemModifyFlags flags = (MenuItemModifyFlags)itemData.Flags;

            if ((flags & MenuItemModifyFlags.CMIM_NAME) == MenuItemModifyFlags.CMIM_NAME)
                item.Text = itemData.Text;

            if ((flags & MenuItemModifyFlags.CMIM_ICON) == MenuItemModifyFlags.CMIM_ICON)
                item.Image = IconImageCache.Singleton.GetIconImage(itemData.Icon);

            if ((flags & MenuItemModifyFlags.CMIM_HOTKEY) == MenuItemModifyFlags.CMIM_HOTKEY)
                item.ShortcutKeys = (Keys)itemData.HotKey;

            if ((flags & MenuItemModifyFlags.CMIM_FLAGS) == MenuItemModifyFlags.CMIM_FLAGS)
            {
                MenuItemProperties itemFlags = (MenuItemProperties)(flags & ~MenuItemModifyFlags.CMIM_ALL);

                switch (itemFlags)
                {
                    case MenuItemProperties.Grayed:
                        item.Enabled = false;
                        break;
                    case MenuItemProperties.Hidden:
                        if (item.OwnerItem == null)
                            menu.Items.Remove(item);
                        else
                        {
                            ToolStripMenuItem popupItem = (ToolStripMenuItem)item.OwnerItem;
                            popupItem.DropDownItems.Remove(item);

                            if (popupItem.DropDownItems.Count == 0)
                                menu.Items.Remove(popupItem);
                        }
                        break;
                    case MenuItemProperties.Checked:
                        item.Checked = true;
                        break;
                    case MenuItemProperties.None:
                        item.Enabled = true;
                        item.Visible = true;
                        item.Checked = false;
                        break;
                }
            }
        }

        #endregion

        #endregion
    }
}
