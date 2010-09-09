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
using System.Runtime.InteropServices;

namespace Virtuoso.Miranda.Plugins.ThirdParty.Updater.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct UPDATE
    {
        public int Size;

        public string ComponentName, VersionUrl;

        public string VersionPrefix;
        public int VersionPrefixLength;

        public string UpdateUrl, BetaVersionUrl;

        public string BetaVersionPrefix;
        public int BetaVersionPrefixLength;
        public string BetaUpdateUrl;

        public string Version;
        public int VersionLength;

        public string BetaChangelogUrl;
    }
}

/*
  typedef struct Update_tag {
      int cbSize;     
      char *szComponentName;		// component name as it will appear in the UI (will be translated before displaying)

      char *szVersionURL;			// URL where the current version can be found (NULL to disable)
      BYTE *pbVersionPrefix;		// bytes occuring in VersionURL before the version, used to locate the version information within the URL data
								    // (note that this URL could point at a binary file - dunno why, but it could :)
      int cpbVersionPrefix;			// number of bytes pointed to by pbVersionPrefix
      char *szUpdateURL;			// URL where dll/zip is located
								    // set to UPDATER_AUTOREGISTER if you want Updater to find the file listing URLs (ensure plugin shortName matches file listing!)

      char *szBetaVersionURL;		// URL where the beta version can be found (NULL to disable betas)
      BYTE *pbBetaVersionPrefix;	// bytes occuring in VersionURL before the version, used to locate the version information within the URL data
      int cpbBetaVersionPrefix;		// number of bytes pointed to by pbVersionPrefix
      char *szBetaUpdateURL;		// URL where dll/zip is located

      BYTE *pbVersion;				// bytes of current version, used for comparison with those in VersionURL
      int cpbVersion;				// number of bytes pointed to by pbVersion

      char *szBetaChangelogURL;		// url for displaying changelog for beta versions
  } Update;
*/
