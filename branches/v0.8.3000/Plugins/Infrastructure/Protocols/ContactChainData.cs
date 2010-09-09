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
using Virtuoso.Miranda.Plugins.Native;

namespace Virtuoso.Miranda.Plugins.Infrastructure.Protocols
{
    public sealed class ContactChainData
    {
        #region .ctors

        internal unsafe ContactChainData(IntPtr pCcsData)
        {
            if (pCcsData == IntPtr.Zero)
                throw new ArgumentNullException("pCssData");

            this.ccsDataPtr = pCcsData;

            CCSDATA ccsData = *(CCSDATA*)pCcsData.ToPointer();
            this.contactInfo = ContactInfo.FromHandle(ccsData.ContactHandle);
            this.wParam = ccsData.WParam;
            this.lParam = ccsData.LParam;
            this.serviceName = Translate.ToString(ccsData.ServiceNamePtr, StringEncoding.Ansi);
        }

        #endregion

        #region Properties

        private ContactInfo contactInfo;
        public ContactInfo ContactInfo
        {
            get { return contactInfo; }
        }

        private string serviceName;
        public string ServiceName
        {
            get { return serviceName; }
        }

        private UIntPtr wParam;
        [CLSCompliant(false)]
        public UIntPtr WParam
        {
            get { return wParam; }
        }

        private IntPtr lParam;
        public IntPtr LParam
        {
            get { return lParam; }
        }

        private IntPtr ccsDataPtr;
        internal IntPtr CcsDataPtr
        {
            get { return ccsDataPtr; }
        }

        #endregion        

        #region Methods

        public string GetLParamAsString(StringEncoding encoding)
        {
            return Translate.ToString(LParam, encoding);
        }

        #endregion
    }
}
