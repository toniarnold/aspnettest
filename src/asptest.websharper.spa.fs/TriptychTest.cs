using asp.websharper.spa.fs;
using asplib.Model;
using asptest.Calculator;
using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using static asplib.View.TagHelper;

namespace asptest
{
    [TestFixture(typeof(InternetExplorerDriver))]
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    public class TriptychTest<TWebDriver> : CalculatorTestBase<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        public override void OneTimeSetUpBrowser()
        {
            base.OneTimeSetUpBrowser();
            driver.Manage().Window.Size = new System.Drawing.Size(1450, 1000);
        }

        [OneTimeSetUp]
        public void DistinctPages()
        {
            this.samePageDefault = false;
        }

        [SetUp]
        public void ReloadUnsetStorage()
        {
            this.Navigate("/");
            StorageImplementation.SessionStorage = null;    // defaults to ViewState (DOM)
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
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));   // init view
            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
        }

        [Test]
        public void CircumambulateStorageTypes()
        {
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));   // init view
            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
            this.Click(Id(TriptychDoc.DatabaseDoc, CalculatorDoc.StorageLink));
            this.driver.Navigate().Refresh(); // only in the F# port
            // Poll SessionStorage first, this.Html() is ambiguous and succeeds too early (still on the triptych)
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.Database));
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Database"));

            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
            this.Click(Id(TriptychDoc.SessionDoc, CalculatorDoc.StorageLink));
            this.driver.Navigate().Refresh();
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.Session));
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Session"));

            this.Click(Id(CalculatorDoc.StorageLink));
            this.AssertTriptychHtml();
            this.Click(Id(TriptychDoc.ViewStateDoc, CalculatorDoc.StorageLink));
            this.driver.Navigate().Refresh();
            this.AssertPoll(() => this.ViewModel.SessionStorage, () => Is.EqualTo(Storage.ViewState));
            this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));
        }

        /// <summary>
        /// Assert the presence of the three calculators superficially by text.
        /// </summary>
        private void AssertTriptychHtml()
        {
            Assert.Multiple(() =>
            {
                // Assert from bottom to top to ensure the page has been fully rendered on the client
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Database"));
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: Session"));
                this.AssertPoll(() => this.Html(), () => Does.Contain("Session Storage: ViewState"));
            });
        }
    }
}