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
using Virtuoso.Hyphen.Mini;
using Virtuoso.Miranda.Plugins;
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Hyphen.Native;

namespace Virtuoso.Hyphen.Mini.Custom
{
    public abstract class CustomizedDatabaseDriver : DatabaseDriver
    {
        #region .ctors

        protected CustomizedDatabaseDriver() { }

        #endregion

        #region Thunks

        protected override sealed int GetCapabilityThunk(int flags)
        {
            return Convert.ToInt32(GetCapability(flags));
        }

        protected override sealed int GetFriendlyNameThunk(IntPtr buffer, int size, int shortName)
        {
            string name = GetFriendlyName(shortName != 0, size);
            if (name == null) return -1;

            byte[] bytes = Encoding.Default.GetBytes(name);

            if (size < bytes.Length)
                return -1;
            else
            {
                Marshal.Copy(bytes, 0, buffer, bytes.Length);
                return 0;
            }
        }

        protected override sealed int InitThunk(string profile, IntPtr pLink)
        {
            return Init(profile, pLink);
        }

        protected override sealed int UnloadThunk(int wasLoaded)
        {
            return Unload(wasLoaded != 0);
        }

        #endregion

        #region Abstract Members

        protected abstract bool GetCapability(int flags);

        protected abstract string GetFriendlyName(bool shortName, int size);

        protected abstract int MakeDatabase(string profile, ref int error);

        protected abstract int GrokHeader(string profile, ref int error);

        protected abstract int Init(string profile, IntPtr pLink);

        protected abstract int Unload(bool wasLoaded);

        #endregion
    }
}
