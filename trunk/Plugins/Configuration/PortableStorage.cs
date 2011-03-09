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

namespace Virtuoso.Miranda.Plugins.Configuration
{
    public class PortableStorage : StorageBase
    {
        #region Helpers

        private string GetPath(Type configType, ConfigurationOptionsAttribute options)
        {
            string configDirectory = Path.Combine(MirandaEnvironment.MirandaFolderPath, "Configuration");

            if (!Directory.Exists(configDirectory))
                Directory.CreateDirectory(configDirectory);

            return Path.Combine(configDirectory, GetFileName(configType, options));
        }

        #endregion			
        
        #region Methods

        public override Stream OpenRead(Type configType, ConfigurationOptionsAttribute options)
        {
            return File.OpenRead(GetPath(configType, options));
        }

        public override Stream OpenWrite(Type configType, ConfigurationOptionsAttribute options)
        {
            return File.OpenWrite(GetPath(configType, options));
        }

        public override bool Exists(Type configType, ConfigurationOptionsAttribute options)
        {
            return File.Exists(GetPath(configType, options));
        }

        public override void Delete(Type configType, ConfigurationOptionsAttribute options)
        {
            string path = GetPath(configType, options);

            if (File.Exists(path))
                File.Delete(path);
        }

        public override void Dispose() { }

        #endregion			
    }
}
