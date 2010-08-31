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
using System.Runtime.InteropServices;
using System.Reflection;
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.Drawing;
using System.Diagnostics;
using System.IO;

namespace Virtuoso.Miranda.Plugins.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    internal struct CLISTMENUITEM
    {
        #region Fields

        private readonly int Size;

        public string Text;
        public uint Flags;
        public int Position;
        public IntPtr Icon;
        public string Service;
        public string PopUpMenu;
        public int PopUpPosition;
        public uint HotKey;
        public string ContactOwner;

        #endregion

        #region .ctors

        public CLISTMENUITEM(MirandaPlugin owner, MenuItemDeclarationAttribute attrib)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (attrib == null)
                throw new ArgumentNullException("attrib");

            this.Text = attrib.Text;
            this.Service = attrib.Service;
            this.ContactOwner = attrib.OwningModule;
            this.Flags = (uint)attrib.Flags;
            this.PopUpMenu = attrib.PopUpMenu;
            this.PopUpPosition = attrib.PopUpPosition;
            this.Position = attrib.Position;
            this.HotKey = (uint)attrib.HotKey;
            this.Icon = IntPtr.Zero;
            this.Size = Marshal.SizeOf(typeof(CLISTMENUITEM));

            LoadIcon(owner, attrib);
        }

        #endregion

        #region Methods

        private void LoadIcon(MirandaPlugin owner, MenuItemDeclarationAttribute attrib)
        {
            try
            {
                if (!attrib.HasIcon)
                    return;

                if (attrib.UseEmbeddedIcon)
                {
                    using (Stream stream = owner.GetType().Assembly.GetManifestResourceStream(attrib.IconID))
                    {
                        if (stream != null)
                            Icon = IconImageCache.Singleton.GetStreamedIcon(stream).Handle;
                        else
                            Debug.Fail("Embedded icon not found.");
                    }
                }
                else
                    Icon = Skin.LoadIcon(int.Parse(attrib.IconID));
            }
            catch
            {
                this.Icon = IntPtr.Zero;
            }
        }

        #endregion
    }

    /*
        typedef struct {
	        int cbSize;			//size in bytes of this structure
	        char *pszName;		//text of the menu item
	        DWORD flags;		//flags
	        int position;		//approx position on the menu. lower numbers go nearer the top
	        HICON hIcon;		//HasIcon to put by the item. If this was not loaded from
	                            //a resource, you can delete it straight after the call
	        char *pszService;	//eventName of service to call when the item gets selected
	        char *pszPopupName;	//eventName of the popup menu that this item is on. If this
						        //is NULL the item is on the root of the menu
	        int popupPosition;	//position of the popup menu on the root menu. Ignored
						        //if pszPopupName is NULL or the popup menu already
						        //existed
	        DWORD hotKey;       //keyboard accelerator, same as lParam of WM_HOTKEY
	                            //0 for none
	        char *pszContactOwner; //contact menus only. The protocol module that owns
	                  //the contacts to which this menu item applies. NULL if it
			          //applies to all contacts. If it applies to multiple but not all
			          //protocols, add multiple menu items or use ME_CLIST_PREBUILDCONTACTMENU
        } CLISTMENUITEM;
    */
}
