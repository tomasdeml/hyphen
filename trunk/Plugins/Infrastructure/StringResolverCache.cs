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
using Virtuoso.Miranda.Plugins.Helpers;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    internal sealed class StringResolverCache : TypeInstanceCache<IStringResolver>
    {
        #region Fields

        private static StringResolverCache singleton;

        #endregion

        #region .ctors

        private StringResolverCache() { }

        #endregion

        #region Properties

        public static StringResolverCache Singleton
        {
            get
            {
                return singleton ?? (singleton = new StringResolverCache());
            }
        }

        #endregion
    }
}
