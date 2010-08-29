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
using System.Collections.ObjectModel;
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.Windows.Forms;
using System.Drawing;

namespace Virtuoso.Miranda.Plugins.Forms.Controls
{
    public class ContactListView : ListView
    {
        #region Fields

        private ImageList ContactImages;
        private System.ComponentModel.IContainer components;

        #endregion

        #region .ctors

        public ContactListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactListView));
            this.ContactImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // ContactImages
            // 
            this.ContactImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ContactImages.ImageStream")));
            this.ContactImages.TransparentColor = System.Drawing.Color.Transparent;
            this.ContactImages.Images.SetKeyName(0, "Contact");
            // 
            // ContactListView
            // 
            this.SmallImageList = this.ContactImages;
            this.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.View = System.Windows.Forms.View.List;
            this.ResumeLayout(false);

        }

        #endregion

        #region Events

        public event EventHandler<ContactFilterEventArgs> FilterContact;

        #endregion

        #region Handlers

        protected virtual object CreateItemTag(ContactInfo contact)
        {
            return contact;
        }

        protected virtual string CreateItemText(ContactInfo contact, object tag)
        {
            return contact.ToString();
        }

        protected virtual int GetImageIndex(ContactInfo contact, object tag)
        {
            return 0;
        }

        #endregion

        #region Methods

        public virtual void LoadContacts()
        {
            ReadOnlyCollection<ContactInfo> contacts = MirandaContext.Current.MirandaDatabase.GetContacts(false);
            Items.Clear();

            ContactFilterEventArgs e = new ContactFilterEventArgs();

            for (int i = 0; i < contacts.Count; i++)
                InsertContact(contacts[i], e);
        }

        public virtual void InsertContact(ContactInfo contact)
        {
            InsertContact(contact, null);
        }

        private void InsertContact(ContactInfo contact, ContactFilterEventArgs e)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            if (e != null && FilterContact != null)
            {
                e.Contact = contact;
                e.Skip = false;

                FilterContact(this, e);

                if (e.Skip)
                    return;
            }

            object tag = CreateItemTag(contact);

            if (tag != null)
            {
                ListViewItem item = new ListViewItem(CreateItemText(contact, tag), GetImageIndex(contact, tag));
                item.Tag = tag;

                Items.Add(item);
            }
        }

        public virtual bool RemoveContact(ContactInfo contact)
        {
            int index = -1;

            for (int i = 0; index == -1 && i < Items.Count; i++)
                if (Items[i].Tag.Equals(CreateItemTag(contact)))
                    index = i;

            if (index != -1)
            {
                Items.RemoveAt(index);
                return true;
            }
            else
                return false;
        }

        public virtual ListViewItem FindContactItem(ContactInfo contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            object tag = CreateItemTag(contact);

            ListViewItem item = null;

            if (Items.Count > 0)
                item = FindItemWithText(CreateItemText(contact, tag), false, 0, false);

            if (item == null)
                return null;

            if (object.ReferenceEquals(item.Tag, tag) || (tag != null && tag.Equals(item.Tag)))
                return item;
            else
                return null;
        }

        #endregion
    }

    public class ContactFilterEventArgs : EventArgs
    {
        public ContactFilterEventArgs() { }

        private ContactInfo contact;
        public ContactInfo Contact
        {
            get { return contact; }
            internal set { contact = value; }
        }

        private bool skip;
        public bool Skip
        {
            get { return skip; }
            set { skip = value; }
        }
    }
}
