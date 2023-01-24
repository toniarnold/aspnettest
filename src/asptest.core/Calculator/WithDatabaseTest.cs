using asplib.Model;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;

namespace asptest.Calculator
{
    [TestFixture]
    public class WithDatabaseTest : CalculateTest
    {
        [OneTimeSetUp]
        public void SetUpStorage()
        {
            StorageImplementation.SessionStorage = Storage.Database;
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        /// <summary>
        /// Database must be cleared after each single test such that the app behaves like the ViewState Test
        /// </summary>
        [TearDown]
        public void ClearDatabase()
        {
            this.Navigate("/?clear=true");
        }

        /// <summary>
        /// Restart Internet Explorer and navigate to the page, database storage should survive.
        /// This worked only with Internet Explorer which didn't run in private mode with selenium.
        /// </summary>
        private void RestartIE()
        {
            this.TearDownBrowser();
            this.SetUpBrowser<EdgeDriver>();
            this.Navigate("/");
        }

        /// <summary>
        /// Reload the page, database storage should survive
        /// </summary>
        private void Reload()
        {
            this.Navigate("/");
        }

        /// <summary>
        /// Same as AddTest(), but with Internet Explorer restart before each action.
        /// </summary>
        [Test]
        public void AddWithPersistenceTest()
        {
            this.Navigate("/");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write("Operand", "2");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Reload();
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Reload();
            this.Click("AddButton");
            this.AssertAddFinalState(before);
            this.Reload();
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
                Assert.That(this.Html(), Does.Contain(" 5\r\n"));
            });
        }
    }
}