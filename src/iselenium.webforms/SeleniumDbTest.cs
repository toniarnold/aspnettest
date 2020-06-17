using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium tests with a [OneTimeSetUp] / [OneTimeTearDown]
    /// pair for starting/stopping the browser, a [OneTimeSetUp] /
    /// [OneTimeTearDown] pair for Storage.Database which cleans up newly added
    /// Main rows and accessors for IStorageControl
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    /// <typeparam name="TMain">the class Main under test</typeparam>
    [Category("ASP_DB")]
    public abstract class SeleniumDbTest<TWebDriver, TMain> : SeleniumTest<TWebDriver, TMain>, IDatabase
        where TWebDriver : IWebDriver, new()
        where TMain : new()
    {
        /// <summary>
        /// Remember the last row in [Main] before the tests started
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUpDatabase()
        {
            this.SetUpDatabase();
        }

        /// <summary>
        /// Delete any rows in [Main] that have been added since the test start
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDownDatabase()
        {
            this.TearDownDatabase();
        }
    }

    /// <summary>
    /// Base class for IE tests with a [OneTimeSetUp] / [OneTimeTearDown] pair
    /// for Storage.Database which cleans up newly added Main rows.
    /// Call SetUpStorage() to configure the storage for that specific test suite.
    /// Provides accessors for IStorageControl
    [Obsolete("Replaced by SeleniumDbTest<InternetExplorerDriver, TMain>")]
    [TestFixture]
    public abstract class StorageTest<TMain> : SeleniumDbTest<InternetExplorerDriver, TMain>, IIE, ISelenium, IDatabase
        where TMain : new()
    {
    }
}