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
using Virtuoso.Miranda.Plugins.Resources;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Virtuoso.Miranda.Plugins.Native;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [CLSCompliant(false)]
    public static class Translate
    {
        #region Fields

        private static readonly Type StatusEnumType = typeof(StatusMode);

        #endregion

        #region ToStatus

        public static StatusMode ToStatus(UIntPtr wParam)
        {
            if (!Enum.IsDefined(StatusEnumType, (int)wParam.ToUInt32()))
                throw new ArgumentException("wParam", TextResources.ExceptionMsg_InvalidValueToTranslate);

            return (StatusMode)(wParam);
        }

        #endregion

        #region ToString

        public static string ToString(IntPtr lParam, StringEncoding marshalAs)
        {
            return ToString(lParam, 0, marshalAs, false);
        }

        public static string ToString(IntPtr lParam, StringEncoding marshalAs, bool transformExceptionsToNull)
        {
            return ToString(lParam, 0, marshalAs, transformExceptionsToNull);
        }

        public static string ToString(IntPtr lParam, int length, StringEncoding marshalAs)
        {
            return ToString(lParam, length, marshalAs, false);
        }

        public static string ToString(IntPtr lParam, int length, StringEncoding marshalAs, bool tranformExceptionsToNull)
        {
            if (lParam == IntPtr.Zero) 
                throw new ArgumentNullException("lParam");

            if (length < 0) 
                throw new ArgumentOutOfRangeException("length");

            if (marshalAs != StringEncoding.MirandaDefault && marshalAs != StringEncoding.Ansi && marshalAs != StringEncoding.Unicode)
                throw new ArgumentOutOfRangeException("marshalAs");                       

            try
            {
                reEval:
                switch (marshalAs)
                {
                    case StringEncoding.MirandaDefault:
                        marshalAs = MirandaEnvironment.MirandaStringEncoding;
                        if (marshalAs == StringEncoding.MirandaDefault) throw new ArgumentException(TextResources.ExceptionMsg_CannotDetectMirandaDefaultStringEncoding);
                        goto reEval;
                    case StringEncoding.Ansi:
                        if (length > 0)
                            return Marshal.PtrToStringAnsi(lParam, length);
                        else
                            return Marshal.PtrToStringAnsi(lParam);
                    case StringEncoding.Unicode:
                        if (length > 0)
                            return Marshal.PtrToStringUni(lParam, length);
                        else
                            return Marshal.PtrToStringUni(lParam);                    
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                if (!tranformExceptionsToNull)
                    throw new ArgumentException("lParam", TextResources.ExceptionMsg_InvalidValueToTranslate + e.Message, e);
                else
                    return null;
            }
        }

        #endregion

        #region ToHandle

        public static IntPtr ToHandle(UIntPtr wParam)
        {
            return new IntPtr((long)wParam.ToUInt64());
        }

        public static UIntPtr ToHandle(IntPtr lParam)
        {
            return new UIntPtr((ulong)lParam.ToInt64());
        }

        public static UnmanagedStringHandle ToHandle(string str, StringEncoding encoding)
        {
            return new UnmanagedStringHandle(str, encoding);
        }

        #endregion

        #region To/From Miranda HyphenVersion Format

        [CLSCompliant(false)]
        public static uint ToMirandaVersion(Version version)
        {
            if (version == null) 
                throw new ArgumentNullException("version");         
   
            return (((((uint)(version.Major)) & 0xFF) << 24) | ((((uint)(version.Minor)) & 0xFF) << 16) | ((((uint)(version.Build)) & 0xFF) << 8) | (((uint)(version.MinorRevision)) & 0xFF));
        }

        [CLSCompliant(false)]
        public static Version FromMirandaVersion(uint version)
        {
            return new Version((int)((version >> 24) & 0xff), (int)((version >> 16) & 0xff), (int)((version >> 8) & 0xff), (int)(version & 0xff));
        }

        #endregion

        #region Misc.

        public static byte[] ToBlob(IntPtr blobPtr, int size)
        {
            if (blobPtr == IntPtr.Zero)
                throw new ArgumentNullException("blobPtr");

            if (size < 0)
                throw new ArgumentOutOfRangeException("size");

            byte[] blob = new byte[size];
            Marshal.Copy(blobPtr, blob, 0, size);

            return blob;
        }

        #endregion

        #region Internal helpers

        internal static object ValueFromVariant(ref DBVARIANT dbVariant)
        {
            switch ((DbVariantValue)dbVariant.Type)
            {
                case DbVariantValue.DBVT_ASCIIZ:
                    return Translate.ToString(dbVariant.Text.TextPtr, dbVariant.Text.TextBufferSize, StringEncoding.Ansi);
                case DbVariantValue.DBVT_UTF8:
                    return Translate.ToString(dbVariant.Text.TextPtr, dbVariant.Text.TextBufferSize, StringEncoding.Unicode);
                case DbVariantValue.DBVT_BLOB:
                    return ToBlob(dbVariant.Blob.BlobPtr, dbVariant.Blob.Size);
                case DbVariantValue.DBVT_BYTE:
                    return dbVariant.Primitives.Byte;
                case DbVariantValue.DBVT_DELETED:
                    return null;
                case DbVariantValue.DBVT_DWORD:
                    return dbVariant.Primitives.DWord;
                case DbVariantValue.DBVT_WORD:
                    return dbVariant.Primitives.Word;
                case DbVariantValue.DBVT_WCHAR:
                    return Marshal.PtrToStringBSTR(dbVariant.Text.TextPtr);
                case DbVariantValue.DBVTF_VARIABLELENGTH:
                    throw new NotSupportedException();
                default:
                    return null;
            }
        }      

        #endregion
    }
}
