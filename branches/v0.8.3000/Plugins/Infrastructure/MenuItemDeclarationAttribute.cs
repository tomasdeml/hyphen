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
using System.Reflection;
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class MenuItemDeclarationAttribute : Attribute, IMirandaObject
    {
        #region Fields & Properties

        private static readonly Type StringResolverType = typeof(IStringResolver);

        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private MenuItemProperties flags;
        public MenuItemProperties Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        private int position, popUpPosition;
        public int PopUpPosition
        {
            get { return popUpPosition; }
            set { popUpPosition = value; }
        }
        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        private bool useEmbeddedIcon;
        public bool UseEmbeddedIcon
        {
            get { return useEmbeddedIcon; }
            set { useEmbeddedIcon = value; }
        }

        private bool hasIcon;
        public bool HasIcon
        {
            get { return hasIcon; }
            set { hasIcon = value; }
        }

        private string iconID;
        public string IconID
        {
            get { return iconID; }
            set { iconID = value; }
        }

        private string service;
        public string Service
        {
            get { return service; }
            internal set { service = value; }
        }

        private string popUpMenu;
        public string PopUpMenu
        {
            get { return popUpMenu; }
        }

        private string owningModule;
        public string OwningModule
        {
            get { return owningModule; }
            set { owningModule = value; }
        }

        private HotKeys hotKey;
        public HotKeys HotKey
        {
            get { return hotKey; }
            set { hotKey = value; }
        }

        private Type stringResolver;
        public Type StringResolver
        {
            get { return stringResolver; }
        }

        private string tag;
        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private bool isContactMenuItem;
        public bool IsContactMenuItem
        {
            get { return isContactMenuItem; }
            set { isContactMenuItem = value; }
        }

        private IntPtr mirandaHandle;
        public IntPtr MirandaHandle
        {
            get
            {
                return this.mirandaHandle;
            }
            internal set
            {
                this.mirandaHandle = value;
            }
        }

        private bool isAdditional;
        public bool IsAdditional
        {
            get { return isAdditional; }
            set { isAdditional = value; }
        }

        #endregion

        #region .ctors

        public MenuItemDeclarationAttribute(string text) : this(text, null, null, null) { }

        public MenuItemDeclarationAttribute(string text, Type stringResolver) : this(text, null, stringResolver) { }

        public MenuItemDeclarationAttribute(string text, string service) : this(text, null, service, null) { }

        public MenuItemDeclarationAttribute(string text, string popUpMenu, string service) : this(text, popUpMenu, service, null) { }

        public MenuItemDeclarationAttribute(string text, string service, Type stringResolver) : this(text, null, service, stringResolver) { }

        public MenuItemDeclarationAttribute(string text, string popUpMenu, string service, Type stringResolver)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            this.text = text;
            this.service = (service == null ? String.Empty : service);
            this.popUpMenu = popUpMenu;
            this.stringResolver = stringResolver;

            /* m_clist.h
             * WARNING: do not use Translate(TS) for p(t)szName or p(t)szPopupName as they
               are translated by the core, which may lead to double translation. */
            if (stringResolver != null && stringResolver.GetType() != typeof(LanguagePackStringResolver))
                ResolveStrings(text, popUpMenu, stringResolver);
        }

        private void ResolveStrings(string text, string popUpMenu, Type stringResolver)
        {
            try
            {
                if (stringResolver != null && stringResolver.GetInterface(StringResolverType.FullName) != null)
                {
                    IStringResolver resolver = null;
                    StringResolverCache cache = StringResolverCache.Singleton;

                    lock (cache)
                    {
                        if (!cache.TryGetValue(stringResolver, out resolver))
                        {
                            resolver = (IStringResolver)Activator.CreateInstance(stringResolver, true);

                            if (resolver == null)
                                throw new TypeLoadException(stringResolver.FullName);
                            else
                                cache.Add(stringResolver, resolver);
                        }
                    }

                    this.text = resolver.ResolveString(text, tag);
                    this.popUpMenu = resolver.ResolveString(popUpMenu, tag);
                }
            }
            catch (Exception e)
            {
                throw new FusionException(String.Format(TextResources.ExceptionMsg_Formatable1_CannotLoadStringResolver, stringResolver.FullName), null, null, null, e);
            }
        }

        #endregion
    }
}
