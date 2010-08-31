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
using Virtuoso.Miranda.Plugins.Configuration;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigurationOptionsAttribute : Attribute
    {
        #region .ctors

        internal ConfigurationOptionsAttribute() { }

        public ConfigurationOptionsAttribute(string configurationVersion) : this(configurationVersion, false, true) { }

        [Obsolete("Will be removed in future. Use named arguments instead.", true)]
        public ConfigurationOptionsAttribute(bool encrypt, bool profileBound) : this(null, encrypt, profileBound) { }

        [Obsolete("Will be removed in future. Use named arguments instead.", false)]
        public ConfigurationOptionsAttribute(string configurationVersion, bool encrypt, bool profileBound)
        {
            if (!String.IsNullOrEmpty(configurationVersion))
                this.version = new Version(configurationVersion);

            this.encrypt = encrypt;
            this.profileBound = profileBound;
        }

        #endregion

        #region Properties

        private readonly Version version;
        public Version Version
        {
            get { return version; }
        }

        private bool profileBound;
        public bool ProfileBound
        {
            get { return profileBound; }
            set { profileBound = value; }
        }

        private bool encrypt;
        public bool Encrypt
        {
            get { return encrypt; }
            set { encrypt = value; }
        }

        private Type storage;
        public Type Storage
        {
            get { return storage; }
            set { storage = value; }
        }

        private Type encryption;
        public Type Encryption
        {
            get { return encryption; }
            set { encryption = value; }
        }

        #endregion

        #region Methods

        internal ConfigurationOptionsAttribute Finalize()
        {
            if (storage == null)
                storage = typeof(IsolatedStorage);

            if (encrypt && encryption == null)
                encryption = typeof(WindowsEncryption);

            return this;
        }

        #endregion
    }
}
