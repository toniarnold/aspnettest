using asplib.View;
using iie;
using NUnit.Framework;

namespace asptest.calculator
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public class WithDatabaseTest : CalculateTest
    {
        [OneTimeSetUp]
        public void SetUpStorage()
        {
            ControlStorageExtension.SessionStorage = Storage.Database;
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            ControlStorageExtension.SessionStorage = null;
        }

        /// <summary>
        /// Database must be cleared after each single test such that the app behaves like the ViewState Test
        /// </summary>
        [TearDown]
        public void ClearDatabase()
        {
            this.Navigate("/asp.webforms/default.aspx?clear=true&endresponse=true");
        }

        /// <summary>
        /// Restart Internet Explorer and navigate to the page, database storage should survive
        /// </summary>
        private void RestartIE()
        {
            this.TearDownIE();
            this.SetUpIE();
            this.Navigate("/asp.webforms/default.aspx");
        }

        /// <summary>
        /// Same as AddTest(), but with Internet Explorer restart before each action.
        /// </summary>
        [Test]
        public void AddSessionPersistsTest()
        {
            this.Navigate("/asp.webforms/default.aspx");
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
            this.AssertAddFinalState(before);
        }

        /// <summary>
        /// Assert twice, first time at the end, then a second time after RestartIE()
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