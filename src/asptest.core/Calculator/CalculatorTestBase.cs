using asplib.Model.Db;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;
using CalculatorController = asp.Controllers.CalculatorController;

namespace asptest.Calculator
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [TestFixture]
    public abstract class CalculatorTestBase : SmcDbTest<EdgeDriver, CalculatorController, CalculatorContext, CalculatorContext.CalculatorControllerState>,
                            IDeleteNewRows
    {
        /// <summary>
        /// Demeter Principle Stack accessor for assertions
        /// </summary>
        protected Stack<string> Stack
        {
            get { return this.Controller.Stack; }
        }
    }
}