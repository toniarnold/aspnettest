using asplib.Model.Db;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace iselenium
{
    /// <summary>
    /// Base class for Selenium tests with a [OneTimeSetUp] / [OneTimeTearDown] pair
    /// for starting/stopping the browser, a [OneTimeSetUp] / [OneTimeTearDown]
    /// pair for Storage.Database which cleans up newly added Main rows and
    /// accessors for IPersistentController.
    /// </summary>
    /// <typeparam name="TWebDriver">Selenium WebDriver</typeparam>
    [Category("ASP_DB")]
    public abstract class SpaDbTest<TWebDriver> : SpaTest<TWebDriver>, IDeleteNewRows
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// IDeleteNewRows
        /// </summary>
        public List<(string, string, object)> MaxIds { get; set; } = new();

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
}