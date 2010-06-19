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
using Virtuoso.Hyphen;
using Virtuoso.Miranda.Plugins.Resources;

namespace Virtuoso.Miranda.Plugins
{
    [Flags]
    public enum LoaderOptions : int
    {
        None = 0,
        HasCustomApiExports = 2,
        CannotBeUnloaded = 4,
    }

    internal enum LoaderOptionsOwner
    {
        Type,
        Assembly
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class LoaderOptionsAttribute : Attribute
    {
        #region Fields

        private LoaderOptions options;
        public LoaderOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        private Version requiredVersion;
        public Version RequiredVersion
        {
            get { return requiredVersion; }
        }

        private Version minimalMirandaVersion;
        public Version MinimalMirandaVersion
        {
            get { return minimalMirandaVersion; }
        }

        #endregion

        #region .ctors

        public LoaderOptionsAttribute(LoaderOptions options)
            : this(null, null, options) { }

        public LoaderOptionsAttribute(string requiredVersion)
            : this(requiredVersion, null, LoaderOptions.None) { }

        public LoaderOptionsAttribute(string requiredVersion, LoaderOptions options)
            : this(requiredVersion, null, options) { }

        public LoaderOptionsAttribute(string requiredVersion, string minimalMirandaVersion)
            : this(requiredVersion, minimalMirandaVersion, LoaderOptions.None) { }

        public LoaderOptionsAttribute(string requiredVersion, string minimalMirandaVersion, LoaderOptions options)
        {
            if (!String.IsNullOrEmpty(requiredVersion))
                this.requiredVersion = new Version(requiredVersion);

            if (!String.IsNullOrEmpty(minimalMirandaVersion))
                this.minimalMirandaVersion = new Version(minimalMirandaVersion);

            this.options = options;
        }

        #endregion

        #region Methods

        internal bool SupportsMirandaVersion(uint mirandaVersion)
        {
            return SupportsMirandaVersion(Translate.FromMirandaVersion(mirandaVersion));
        }

        internal bool SupportsMirandaVersion(Version mirandaVersion)
        {
            // If the plugin has a min supported Miranda info..
            if (MinimalMirandaVersion != null)
            {
                if (mirandaVersion < MinimalMirandaVersion)
                    return false;
            }
            // If not, use Hyphen's...
            else if (!Loader.SupportsMirandaVersion(mirandaVersion))
                return false;

            return true;
        }

        internal static LoaderOptionsAttribute Get(Type pluginType, LoaderOptionsOwner target)
        {
            if (pluginType == null)
                throw new ArgumentNullException("pluginType");

            Type thisType = typeof(LoaderOptionsAttribute);
            LoaderOptionsAttribute result = null;

            switch (target)
            {
                case LoaderOptionsOwner.Type:
                    foreach (LoaderOptionsAttribute attrib in pluginType.GetCustomAttributes(thisType, true))
                    {
                        if (result == null)
                            result = attrib;
                        else
                        {
                            if (result.requiredVersion == null)
                                result.requiredVersion = attrib.requiredVersion;
                            else if (attrib.RequiredVersion != null)
                                throw new NotSupportedException(TextResources.ExceptionMsg_DuplicitLoaderOptions);

                            if (result.minimalMirandaVersion == null)
                                result.minimalMirandaVersion = attrib.minimalMirandaVersion;
                            else if (attrib.MinimalMirandaVersion != null)
                                throw new NotSupportedException(TextResources.ExceptionMsg_DuplicitLoaderOptions);                             
                        }

                        result.options |= attrib.options;
                    }
                    break;
                case LoaderOptionsOwner.Assembly:
                    if (pluginType.Assembly.IsDefined(thisType, false))
                        result = (LoaderOptionsAttribute)pluginType.Assembly.GetCustomAttributes(thisType, false)[0];
                    break;
                default:
                    throw new ArgumentOutOfRangeException("target");
            }

            return result ?? new LoaderOptionsAttribute(LoaderOptions.None);
        }

        #endregion
    }
}
