using asp.blazor.Components.CalculatorParts;
using asplib.Model;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace asptest.CalculatorTest
{
    [TestFixture(typeof(EdgeDriver))]
    public class WithDatabaseTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [OneTimeSetUp]
        public void SetUpStorage()
        {
            StorageImplementation.SetStorage(Storage.Database);
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        /// <summary>
        /// Reload the page, data storage should survive - we can't restart the
        /// browser no more with modern selenium as it starts the browser in
        /// private mode
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
            Navigate("/");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter)); // persisted
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "2");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Reload();
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3");
            Click(Component.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Reload();
            this.Click(Dynamic<Calculate>(Component.calculatorPart).addButton);
            this.AssertAddFinalState(before);
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
                Assert.That(this.Html(), Does.Contain("5"));
            });
        }
    }
}