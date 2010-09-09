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
using System.Runtime.InteropServices;
using Virtuoso.Miranda.Plugins.Infrastructure;

namespace Virtuoso.Miranda.Plugins.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct DBCONTACTWRITESETTING
    {
        #region Fields

        public IntPtr Module;
        public IntPtr Name;

        public DBVARIANT Value; 

        #endregion

        #region Methods

        public static unsafe object ExtractValue(IntPtr pDbWriteSetting)
        {
            DBCONTACTWRITESETTING dbWriteSetting = *(DBCONTACTWRITESETTING*)pDbWriteSetting.ToPointer();

            switch ((DatabaseSettingType)dbWriteSetting.Value.Type)
            {
                case DatabaseSettingType.AsciiString:
                    return Marshal.PtrToStringAnsi(dbWriteSetting.Value.Text.TextPtr);
                case DatabaseSettingType.UnicodeString:
                case DatabaseSettingType.UTF8String:
                    return Marshal.PtrToStringUni(dbWriteSetting.Value.Text.TextPtr);
                case DatabaseSettingType.Byte:
                    return dbWriteSetting.Value.Primitives.Byte;
                case DatabaseSettingType.UInt16:
                    return dbWriteSetting.Value.Primitives.Word;
                case DatabaseSettingType.UInt32:
                    return dbWriteSetting.Value.Primitives.DWord;
                case DatabaseSettingType.Blob:
                    return Translate.ToBlob(dbWriteSetting.Value.Blob.BlobPtr, dbWriteSetting.Value.Blob.Size);
                case DatabaseSettingType.Deleted:
                    return null;
                default:
                    LogUnsupportedValue(dbWriteSetting);
                    return null;
            }
        }

        private static void LogUnsupportedValue(DBCONTACTWRITESETTING dbWriteSetting)
        {
            string name = "N/A";
            string module = "N/A";

            try
            {
                name = Translate.ToString(dbWriteSetting.Name, StringEncoding.Ansi);
                module = Translate.ToString(dbWriteSetting.Module, StringEncoding.Ansi);
            }
            catch (Exception)
            { }

            Log.Warning("Attempted to extract an unsupported DB contact setting {0}:{1} of type {1}",
                        module, name, dbWriteSetting.Value.Type.ToString("X"));
        } 

        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct DBCONTACTGETSETTING
    {
        public string Module;
        public string Name;

        public IntPtr DbVariantPtr;
    }

    /*[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct DBCONTACTGETSETTING_PTR
    {
        public IntPtr Module;
        public IntPtr Name;

        public IntPtr DbVariant;
    }*/
}
