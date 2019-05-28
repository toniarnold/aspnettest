using asplib.Model;
using iie;
using NUnit.Framework;

namespace asptest.Calculator
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
        public void AddSessionPersistsTest()
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
                this.Navigate("/");
            });
        }
    }
}