using asp.calculator.Control;
using iselenium;
using NUnit.Framework;
using System.Collections.Generic;

namespace asptest.calculator
{
#pragma warning disable CS0618 // IIE obsolete

    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]  // NUnit CHANGES.txt: * 655882 	Make CategoryAttribute inherited
    public abstract class CalculatorTestBase : SmcTest<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
#pragma warning restore CS0618 // IIE obsolete
    {
        protected Stack<string> Stack
        {
            get { return this.MainControl.Main.Stack; }
        }
    }
}