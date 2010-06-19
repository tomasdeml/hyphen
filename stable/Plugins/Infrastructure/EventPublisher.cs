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
    public abstract class EventPublisher
    {
        internal EventPublisher() { }

        protected bool InvokeChainCancelable<TEventArgs>(MirandaEventHandler<TEventArgs> handler, TEventArgs eventArgs) where TEventArgs : MirandaEventArgs
        {
            return InvokeChainCancelable<TEventArgs>(handler, this, eventArgs);
        }

        public static bool InvokeChainCancelable<TEventArgs>(MirandaEventHandler<TEventArgs> handler, object sender, TEventArgs eventArgs) where TEventArgs : MirandaEventArgs
        {
            bool retValue = EventResult.HonourEventChain;

            if (handler != null)
            {
                foreach (MirandaEventHandler<TEventArgs> del in handler.GetInvocationList())
                {
                    if ((retValue = del(sender, eventArgs)) == EventResult.BreakEventChain)
                        break;
                }
            }

            return retValue;
        }
    }
}
