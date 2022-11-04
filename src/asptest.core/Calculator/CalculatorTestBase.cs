using asplib.Model.Db;
using iselenium;
using NUnit.Framework;
using System.Collections.Generic;
using CalculatorController = asp.Controllers.CalculatorController;

namespace asptest.Calculator
{
#pragma warning disable CS0618 // IIE obsolete

    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public abstract class CalculatorTestBase : SmcTest<CalculatorController, CalculatorContext, CalculatorContext.CalculatorControllerState>,
                            IDeleteNewRows
#pragma warning restore CS0618 // IIE obsolete
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