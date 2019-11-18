using asplib.Model.Db;
using iie;
using NUnit.Framework;
using System.Collections.Generic;
using CalculatorController = asp.Controllers.CalculatorController;

namespace asptest.Calculator
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public abstract class CalculatorTestBase : SmcTest<CalculatorController, CalculatorContext, CalculatorContext.CalculatorControllerState>,
                            IDeleteNewRows
    {
        /// <summary>
        /// IDeleteNewRows
        /// </summary>
        public List<(string, string, object)> MaxIds { get; set; }

        /// <summary>
        /// Demeter Principle Stack accessor for assertions
        /// </summary>
        protected Stack<string> Stack
        {
            get { return this.Controller.Stack; }
        }

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