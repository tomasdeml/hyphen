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
using Virtuoso.Miranda.Plugins.ThirdParty.Updater.Native;
using Virtuoso.Miranda.Plugins.Native;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins.ThirdParty.Updater
{
    public static class UpdaterPlugin
    {
        private const string MS_UPDATE_REGISTER = "Update/Register";

        public static bool IsUpdateSupported()
        {
            return ServiceManager.ServiceExists(MS_UPDATE_REGISTER);
        }

        public static void RegisterForUpdate(Update update)
        {
            if (update == null) throw new ArgumentNullException("update");
            if (!IsUpdateSupported()) throw new NotSupportedException();

            UnmanagedStructHandle<UPDATE> updateHandle = UnmanagedStructHandle<UPDATE>.Empty;

            try
            {
                UPDATE updateNative;
                update.MarshalToNative(out updateNative);

                updateHandle = new UnmanagedStructHandle<UPDATE>(ref updateNative);
                int result = MirandaContext.Current.CallService(MS_UPDATE_REGISTER, UIntPtr.Zero, updateHandle.IntPtr);

                if (result != 0)
                    throw new MirandaException(String.Format(TextResources.ExceptionMsg_Formatable2_MirandaServiceReturnedFailure, MS_UPDATE_REGISTER, result.ToString()));
            }
            catch (MirandaException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new MirandaException(TextResources.ExceptionMsg_ErrorWhileCallingMirandaService, e);
            }
            finally
            {
                updateHandle.Free();
            }
        }
    }
}