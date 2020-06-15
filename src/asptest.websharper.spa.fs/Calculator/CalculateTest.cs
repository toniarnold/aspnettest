using asp.websharper.spa.fs;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using static asplib.View.TagHelper;

namespace asptest.Calculator
{
    [TestFixture(typeof(InternetExplorerDriver))]
    public class CalculateTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void NavigateIndexTest()
        {
            this.Navigate("/");
            Assert.Multiple(() =>
            {
                this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Splash)); // "early binding"
                Assert.That(this.Html(), Does.Contain("RPN calculator SPA"));
                Assert.That(this.Html(), Does.Contain("Map1.Splash"));          // even later than "late binding"
            });
        }

        [Test]
        public void InitEnterTest()
        {
            this.Click(Id(CalculatorDoc.EnterButton));
            Assert.Multiple(() =>
            {
                this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Enter));
                Assert.That(this.Html(), Does.Contain("Map1.Enter"));
            });
        }

        [Test]
        public void EnterTest()
        {
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Write(Id(CalculatorDoc.OperandTextbox), "1");
            this.Click(Id(CalculatorDoc.EnterButton));
            Assert.Multiple(() =>
            {
                this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("1"));
                Assert.That(this.Stack.Count, Is.EqualTo(1));
                Assert.That(this.Html(), Does.Contain("<ul><li>1</li></ul>"));
            });
        }
    }
}