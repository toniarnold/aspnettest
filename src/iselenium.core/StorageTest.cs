using asplib.Model.Db;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;

namespace iselenium
{
    /// <summary>
    /// Base class for Browser tests with a [OneTimeSetUp] / [OneTimeTearDown] pair
    /// for Storage.Database which cleans up newly added Main rows.
    /// Call SetUpStorage() to configure the storage for that specific test suite.
    /// Provides accessors for IStorageControl
    public abstract class StorageTest<TWebDriver, TController> : SeleniumTest<TWebDriver, TController>, IDeleteNewRows
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// IDeleteNewRows
        /// </summary>
        public List<(string, string, object)> MaxIds { get; set; }

        /// <summary>
        /// Remember the last row in [Main] before the tests started
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUpDatabase()
        {
            this.SelectMaxId(ASP_DBEntities.ConnectionString, "Main", "mainid");
        }

        /// <summary>
        /// Delete any rows in [Main] that have been added since the test start
        /// </summary>
        [OneTimeTearDown]
        public void OneTimeTearDownDatabase()
        {
            this.DeleteNewRows(ASP_DBEntities.ConnectionString);
        }
    }

    /// <summary>
    /// Base class for IE tests with a [OneTimeSetUp] / [OneTimeTearDown] pair
    /// for Storage.Database which cleans up newly added Main rows.
    /// Call SetUpStorage() to configure the storage for that specific test suite.
    /// Provides accessors for IStorageControl
    [Obsolete("Replaced by StorageTest<InternetExplorerDriver, TMain>")]
    [TestFixture]
    public abstract class StorageTest<TController> : StorageTest<InternetExplorerDriver, TController>, IIE, ISelenium, IDeleteNewRows
    {
    }
}