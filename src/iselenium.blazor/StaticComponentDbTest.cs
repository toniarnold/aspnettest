using asplib.Components;
using asplib.Model.Db;
using NUnit.Framework;
using OpenQA.Selenium;

namespace iselenium
{
    /// <summary>
    /// Test fixture for a StaticComponentBase Component which is expected to
    /// write persistent Main objects to the database. New entries are cleaned
    /// up afterwards. The TestFocus is set to the Component type and its
    /// instance is assigned to the Component property at OnAfterRenderAsync.
    /// </summary>
    /// <typeparam name="TWebDriver"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    public class StaticComponentDbTest<TWebDriver, TComponent> : StaticComponentTest<TWebDriver, TComponent>, IDeleteNewRows
        where TWebDriver : IWebDriver, new()
        where TComponent : IStaticComponent
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