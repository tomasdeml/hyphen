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
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Forms;
using Virtuoso.Hyphen.Mini;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    internal sealed class HookDescriptor : IMirandaObject, IDescriptor
    {
        #region Fields

        private string name;
        private IntPtr handle;

        private Callback callback, callbackStub;
        private HookType hookType;

        private PluginDescriptor owner;
        private bool registeredManually;

        #endregion

        #region .ctor

        public HookDescriptor(string name, PluginDescriptor owner, Callback callback, HookType type)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (owner == null)
                throw new ArgumentNullException("owner");

            if (callback == null)
                throw new ArgumentNullException("callback");

            this.callbackStub = SafeCallbackStub;

            this.name = name;
            this.owner = owner;
            this.callback = callback;
            this.hookType = type;
        }

        public static HookDescriptor SetUpAndStore(IList<HookDescriptor> targetContainer, string name, PluginDescriptor owner, Callback callback, HookType type)
        {
            if (targetContainer == null)
                throw new ArgumentNullException("targetContainer");

            HookDescriptor descriptor = new HookDescriptor(name, owner, callback, type);
            targetContainer.Add(descriptor);

            return descriptor;
        }

        public static HookDescriptor SetUpAndStore(IDictionary<string, HookDescriptor> targetContainer, string name, PluginDescriptor owner, Callback callback, HookType type)
        {
            return SetUpAndStore<string>(targetContainer, name, name, owner, callback, type);
        }

        public static HookDescriptor SetUpAndStore<T>(IDictionary<T, HookDescriptor> targetContainer, T key, string name, PluginDescriptor owner, Callback callback, HookType type)
        {
            if (targetContainer == null)
                throw new ArgumentNullException("targetContainer");

            HookDescriptor descriptor = new HookDescriptor(name, owner, callback, type);
            targetContainer.Add(key, descriptor);

            return descriptor;
        }

        #endregion

        #region Methods

        private int SafeCallbackStub(UIntPtr wParam, IntPtr lParam)
        {
            try
            {
                return callback(wParam, lParam);
            }
            catch (Exception e)
            {
                MirandaPlugin.GetExceptionHandler(owner).HandleException(e, owner);
                return (int)CallbackResult.Failure;
            }
        }

        public override int GetHashCode()
        {
            return (name.GetHashCode() + callback.Method.Name.GetHashCode() + (int)hookType);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            HookDescriptor other = obj as HookDescriptor;

            if (other == null)
                return false;

            return GetHashCode() == other.GetHashCode();
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
        }

        public IntPtr MirandaHandle
        {
            get { return handle; }
            internal set { handle = value; }
        }

        public Callback Callback
        {
            get { return callbackStub; }
        }

        public HookType HookType
        {
            get { return hookType; }
        }

        public PluginDescriptor Owner
        {
            get { return owner; }
        }

        public bool RegisteredManually
        {
            get { return registeredManually; }
            set { registeredManually = value; }
        }

        #endregion
    }
}
