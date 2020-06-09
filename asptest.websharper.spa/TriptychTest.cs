using asp.websharper.spa.Client;
using asplib.Model;
using asptest.Calculator;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using static asplib.View.TagHelper;

namespace asptest
{
    [TestFixture(typeof(InternetExplorerDriver))]
    public class TriptychTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [SetUp]
        public void UnsetStorage()
        {
            StorageImplementation.SessionStorage = null;    // defaults to ViewState
        }

        [TearDown]
        public void ClearStorage()
        {
            StorageImplementation.SessionStorage = Storage.Database;
            this.Navigate("/?clear=true");
            StorageImplementation.SessionStorage = Storage.Session;
            this.Navigate("/?clear=true");
            StorageImplementation.SessionStorage = null;
        }

        [Test]
        public void NavigateTriptychTest()
        {
            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
        }

        /// <summary>
        /// Assert the presence of the three calculators superficially by text.
        /// </summary>
        private void AssertTriptychHtml()
        {
            Assert.Multiple(() =>
            {
                // Assert from bottom to  top to ensure the page has been fully rendered on the client
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Database"));
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Session"));
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));
            });
        }

        [Test]
        public void CircumambulateStorageTypes()
        {
            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
            this.Click(Id(TriptychDoc.DatabaseDoc, CalculatorDoc.StorageLink));
            // Poll SessionStorage first, this.Html() is ambiguous and succeeds too early (still on the triptych)
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.Database));
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Database"));

            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
            this.Click(Id(TriptychDoc.SessionDoc, CalculatorDoc.StorageLink));
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.Session));
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Session"));

            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
            this.Click(Id(TriptychDoc.ViewStateDoc, CalculatorDoc.StorageLink));
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.ViewState));
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));
        }
    }
}