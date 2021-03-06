﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Virtuoso.Miranda.Plugins;
using System.Reflection;
namespace Virtuoso.Miranda.Plugins.UnitTests
{
    /// <summary>
    ///This is a test class for Virtuoso.Miranda.Plugins.PluginDescriptor and is intended
    ///to contain all Virtuoso.Miranda.Plugins.PluginDescriptor Unit Tests
    ///</summary>
    [TestClass()]
    public class PluginDescriptorTest
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
        [TestMethod(), ExpectedException(typeof(InvalidOperationException))]
        public void EqualsTest()
        {
            MirandaPlugin plugin1 = Virtuoso_Miranda_Plugins_MirandaPlugin_EmptyPluginAccessor.CreatePrivate();
            MirandaPlugin plugin2 = Virtuoso_Miranda_Plugins_MirandaPlugin_EmptyPluginAccessor.CreatePrivate();

            PluginDescriptor descriptor1 = Virtuoso_Miranda_Plugins_PluginDescriptorAccessor.CreatePrivate(plugin1);
            PluginDescriptor descriptor2 = Virtuoso_Miranda_Plugins_PluginDescriptorAccessor.CreatePrivate(plugin2);

            Assert.AreEqual<bool>(true, descriptor1.Equals(descriptor2), "PluginDescriptor.Equals returned wrong result.");
            Assert.AreEqual<bool>(false, descriptor1.Equals(null), "PluginDescriptor.Equals returned wrong result.");
            Assert.AreEqual<bool>(false, descriptor1.Equals(new object()), "PluginDescriptor.Equals returned wrong result.");

            Virtuoso_Miranda_Plugins_PluginDescriptorAccessor.CreatePrivate(plugin1);
        }

        /// <summary>
        ///A test for PluginDescriptor (MirandaPlugin, bool)
        ///</summary>
        [TestMethod(), ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTest()
        {
            Virtuoso_Miranda_Plugins_PluginDescriptorAccessor.CreatePrivate(null);
        }
    }
}
