using asplib.Model;
using asptest.Calculator;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.IE;

namespace asptest
{
    [Ignore("Works in debug mode, but usually not from the runner")]
    [TestFixture]
    //public class TriptychTest<TWebDriver> : CalculatorTestBase<TWebDriver>
    //    where TWebDriver : IWebDriver, new()
    public class TriptychTest : CalculatorTestBase<InternetExplorerDriver>
    {
        [SetUp]
        public void UnsetStorage()
        {
            StorageImplementation.SessionStorage = null;
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
            this.Click("storageLink");
            this.AssertTriptychHtml();
        }

        /// <summary>
        /// Assert the presence of the three calculators superficially by text.
        /// </summary>
        private void AssertTriptychHtml()
        {
            Assert.Multiple(() =>
            {
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Session"));
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Database"));
            });
        }

        [Ignore("Inconsistently implemented C#/F#, SessionStorage assertion doesn't work")]
        [Test]
        public void CircumambulateStorageTypes()
        {
            this.Click("storageLink");
            this.AssertTriptychHtml();
            this.Click("storageLinkViewState");
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Splash));
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.ViewState));

            this.Click("storageLink");
            this.AssertTriptychHtml();
            this.Click("storageLinkSession");
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Session"));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Splash));
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.Session));

            this.Click("storageLink");
            this.AssertTriptychHtml();
            this.Click("storageLinkDatabase");
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Database"));
            this.AssertPoll(() => this.State, () => Is.EqualTo(CalculatorContext.Map1.Splash));
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.Database));
        }
    }
}