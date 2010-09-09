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
using System.Threading;
using System.Runtime.CompilerServices;

namespace Virtuoso.Miranda.Plugins.Helpers
{
    /* CLASS ORIGIN: Spearhead project */
    public class MessageQueue
    {
        #region Fields

        private volatile bool enabled, suspended;

        private readonly Queue<KeyValuePair<ContactInfo, string>> queue = new Queue<KeyValuePair<ContactInfo, string>>(5);
        private Thread QueueThread;

        private readonly ManualResetEvent waitHandle = new ManualResetEvent(true);

        #endregion

        #region Enums

        protected enum CommonWaitTime : int
        {
            QueueSuspension = 50,
            QueueItemProcessed = 1000,
            QueueProcessed = 1000,            
        }

        #endregion

        #region Events

        public event EventHandler MessageSending;
        public event EventHandler MessageSent;

        #endregion

        #region .ctors & .dctors

        public MessageQueue()
        {
            QueueThread = InitializeQueueThread();
        }

        protected virtual Thread InitializeQueueThread()
        {
            Thread thread = new Thread(ProcessQueue);
            thread.IsBackground = true;

            return thread;
        }

        ~MessageQueue()
        {
            SetState(false);
            waitHandle.Close();
        }

        #endregion

        #region Properties

        public bool Suspended
        {
            get
            {
                return suspended;
            }
        }

        public ManualResetEvent WaitHandle
        {
            get
            {
                return waitHandle;
            }
        }

        protected Queue<KeyValuePair<ContactInfo, string>> Queue
        {
            get { return queue; }
        }

        public bool Enabled
        {
            get
            {
                return enabled;
            }
        }

        public bool QueueHasItems
        {
            get
            {
                lock (Queue)
                    return Queue.Count > 0;
            }
        }

        protected virtual bool ClearQueueWhenDisabled
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Virtuals

        protected virtual void ProcessQueue()
        {
            while (Enabled)
            {
                while (suspended)
                    Wait(CommonWaitTime.QueueSuspension);

                lock (Queue)
                {
                    while (QueueHasItems)
                    {
                        waitHandle.Reset();
                        RaiseMessageForwardingEvent();

                        DequeueAndSendMessage();
                        Wait(QueueItemProcessedWaitTime);

                        RaiseMessageForwardedEvent();
                    }

                    waitHandle.Set();
                }

                Wait(QueueProcessedWaitTime);
            }

            waitHandle.Set();
        }

        protected void DequeueAndSendMessage()
        {
            lock (Queue)
            {
                KeyValuePair<ContactInfo, string> data = Queue.Dequeue();
                SendMessage(data.Key, data.Value);
            }
        }

        protected virtual void SendMessage(ContactInfo recipient, string message)
        {
            recipient.SendMessage(message);
        }

        protected virtual int QueueItemProcessedWaitTime
        {
            get
            {
                return (int)CommonWaitTime.QueueItemProcessed;
            }
        }

        protected virtual int QueueProcessedWaitTime
        {
            get
            {
                return (int)CommonWaitTime.QueueProcessed;
            }
        }

        #endregion

        #region Methods

        public void SuspendQueue()
        {
            suspended = true;
        }

        public void ResumeQueue()
        {
            suspended = false;
        }

        public void EnqueueMessage(ContactInfo to, string message)
        {
            lock (queue)
                queue.Enqueue(new KeyValuePair<ContactInfo, string>(to, message));

            SetState(true);
        }

        public void SetState(bool enabled)
        {
            // Queue is used as a sync object here...
            lock (Queue)
            {
                if (enabled)
                {
                    if ((QueueThread.ThreadState & ThreadState.Stopped) == ThreadState.Stopped)
                        QueueThread = InitializeQueueThread();

                    if ((QueueThread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
                        QueueThread.Start();
                }
                else
                {
                    Queue.Clear();
                }

                this.enabled = enabled;
            }
        }

        protected void RaiseMessageForwardingEvent()
        {
            if (MessageSending != null)
                MessageSending(this, EventArgs.Empty);
        }

        protected void RaiseMessageForwardedEvent()
        {
            if (MessageSent != null)
                MessageSent(this, EventArgs.Empty);
        }

        protected void Wait(CommonWaitTime miliseconds)
        {
            Wait((int)miliseconds);
        }

        protected void Wait(int miliseconds)
        {
            Thread.Sleep(miliseconds);
        }

        #endregion
    }
}
