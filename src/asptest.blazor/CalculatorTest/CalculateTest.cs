using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace asptest.CalculatorTest
{
    //[TestFixture(typeof(ChromeDriver))]
    //[TestFixture(typeof(FirefoxDriver))]
    //[TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(EdgeDriver))]
    public class CalculateTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/");
            Assert.Multiple(() =>
            {
                Assert.That(Html(), Does.Contain("RPN calculator"));
                Assert.That(Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
            });
        }
    }
}