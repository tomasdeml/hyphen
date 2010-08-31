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
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.IO;

namespace Virtuoso.Miranda.Plugins.Configuration
{
    public abstract class StorageBase : IStorage
    {
        #region .ctors

        protected StorageBase() { }

        #endregion			
        
        #region Helpers

        protected virtual string GetFileName(Type configType, ConfigurationOptionsAttribute options)
        {
            if (configType == null)
                throw new ArgumentNullException("configType");

            string versionSuffix = options != null && options.Version != null ?
                options.Version.ToString() : String.Empty;

            string profileBoundSuffix = String.Empty;

            if (options != null && options.ProfileBound)
            {
                if (!MirandaContext.Initialized)
                    throw new InvalidOperationException();

                profileBoundSuffix = Path.GetFileNameWithoutExtension(MirandaContext.Current.MirandaDatabase.ProfileName);
            }

            return String.Format("{0}_{1}_{2}", configType.FullName, versionSuffix, profileBoundSuffix).Replace('.', '-') + ".dat";
        }

        #endregion			
        
        #region Methods

        public abstract Stream OpenRead(Type configType, ConfigurationOptionsAttribute options);

        public abstract Stream OpenWrite(Type configType, ConfigurationOptionsAttribute options);

        public abstract bool Exists(Type configType, ConfigurationOptionsAttribute options);

        public abstract void Dispose();

        #endregion
    }
}
