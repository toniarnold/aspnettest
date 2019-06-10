using asp.websharper.spa.Remoting;
using iie;
using NUnit.Framework;
using System.Collections.Generic;

namespace asptest.websharper.spa.Calculator
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public abstract class CalculatorTestBase : IETest
    {
        protected CalculatorContext Fsm
        {
            get { return CalculatorServer.Calculator.Fsm; }
        }

        protected CalculatorContext.CalculatorState State
        {
            get { return CalculatorServer.Calculator.State; }
        }

        protected Stack<string> Stack
        {
            get { return CalculatorServer.Calculator.Stack; }
        }
    }
}