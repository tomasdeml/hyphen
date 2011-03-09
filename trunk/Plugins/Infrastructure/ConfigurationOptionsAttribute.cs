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
                Version = new Version(configurationVersion);

            Encrypt = encrypt;
            ProfileBound = profileBound;
        }

        #endregion

        #region Properties

        public Version Version
        {
            get; private set;
        }

        public bool ProfileBound { get; set; }

        public bool Encrypt { get; set; }

        public Type Storage { get; set; }

        public Type Encryption { get; set; }

        public string StaticFileName { get; set; }

        #endregion

        #region Methods

        internal ConfigurationOptionsAttribute Initialize()
        {
            if (Storage == null)
                Storage = typeof(IsolatedStorage);

            if (Encrypt && Encryption == null)
                Encryption = typeof(WindowsEncryption);

            return this;
        }

        #endregion
    }
}
