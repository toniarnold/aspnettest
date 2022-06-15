using asp.blazor.CalculatorSmc;
using asp.blazor.Components;
using iselenium;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace asptest.CalculatorTest
{
    public abstract class CalculatorTestBase<TWebDriver> :
        SmcComponentDbTest<TWebDriver, CalculatorComponent, Calculator, CalculatorContext, CalculatorContext.CalculatorState>
        where TWebDriver : IWebDriver, new()
    {
        protected Stack<string> Stack
        {
            get { return this.Main.Stack; }
        }
    }
}