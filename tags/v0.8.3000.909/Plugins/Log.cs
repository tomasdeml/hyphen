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
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Virtuoso.Miranda.Plugins
{
    internal static class Log
    {
        #region Properties

        private static TraceSwitch traceSwitch;
        public static TraceSwitch TraceSwitch
        {
            get { return traceSwitch; }
        }

        #endregion

        #region .ctors

        static Log()
        {
            traceSwitch = new TraceSwitch("HyphenTracing", "Hyphen Tracing", "Warning");
        } 

        #endregion

        [Conditional("DEBUG")]
        public static void DebuggerWrite(int priority, string source, string message)
        {
            Debugger.Log(priority, source, message);
        }

        public static void Warning(string message, string category, params string[] formatArgs)
        {
            Trace.WriteLineIf(TraceSwitch.TraceWarning, String.Format(message, formatArgs), category);
        }
    }
}
