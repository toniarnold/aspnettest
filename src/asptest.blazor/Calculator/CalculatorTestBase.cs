using iselenium;
using OpenQA.Selenium;

namespace asptest.Calculator
{
    public abstract class CalculatorTestBaset<TWebDriver> : SpaDbTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
    }
}