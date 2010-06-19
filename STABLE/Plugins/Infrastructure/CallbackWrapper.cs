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

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    internal class CallbackWrapper
    {
        #region Properties

        private string serviceName;
        public string ServiceName
        {
            get { return serviceName; }
            protected set { serviceName = value; }
        }

        #endregion

        #region .ctors

        private CallbackWrapper(string serviceName)
        {
            if (String.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("serviceName");

            this.serviceName = serviceName;
        }

        public static Callback Create(string serviceName)
        {
            return new Callback(new CallbackWrapper(serviceName).Callback);
        }

        #endregion

        #region Methods

        protected int Callback(UIntPtr wParam, IntPtr lParam)
        {
            return MirandaContext.Current.CallService(ServiceName, wParam, lParam);
        }

        #endregion
    }
}
