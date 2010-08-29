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

namespace Virtuoso.Miranda.Plugins.Collections
{
    /* DO NOT USE FIND... OR CONTAINS... METHODS THAT ACCEPT A TYPE INSTANCE FROM ANOTHER APPDOMAIN,
     * YOU MAY LEAK ITS ASSEMBLY TO THAT DOMAIN. */
    public sealed class PluginDescriptorCollection : List<PluginDescriptor>
    {
        #region .ctors

        internal PluginDescriptorCollection() : base(3) { }

        #endregion

        #region Find methods

        public PluginDescriptor FindDescriptorOf(MirandaPlugin plugin)
        {
            if (plugin == null) 
                throw new ArgumentNullException("plugin");

            foreach (PluginDescriptor descriptor in this)
                if (descriptor.Plugin == plugin) return descriptor;

            return null;
        }

        public PluginDescriptor FindDescriptorOf(Type pluginType)
        {
            if (pluginType == null) 
                throw new ArgumentNullException("plugin");

            if (!pluginType.IsSubclassOf(DefaultPluginManager.PluginType)) 
                return null;

            foreach (PluginDescriptor descriptor in this)
                if (descriptor.Plugin.GetType() == pluginType) return descriptor;

            return null;
        }

        #endregion

        #region Contains methods

        public bool ContainsDescriptorOf(MirandaPlugin plugin)
        {
            return (FindDescriptorOf(plugin) != null);
        }

        public bool ContainsDescriptorOf(Type pluginType)
        {
            return (FindDescriptorOf(pluginType) != null);
        }

        #endregion
    }
}
