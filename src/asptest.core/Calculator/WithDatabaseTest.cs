using asplib.Model;
using iselenium;
using NUnit.Framework;

namespace asptest.Calculator
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
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
        /// Restart Internet Explorer and navigate to the page, database storage should survive
        /// </summary>
        private void RestartIE()
        {
            this.TearDownIE();
            this.SetUpIE();
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
            this.RestartIE();
            this.Write("Operand", "2");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.RestartIE();
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.RestartIE();
            this.Write("Operand", "3");
            this.Click("EnterButton");
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.RestartIE();
            this.Click("AddButton");
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
            });
        }
    }
}