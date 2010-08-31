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
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    public static class LanguagePack
    {
        #region Enums

        private enum LanguagePackEncoding : ushort
        {
            Ansi = 0,
            Unicode = 0x1000
        }

        #endregion

        #region Constants

        private const string MS_LANGPACK_TRANSLATESTRING = "LangPack/TranslateString";

        #endregion

        #region FromPointer methods

        public static string TranslateString(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            UnmanagedStringHandle stringHandle = UnmanagedStringHandle.Empty;

            try
            {
                StringEncoding mirandaEncoding = MirandaEnvironment.MirandaStringEncoding;
                LanguagePackEncoding encoding = (mirandaEncoding == StringEncoding.Unicode ? LanguagePackEncoding.Unicode : LanguagePackEncoding.Ansi);

                stringHandle = new UnmanagedStringHandle(str, mirandaEncoding);
                IntPtr translatedPtr = (IntPtr)MirandaContext.Current.CallService(MS_LANGPACK_TRANSLATESTRING, (UIntPtr)encoding, stringHandle.IntPtr);

                return translatedPtr == stringHandle.IntPtr ? str : Translate.ToString(translatedPtr, mirandaEncoding);
            }
            catch (Exception e)
            {
                throw new MirandaException(TextResources.ExceptionMsg_ErrorWhileCallingMirandaService, e);
            }
            finally
            {
                stringHandle.Free();
            }
        }

        #endregion
    }
}
