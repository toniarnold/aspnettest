using asp.calculator.Control;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;

namespace asptest.webforms.specflow.Drivers
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [TestFixture]
    public abstract class CalculatorTestBase : SmcDbTest<EdgeDriver, Calculator, CalculatorContext, CalculatorContext.CalculatorState>
    {
        public Stack<string> Stack
        {
            get { return this.MainControl.Main.Stack; }
        }
    }
}