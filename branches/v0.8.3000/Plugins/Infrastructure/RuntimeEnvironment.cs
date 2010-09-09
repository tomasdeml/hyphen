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
using Virtuoso.Hyphen;
using System.Threading;

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    /// <summary>
    /// Provides information about current state of Hyphen runtime. 
    /// This class can be used only from the context of standalone modules (i.e. from the default AppDomain only).
    /// </summary>
    public static class RuntimeEnvironment
    {
        #region Fields

        private static volatile bool Initialized;

        #endregion

        #region Methods

        /// <summary>
        /// Marks the class initialized.
        /// </summary>
        internal static void Initialize()
        {
            Initialized = true;
        }

        /// <summary>
        /// Verifies whether the class was initialized (from the default AppDomain).
        /// </summary>
        /// <exception cref="NotSupportedException">Class not initialized (i.e. called from other than default AppDomain).</exception>
        private static void VerifyInitialized()
        {
            if (!Initialized)
                throw new NotSupportedException();
        }

        #endregion

        #region Properties

        private static bool hyphenIsLoading;

        /// <summary>
        /// Gets an indication whether the Hyphen runtime is currently loading.
        /// </summary>
        /// <exception cref="NotSupportedException">Class not initialized (i.e. called from other than default AppDomain).</exception>
        public static bool HyphenIsLoading
        {
            get { VerifyInitialized(); return hyphenIsLoading; }
            internal set { hyphenIsLoading = value; }
        }

        /// <summary>
        /// Gets an indication whether the Isolated plugins are loaded.
        /// </summary>
        /// <exception cref="NotSupportedException">Class not initialized (i.e. called from other than default AppDomain).</exception>
        public static bool IsolatedModePluginsLoaded
        {
            get { VerifyInitialized(); return Loader.GetInstance().PluginsLoaded; }
        }

        #endregion
    }
}
