using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

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
            Navigate("/");
            Assert.Multiple(() =>
            {
                Assert.That(Html(), Does.Contain("RPN calculator"));
                Assert.That(Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
            });
        }

        [Test]
        public void InitEnterTest()
        {
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.Multiple(() =>
            {
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
                Assert.That(this.Html(), Does.Contain("Map1.Enter"));
            });
        }
    }
}