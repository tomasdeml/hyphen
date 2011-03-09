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
using System.IO;
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.IO.IsolatedStorage;

namespace Virtuoso.Miranda.Plugins.Configuration
{
    public class IsolatedStorage : StorageBase
    {
        #region Fields

        private IsolatedStorageFile Store;

        #endregion			
        
        #region .ctors

        public IsolatedStorage()
        {
            Store = IsolatedStorageFile.GetUserStoreForAssembly();
        }

        #endregion			

        #region Methods

        public override Stream OpenRead(Type configType, ConfigurationOptionsAttribute options)
        {
            if (Store == null)
                throw new InvalidOperationException();

            return new IsolatedStorageFileStream(GetFileName(configType, options), FileMode.Open, Store);
        }

        public override Stream OpenWrite(Type configType, ConfigurationOptionsAttribute options)
        {
            if (Store == null)
                throw new InvalidOperationException();

            return new IsolatedStorageFileStream(GetFileName(configType, options), FileMode.Create, Store);
        }

        public override bool Exists(Type configType, ConfigurationOptionsAttribute options)
        {
            return Store.GetFileNames(GetFileName(configType, options)).Length != 0;
        }

        public override void Delete(Type configType, ConfigurationOptionsAttribute options)
        {
            if (!Exists(configType, options))
                return;

            string path = GetFileName(configType, options);
            Store.DeleteFile(path);
        }

        public override void Dispose()
        {
            if (Store != null)
                Store.Dispose();

            Store = null;
        }

        #endregion
    }
}
