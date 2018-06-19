using asp.calculator.Control;
using iie;
using NUnit.Framework;
using System.Collections.Generic;

namespace testie.asp.calculator
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [Category("SHDocVw.InternetExplorer")]  // NUnit CHANGES.txt: * 655882 	Make CategoryAttribute inherited
    public abstract class CalculatorTestBase : SmcTest<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
    {
        protected Stack<string> Stack
        {
            get { return this.MainControl.Main.Stack; }
        }
    }
}