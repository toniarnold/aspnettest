using asplib.Components;
using iselenium;
using minimal.blazor.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;

namespace minimaltest.blazor
{
    // Uses the blank ISelenium extension, therefore SPA-specific wait:
    // SeleniumExtensionBase.RequestTimeout has to be set explicitly.
    public class IndexTest : ISelenium
    {
#pragma warning disable IDE1006, CS8618 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }

        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006, CS8618

        public Index Index => (Index)TestFocus.Component;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.SetUpBrowser<EdgeDriver>();
            TestFocus.SetFocus(typeof(Index));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestFocus.RemoveFocus();
            this.TearDownBrowser();
        }

        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/");
            var html = this.Html();
            // Blazor adds tabindex: <h1 tabindex="-1">minimalist test setup</h1>
            this.AssertPoll(() => html, () => Does.Contain(">minimalist test setup</h1>"));
        }

        [Test]
        public void ClickWithStaticTest()
        {
            this.Navigate("/");
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup</h1>"));
            this.Click(By.CssSelector, $"*[_bl_{Index.withstaticLink.Id}]", expectRequest: true, wait: SeleniumExtensionBase.RequestTimeout);
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup with static main</h1>"));
        }

        [Test]
        public void ClickWithStorageTest()
        {
            this.Navigate("/");
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup</h1>"));
            this.Click(By.CssSelector, $"*[_bl_{Index.withstorageLink.Id}]", expectRequest: true, wait: SeleniumExtensionBase.RequestTimeout);
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup with storage</h1>"));
        }
    }
}