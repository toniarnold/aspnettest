using asp.calculator.Control;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;

namespace asptest.webforms.specflow.Drivers
{
    public abstract class CalculatorTestBase<TWebDriver> :
        SmcDbTest<EdgeDriver, Calculator,
            CalculatorContext, CalculatorContext.CalculatorState>
        where TWebDriver : IWebDriver, new()
    {
        public Stack<string> Stack
        {
            get { return this.MainControl.Main.Stack; }
        }
    }
}