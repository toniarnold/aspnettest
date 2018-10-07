﻿using asplib.View;
using iie;
using NUnit.Framework;

namespace testie.asp.calculator
{
    /// <summary>
    /// Extends CalculatorTest by using Session instead of ViewState as storage
    /// and executes the same tests declared in the base class.
    /// </summary>
    [Category("SHDocVw.InternetExplorer")]
    public class WithSessionTest : CalculateTest
    {
        [OneTimeSetUp]
        public void SetUpStorage()
        {
            ControlStorageExtension.SessionStorage = Storage.Session;
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            ControlStorageExtension.SessionStorage = null;
        }

        /// <summary>
        /// Session must be cleared after each single test such that the app behaves like the ViewState Test
        /// </summary>
        [TearDown]
        public void ClearSession()
        {
            this.Navigate("/asp.webforms/default.aspx?clear=true&endresponse=true");
        }

        /// <summary>
        /// Reload the page, session storage should survive
        /// </summary>
        private void Reload()
        {
            this.Navigate("/asp.webforms/default.aspx");
        }

        /// <summary>
        /// Same as AddTest(), but with page reload before each action.
        /// </summary>
        [Test]
        public void AddSessionPersistsTest()
        {
            this.Navigate("/asp.webforms/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write("enter.operandTextBox", "2");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Reload();
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Reload();
            this.Click("calculate.addButton");
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
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
                Assert.That(this.Stack.Peek(), Is.EqualTo("5"));
                Assert.That(this.Stack.Count, Is.EqualTo(before - 1));
                Assert.That(this.Html(), Does.Contain(" 5\n"));
                this.Navigate("/asp.webforms/default.aspx");
            });
        }
    }
}