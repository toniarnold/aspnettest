using asp.websharper.spa.Model;
using asp.websharper.spa.Remoting;
using iselenium;
using OpenQA.Selenium;
using System.Collections.Generic;
using ModelCalculator = asp.websharper.spa.Model.Calculator;

namespace asptest.Calculator
{
    /// <summary>
    /// Concrete base class for the Calculator with additional specific accessors
    /// </summary>
    public class CalculatorTestBase<TWebDriver> : SpaSmcTest<TWebDriver, CalculatorViewModel,
                                                    ModelCalculator, CalculatorContext, CalculatorContext.CalculatorState>
        where TWebDriver : IWebDriver, new()
    {
        protected override CalculatorViewModel GetViewModel()
        {
            return CalculatorServer.CalculatorViewModel;
        }

        protected Stack<string> Stack
        {
            get { return this.Main.Stack; }
        }
    }
}