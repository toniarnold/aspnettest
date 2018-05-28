using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using iie;

using asplib.View;


namespace testie.asp
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public class CalculatorDatabaseTest : CalculatorViewstateTest
    {
        [OneTimeSetUp]
        public override void SetUpStorage()
        {
            ControlMainExtension.SessionStorage = Storage.Database;
        }

        /// <summary>
        /// Database must be cleared after each single test such that the app behaves like the Viewstate Test
        /// </summary>
        [TearDown]
        public void ClearDatabase()
        {
            this.Navigate("/asp/default.aspx?clear=true&endresponse=true");
        }

        /// <summary>
        /// Restart internet explorer and navigate to the page, session storage should survive
        /// </summary>
        private void RestartIE()
        {
            this.TearDownIE();
            this.SetUpIE();
            this.Navigate("/asp/default.aspx");
        }

        /// <summary>
        /// Same as AddTest(), but with internet explorer restar before each action.
        /// </summary>
        [Test]
        public void AddSessionPersistsTest()
        {
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.RestartIE();
            this.Write("enter.operandTextBox", "2");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.RestartIE();
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.RestartIE();
            this.Write("enter.operandTextBox", "3");
            this.Click("footer.enterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.RestartIE();
            this.Click("calculate.addButton");
            this.AssertAddFinalState(before);
            this.RestartIE();
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
                this.Navigate("/asp/default.aspx");
            });
        }
    }
}
