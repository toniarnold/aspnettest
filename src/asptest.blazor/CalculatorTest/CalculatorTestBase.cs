using asp.blazor.Components;
using iselenium;
using OpenQA.Selenium;
using asp.blazor.CalculatorSmc;

namespace asptest.CalculatorTest
{
    public abstract class CalculatorTestBase<TWebDriver> :
        SmcComponentDbTest<TWebDriver, CalculatorComponent, Calculator, CalculatorContext, CalculatorContext.CalculatorState>
        where TWebDriver : IWebDriver, new()
    {
    }
}