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

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public sealed class ServiceCallInterceptionManager
    {
        #region Fields

        private readonly Dictionary<string, Callback> Interceptors;

        #endregion

        #region .ctors

        public ServiceCallInterceptionManager()
        {
            this.Interceptors = new Dictionary<string, Callback>(2);
        }

        #endregion

        #region Methods

        public bool RequiresInterception(string serviceName)
        {
            lock (Interceptors)
                return Interceptors.ContainsKey(serviceName);
        }

        public Callback this[string serviceName]
        {
            get
            {
                lock (Interceptors)
                {
                    Callback interceptor = null;
                    Interceptors.TryGetValue(serviceName, out interceptor);

                    return interceptor;
                }
            }
            set
            {
                Register(serviceName, value);
            }
        }

        public void Register(string serviceName, Callback interceptor)
        {
            lock (Interceptors)
                Interceptors[serviceName] = interceptor;
        }

        public void Unregister(string serviceName)
        {
            lock (Interceptors)
                Interceptors.Remove(serviceName);
        }

        #endregion
    }
}
