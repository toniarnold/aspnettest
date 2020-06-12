using asp.websharper.spa.fs;
using asplib.Model;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using static asplib.View.TagHelper;

namespace asptest.Calculator
{
    [TestFixture(typeof(InternetExplorerDriver))]
    public class WithDatabaseTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        public override void OneTimeSetUpBrowser()
        {
            // Set overriding storage before the browser gets started
            StorageImplementation.SessionStorage = Storage.Database;
            base.OneTimeSetUpBrowser();
            this.Navigate("/", delay: 1000, pause: 1000); // defensively load twice for restoring state
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        [TearDown]
        public void ClearDatabase()
        {
            this.OneTimeTearDownDatabase();  // unneeded with only one test
        }

        /// <summary>
        /// Reload the page, session storage should survive
        /// </summary>
        private void RestartBrowser()
        {
            this.TearDownBrowser();
            this.OneTimeSetUpBrowser();
        }

        /// <summary>
        /// Same as AddTest(), but with browser restart before each action.
        /// </summary>
        [Test]
        public void AddWithPersistenceTest()
        {
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Enter));
            this.RestartBrowser();
            this.Write(Id(CalculatorDoc.OperandTextbox), "2");
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.RestartBrowser();
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Enter));
            this.RestartBrowser();
            this.Write(Id(CalculatorDoc.OperandTextbox), "3");
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.RestartBrowser();
            this.Click(Id(CalculatorDoc.AddButton));
            this.AssertAddFinalState(before);
            this.RestartBrowser();
            this.AssertAddFinalState(before);
        }

        private void AssertAddFinalState(int before)
        {
            Assert.Multiple(() =>
            {
                this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain("<ul><li>5</li></ul>"));
                this.Navigate("/");
            });
        }
    }
}