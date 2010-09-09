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
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Virtuoso.Miranda.Plugins.Collections
{
    public sealed class ProtocolDictionary : Dictionary<string, Protocol>
    {
        #region .ctors

        internal ProtocolDictionary(int count) : base(count) { }

        #endregion

        #region Indexers

        public new Protocol this[string key]
        {
            get
            {
                bool notFound = (key == null || !ContainsKey(key));
                Debug.Assert(!notFound);

                if (notFound)
                    return Protocol.UnknownProtocol;

                return base[key];
            }
        }

        #endregion
    }
}
