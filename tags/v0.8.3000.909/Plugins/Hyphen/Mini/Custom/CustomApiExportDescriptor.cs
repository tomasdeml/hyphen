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

namespace Virtuoso.Hyphen.Mini.Custom
{
    public delegate object CustomApiExportCallback(object[] args);

    public sealed class CustomApiExportDescriptor
    {
        #region Fields

        private readonly string exportName;
        public string ExportName
        {
            get { return exportName; }
        }

        private readonly CustomApiExportCallback callback;
        public CustomApiExportCallback Callback
        {
            get { return callback; }
        } 

        #endregion

        #region .ctors

        public CustomApiExportDescriptor(string exportName, CustomApiExportCallback callback)
        {
            if (String.IsNullOrEmpty(exportName)) throw new ArgumentNullException("exportName");
            if (callback == null) throw new ArgumentNullException("callback");

            this.exportName = exportName;
            this.callback = callback;
        }

        #endregion

        #region Methods

        internal T Execute<T>(params object[] args)
        {
            object result = callback(args);

            if (result != null)
                return (T)result;
            else
                return (T)default(T);
        }

        #endregion
    }
}
