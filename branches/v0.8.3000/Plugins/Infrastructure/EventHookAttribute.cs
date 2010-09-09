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

namespace Virtuoso.Miranda.Plugins.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class EventHookAttribute : HookAttribute
    {
        #region Fields

        private string eventName;
        public string EventName
        {
            get
            {
                return this.eventName;
            }
            set
            {
                this.eventName = value;
            }
        }

        internal override string HookName
        {
            get { return EventName; }
        }

        internal override HookType HookType
        {
            get { return HookType.EventHook; }
        }

        #endregion

        #region .ctors

        public EventHookAttribute(string eventName)
        {
            if (eventName == null) 
                throw new ArgumentNullException("eventName");

            this.eventName = eventName;
        }

        #endregion        
    }
}
