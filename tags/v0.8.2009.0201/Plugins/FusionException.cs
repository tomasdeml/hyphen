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
using System.Reflection;
using Virtuoso.Miranda.Plugins.Infrastructure;
using Virtuoso.Miranda.Plugins.Resources;
using System.Runtime.Serialization;

namespace Virtuoso.Miranda.Plugins
{
    [Serializable]
    public class FusionException : Exception, IExceptionDumpController
    {
        #region Fields

        private readonly Assembly assembly;
        private readonly Type pluginType;
        private readonly MirandaPlugin instantiatedPlugin;
        private readonly string fusionLog;

        #endregion

        #region .ctors

        public FusionException(string message, Assembly assembly, Type type, MirandaPlugin plugin, Exception inner)
            : this(message, TextResources.UI_Label_Empty, assembly, type, plugin, inner)
        { }

        public FusionException(string message, string fusionLog, Assembly assembly, Type type, MirandaPlugin plugin, Exception inner) : base(message, inner)
        {
            this.assembly = assembly;
            this.pluginType = type;
            this.instantiatedPlugin = plugin;
            this.fusionLog = fusionLog;
        }

        protected FusionException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        #endregion

        #region Properties

        public Assembly Assembly
        {
            get { return assembly; }
        }

        public Type PluginType
        {
            get { return pluginType; }
        }

        public MirandaPlugin InstantiatedPlugin
        {
            get { return instantiatedPlugin; }
        }

        public string FusionLog
        {
            get { return fusionLog; }
        }

        #endregion

        #region IExceptionDumpController Members

        void IExceptionDumpController.DumpException(Exception e, StringBuilder dump)
        {
            FusionException ex = (FusionException)e;

            dump.AppendFormat("=== Description ==={0}{1}{0}{0}", Environment.NewLine, ex.Message);
            dump.AppendFormat("=== Assembly ==={0}{1}{0}{0}", Environment.NewLine, ex.Assembly == null ? TextResources.UI_Label_Unknown : ex.Assembly.ToString());
            dump.AppendFormat("=== Type ==={0}{1}{0}{0}", Environment.NewLine, ex.PluginType == null ? TextResources.UI_Label_Unknown : ex.PluginType.FullName);
            dump.AppendFormat("=== Fusion log ==={0}{1}{0}{0}", Environment.NewLine, ex.FusionLog);
        }

        #endregion
    }
}
