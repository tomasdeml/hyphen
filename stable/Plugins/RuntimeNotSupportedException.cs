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
using System.Runtime.Serialization;
using Virtuoso.Miranda.Plugins.Resources;
using Virtuoso.Hyphen;

namespace Virtuoso.Miranda.Plugins
{
    internal sealed class RuntimeNotSupportedException : NotSupportedException
    {
        #region Fields

        private Version requiredVersion, availableVersion;

        public Version AvailableVersion
        {
            get { return availableVersion; }
        }

        public Version RequiredVersion
        {
            get { return requiredVersion; }
        }

        #endregion

        #region .ctors

        public RuntimeNotSupportedException(Type pluginType, Version requiredVersion)
            : this(pluginType, requiredVersion, true) { }

        public RuntimeNotSupportedException(Type pluginType, Version requiredVersion, bool isHyphenVersion) 
            : base(String.Format(TextResources.ExceptionMsg_Formatable2_RuntimeVersionNotAvailable, 
            (pluginType == null ? String.Empty : pluginType.FullName), 
            (requiredVersion == null ? String.Empty : String.Format("{0} {1}", (!isHyphenVersion ? "Miranda" : "Hyphen"), requiredVersion))))
        {
            if (requiredVersion == null) 
                throw new ArgumentNullException("requiredVersion");

            this.requiredVersion = requiredVersion;
            this.availableVersion = Loader.HyphenVersion;
        }

        private RuntimeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion
    }
}
