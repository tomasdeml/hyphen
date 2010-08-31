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
using System.Runtime.InteropServices;

namespace Virtuoso.Miranda.Plugins.ThirdParty.Updater
{
    public class Update
    {
        #region Fields

        private readonly string pluginName;

        public string PluginName
        {
            get { return pluginName; }
        }

        private readonly Version currentVersion;

        public Version CurrentVersion
        {
            get { return currentVersion; }
        }

        private readonly Uri updateUrl, versionUrl;
        private Uri betaVersionUrl, betaUpdateUrl, betaChangelogUrl;

        public Uri BetaChangelogUrl
        {
            get { return betaChangelogUrl; }
            set { betaChangelogUrl = value; }
        }

        public Uri BetaUpdateUrl
        {
            get { return betaUpdateUrl; }
            set { betaUpdateUrl = value; }
        }

        public Uri BetaVersionUrl
        {
            get { return betaVersionUrl; }
            set { betaVersionUrl = value; }
        }

        public Uri VersionUrl
        {
            get { return versionUrl; }
        } 

        public Uri UpdateUrl
        {
            get { return updateUrl; }
        }

        private readonly string versionTextPrefix;
        private string betaVersionTextPrefix;

        public string BetaVersionTextPrefix
        {
            get { return betaVersionTextPrefix; }
            set { betaVersionTextPrefix = value; }
        }

        public string VersionTextPrefix
        {
            get { return versionTextPrefix; }
        }

        #endregion

        #region .ctors

        public Update(MirandaPlugin plugin, Uri updateUrl, Uri versionUrl, string versionTextPrefix)
        {
            if (plugin == null) throw new ArgumentNullException("plugin");
            if (updateUrl == null) throw new ArgumentNullException("updateUrl");
            if (versionUrl == null) throw new ArgumentNullException("versionUrl");
            if (versionTextPrefix == null) throw new ArgumentNullException("versionTextPrefix");

            this.pluginName = plugin.Name;
            this.currentVersion = plugin.Version;
            this.updateUrl = updateUrl;
            this.versionUrl = versionUrl;
            this.versionTextPrefix = versionTextPrefix;
        }

        internal void MarshalToNative(out UPDATE update)
        {
            update = new UPDATE();
            update.Size = Marshal.SizeOf(typeof(UPDATE));

            update.ComponentName = pluginName;
            update.UpdateUrl = updateUrl.ToString();

            update.VersionUrl = versionUrl.ToString();
            update.VersionPrefix = versionTextPrefix;
            update.VersionPrefixLength = versionTextPrefix.Length;

            update.Version = currentVersion.ToString(4);
            update.VersionLength = update.Version.Length;            

            update.BetaChangelogUrl = betaChangelogUrl != null ? betaChangelogUrl.ToString() : null;
            update.BetaUpdateUrl = betaUpdateUrl != null ? betaUpdateUrl.ToString() : null;
            update.BetaVersionUrl = betaVersionUrl != null ? betaVersionUrl.ToString() : null;
            update.BetaVersionPrefix = betaVersionTextPrefix;
            update.BetaVersionPrefixLength = betaVersionTextPrefix != null ? betaVersionTextPrefix.Length : 0;            
        }

        #endregion
    }
}

/*
    Update update = {0};
	char szVersion[16];
	update.cbSize = sizeof(Update);

	update.szComponentName = pluginInfo.shortName;
	update.pbVersion = (BYTE *)CreateVersionString(&pluginInfo, szVersion);
	update.cpbVersion = strlen((char *)update.pbVersion);

	update.szUpdateURL = BETA_HOST_URL_PREFIX "/ver_updater_unicode.zip";
	update.szVersionURL = BETA_HOST_URL_PREFIX "/updater_unicode.html";
	update.pbVersionPrefix = (BYTE *)"Updater (Unicode) version ";
	update.cpbVersionPrefix = strlen((char *)update.pbVersionPrefix);
*/
