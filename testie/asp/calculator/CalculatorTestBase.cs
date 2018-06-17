using asp.calculator.Control;
using iie;
using System.Collections.Generic;

namespace testie.asp.calculator
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    public abstract class CalculatorTestBase : SmcTest<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
    {
        protected Stack<string> Stack
        {
            get { return this.MainControl.Main.Stack; }
        }
    }
}