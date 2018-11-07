using iie;
using NUnit.Framework;
using System.Collections.Generic;
using CalculatorController = asp.Controllers.Calculator;

namespace asptest.Calculator
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public abstract class CalculatorTestBase : SmcTest<CalculatorController, CalculatorContext, CalculatorContext.CalculatorState>
    {
        protected Stack<string> Stack
        {
            get { return this.Controller.Stack; }
        }
    }
}