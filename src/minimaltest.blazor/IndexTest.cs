using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

namespace minimaltest.blazor
{
    // Uses the generic ISelenium extension, therefore SPA-specific wait:
    // SeleniumExtensionBase.RequestTimeout has to be set explicitly.
    public class IndexTest : ISelenium
    {
#pragma warning disable IDE1006, CS8618 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }

        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006, CS8618

        [OneTimeSetUp]
        public void OneTimeSetUpBrowser()
        {
            this.SetUpBrowser<ChromeDriver>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBrowser()
        {
            this.TearDownBrowser();
        }

        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/");
            var html = this.Html();
            // Blazor adds tabindex: <h1 tabindex="-1">minimalist test setup</h1>
            Assert.That(html, Does.Contain(">minimalist test setup</h1>"));
        }

        [Test]
        public void ClickWithStaticTest()
        {
            this.Navigate("/");
            Assert.That(this.Html(), Does.Contain(">minimalist test setup</h1>"));
            this.ClickID("withstatic-link", expectRequest: false, wait: SeleniumExtensionBase.RequestTimeout);
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup with static main</h1>"));
        }

        [Test]
        public void ClickWithStorageTest()
        {
            this.Navigate("/");
            Assert.That(this.Html(), Does.Contain(">minimalist test setup</h1>"));
            this.ClickID("withstorage-link", expectRequest: false, wait: SeleniumExtensionBase.RequestTimeout);
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup with storage</h1>"));
        }
    }
}