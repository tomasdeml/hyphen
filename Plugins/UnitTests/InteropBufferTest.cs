﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Virtuoso.Miranda.Plugins.Native;
using System.Threading;
using System.Runtime.InteropServices;
namespace Virtuoso.Miranda.Plugins.UnitTests
{
    /// <summary>
    ///This is a test class for Virtuoso.Miranda.Plugins.Native.InteropBuffer and is intended
    ///to contain all Virtuoso.Miranda.Plugins.Native.InteropBuffer Unit Tests
    ///</summary>
    [TestClass()]
    public class InteropBufferTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Equals (object)
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            InteropBuffer_Accessor target;
            InteropBuffer_Accessor obj;
            bool expected;
            bool actual;

            target = new InteropBuffer_Accessor(1);

            obj = null;
            expected = false;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual, "Virtuoso.Miranda.Plugins.Native.InteropBuffer.Equals did not return the expected value.");

            obj = new InteropBuffer_Accessor(1);
            expected = false;
            actual = target.Equals(obj);
            obj.Lock();
            ((IDisposable)obj).Dispose();
            obj.Unlock();
            Assert.AreEqual(expected, actual, "Virtuoso.Miranda.Plugins.Native.InteropBuffer.Equals did not return the expected value.");

            obj = target;
            expected = true;
            actual = target.Equals(obj);

            target.Lock();
            ((IDisposable)target).Dispose();
            target.Unlock();

            Assert.AreEqual(expected, actual, "Virtuoso.Miranda.Plugins.Native.InteropBuffer.Equals did not return the expected value.");
        }

        /// <summary>
        ///A test for IntPtr
        ///</summary>
        [TestMethod()]
        public void IntPtrTest()
        {
            InteropBuffer_Accessor target = new InteropBuffer_Accessor(1);

            target.Lock();
            IntPtr ptr = target.IntPtr;
            target.Unlock();

            try
            {
                ptr = target.IntPtr;
                Assert.Fail("InteropBuffer::IntPtr succeded without locking.");
            }
            catch { }

            try
            {
                target.Lock();
                ((IDisposable)target).Dispose();

                ptr = target.IntPtr;
                target.Unlock();

                Assert.Fail("InteropBuffer::IntPtr succeded after disposal.");
            }
            catch { }
        }

        /// <summary>
        ///A test for Lock ()
        ///</summary>
        [TestMethod()]
        public void LockTest()
        {
            InteropBuffer_Accessor target = new InteropBuffer_Accessor(1);

            try
            {
                target.Lock();
                Thread t = new Thread(new ThreadStart(delegate { target.Lock(); }));
                t.Start();
                Thread.Sleep(250);

                Assert.IsFalse(t.Join(1000), "InteropBuffer::Lock does not block other threads.");
                Assert.IsTrue(target.Locked, "InteropBuffer::Lock did not set the lock owner.");
            }
            finally
            {
                ((IDisposable)target).Dispose();
                target.Unlock();
            }
        }

        /// <summary>
        ///A test for Locked
        ///</summary>
        [TestMethod()]
        public void LockedTest()
        {
            InteropBuffer_Accessor target = new InteropBuffer_Accessor(1);
            Assert.IsFalse(target.Locked, "InteropBuffer::Locked returned wrong result.");

            target.Lock();
            Assert.IsTrue(target.Locked, "InteropBuffer::Locked returned wrong result.");
            target.Unlock();
        }

        /// <summary>
        ///A test for Unlock ()
        ///</summary>
        [TestMethod()]
        public void UnlockTest()
        {
            InteropBuffer_Accessor target = new InteropBuffer_Accessor(1);

            try
            {
                target.Unlock();
                Assert.Fail("InteropBuffer::Unlock allows to unlock non-locked buffers.");
            }
            catch { }

            target.Lock();
            target.Unlock();
            Assert.IsFalse(target.Locked, "InteropBuffer::Unlock does not update the Locked property.");

            Thread t = new Thread(new ParameterizedThreadStart(delegate(object obj)
            {
                target.Lock();
            }));
            t.Start();
            t.Join();

            try
            {
                target.Unlock();
                Assert.Fail("InteropBuffer::Unlock allows an unlock from a different thread.");
            }
            catch { }
            finally
            {
                target.Dispose(true);
            }
        }

        /// <summary>
        ///A test for Zero ()
        ///</summary>
        [TestMethod()]
        public void ZeroTest()
        {
            InteropBuffer_Accessor target = new InteropBuffer_Accessor(1);

            try
            {
                target.Zero();
                Assert.Fail("InteropBuffer::Zero allows unlocked use.");
            }
            catch { }

            target.Lock();

            Marshal.WriteByte(target.IntPtr, 0xaf);
            target.Zero();

            try
            {
                Assert.AreEqual<byte>(0, Marshal.ReadByte(target.IntPtr));
            }
            finally
            {
                ((IDisposable)target).Dispose();
                target.Unlock();
            }
        }

        /// <summary>
        ///A test for InteropBuffer (int)
        ///</summary>
        [TestMethod(), ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorTest()
        {
            int capacity = 0;
            InteropBuffer_Accessor target = new InteropBuffer_Accessor(capacity);
        }
    }
}
