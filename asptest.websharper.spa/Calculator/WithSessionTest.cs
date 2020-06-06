using asp.websharper.spa.Client;
using asplib.Model;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using static asplib.View.TagHelper;

namespace asptest.Calculator
{
    public class WithSessionTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [OneTimeSetUp]
        public void SetUpStorage()
        {
            StorageImplementation.SessionStorage = Storage.Session;
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        /// <summary>
        /// Session must be cleared after each single test such that the app behaves like the ViewState Test
        /// </summary>
        [TearDown]
        public void ClearSession()
        {
            this.Navigate("/?clear=true");
        }

        /// <summary>
        /// Reload the page, session storage should survive
        /// </summary>
        private void Reload()
        {
            this.Navigate("/");
        }

        /// <summary>
        /// Same as AddTest(), but with page reload before each action.
        /// </summary>
        [Test]
        public void AddWithPersistenceTest()
        {
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write(Id(CalculatorDoc.OperandTextbox), "2");
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Reload();
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write(Id(CalculatorDoc.OperandTextbox), "3");
            this.Click(Id(CalculatorDoc.EnterButton));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Reload();
            this.Click(Id(CalculatorDoc.AddButton));
            this.AssertAddFinalState(before);
            this.Reload();
        }

        /// <summary>
        /// Assert twice, once after reloading
        /// </summary>
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