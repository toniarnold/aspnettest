using asp.blazor.CalculatorSmc;
using asp.blazor.Components;
using iselenium;
using OpenQA.Selenium;

namespace asptest.blazor.specflow.Drivers
{
    public abstract class CalculatorTestBase<TWebDriver> :
        SmcComponentDbTest<TWebDriver, CalculatorComponent, Calculator,
            CalculatorContext, CalculatorContext.CalculatorState>
        where TWebDriver : IWebDriver, new()
    {
        public Stack<string> Stack
        {
            get { return Main.Stack; }
        }
    }
}