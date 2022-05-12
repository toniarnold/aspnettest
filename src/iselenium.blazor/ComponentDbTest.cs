using asplib.Model.Db;
using NUnit.Framework;
using OpenQA.Selenium;

namespace iselenium
{
    public class ComponentDbTest<TWebDriver, TComponent> : ComponentTest<TWebDriver, TComponent>, IDeleteNewRows
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