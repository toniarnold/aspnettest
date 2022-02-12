using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

namespace minimaltest.blazor
{
    public class IndexTest : ISelenium
    {
#pragma warning disable IDE1006 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }
        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006

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
    }
}