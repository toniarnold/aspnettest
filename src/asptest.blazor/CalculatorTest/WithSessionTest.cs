using asp.blazor.Components.CalculatorParts;
using asplib.Model;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace asptest.CalculatorTest
{
    //[TestFixture(typeof(ChromeDriver))]
    //[TestFixture(typeof(FirefoxDriver))]
    //[TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(EdgeDriver))]
    public class WithSessionTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [OneTimeSetUp]
        public void SetUpStorage()
        {
            StorageImplementation.SetStorage(Storage.SessionStorage);
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            StorageImplementation.SessionStorage = null;
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
            Navigate("/");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter)); // persisted
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "2");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            this.Reload();
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
            this.Reload();
            this.Write(Dynamic<Enter>(Cut.calculatorPart).operand, "3");
            Click(Cut.footer.enterButton);
            Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Calculate));
            var before = this.Stack.Count;
            this.Reload();
            this.Click(Dynamic<Calculate>(Cut.calculatorPart).addButton);
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