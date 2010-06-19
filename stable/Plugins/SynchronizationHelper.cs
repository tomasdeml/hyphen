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
using System.Threading;
using Virtuoso.Miranda.Plugins.Infrastructure;
using System.Collections;

namespace Virtuoso.Miranda.Plugins
{
    /* Impl note: I've introduced this class to clearly define the locking semantics for particular objects, 
     * classes not covered don't have to be 'suspended'. */
    internal static class SynchronizationHelper
    {
        #region Helpers

        private static void VerifyNotNull(object obj)
        {
            if (obj == null) 
                throw new ArgumentNullException();
        }

        #endregion

        #region BeginXXXUpdate Methods

        public static void EndUpdate(object obj)
        {
            VerifyNotNull(obj);
            Monitor.Exit(obj);
        }

        public static void BeginPluginUpdate(MirandaPlugin plugin)
        {
            VerifyNotNull(plugin);
            Monitor.Enter(plugin);
        }

        public static void BeginDescriptorUpdate(IDescriptor descriptor)
        {
            VerifyNotNull(descriptor);
            Monitor.Enter(descriptor);
        }

        public static void BeginMenuItemUpdate(MenuItemDeclarationAttribute item)
        {
            VerifyNotNull(item);
            Monitor.Enter(item);
        }

        public static void BeginCollectionUpdate(IList collection)
        {
            VerifyNotNull(collection);
            Monitor.Enter(collection);
        }

        public static void BeginCollectionUpdate(IDictionary dictionary)
        {
            VerifyNotNull(dictionary);
            Monitor.Enter(dictionary);
        }

        public static void BeginHandleUpdate(IMirandaObject handle)
        {
            VerifyNotNull(handle);
            Monitor.Enter(handle);
        }

        #endregion
    }
}
