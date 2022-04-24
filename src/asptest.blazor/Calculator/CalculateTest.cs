using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace asptest.Calculator
{
    //[TestFixture(typeof(ChromeDriver))]
    //[TestFixture(typeof(FirefoxDriver))]
    //[TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(EdgeDriver))]
    public class CalculateTest<TWebDriver> : CalculatorTestBaset<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/");
            Assert.Multiple(() =>
            {
                this.AssertPoll(() => this.Html(), () => Does.Contain("RPN calculator"));
                this.AssertPoll(() => this.Html(), () => Does.Contain("Map1.Splash"));          // even later than "late binding"
            });
        }
    }
}