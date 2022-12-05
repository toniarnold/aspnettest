using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;

namespace minimaltest
{
    /// <summary>
    /// Directly inherits from the ISelenium extension, therefore set the defaults explicitly
    /// </summary>
    [TestFixture]
    public class DefaultTest : ISelenium
    {
#pragma warning disable IDE1006 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }
        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006

        [OneTimeSetUp]
        public void OneTimeSetUpIE()
        {
            this.SetUpBrowser<EdgeDriver>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownIE()
        {
            this.TearDownBrowser();
        }

        [Test]
        public void NavigateDefaultTest()
        {
            for (int i = 1; i <= 3; i++)    // trying to catch 304 responses early on...
            {
                this.Navigate("/");
                Assert.That(this.Html(wait: SeleniumExtensionBase.RequestTimeout),
                            Does.Contain("<h1>minimalist test setup spa</h1>"));
            }
        }

        [Test]
        public void ClickWithStaticRemoteTest()
        {
            this.Navigate("/");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup spa</h1>"));
            this.ClickID("withstatic-link", expectRequest: false, wait: SeleniumExtensionBase.RequestTimeout);
            Assert.That(this.Html(wait: SeleniumExtensionBase.RequestTimeout),
                        Does.Contain("<h1>minimalist test setup with static remote</h1>"));
        }

        [Test]
        public void ClickWithStorageRemoteTest()
        {
            this.Navigate("/");
            this.ClickID("withstorage-link", expectRequest: false, wait: SeleniumExtensionBase.RequestTimeout);
            Assert.That(this.Html(wait: SeleniumExtensionBase.RequestTimeout),
                        Does.Contain("<h1>minimalist test setup with remote storage</h1>"));
        }
    }
}