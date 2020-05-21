using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace minimaltest.websharper.spa
{
    [TestFixture]
    public class DefaultTest : IIE
    {
#pragma warning disable IDE1006 // Members in Selenium-generated C# code
        public IDictionary<string, object> vars { get; set; }
        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }
#pragma warning restore IDE1006

        [OneTimeSetUp]
        public void OneTimeSetUpIE()
        {
            this.SetUpIE();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownIE()
        {
            this.TearDownIE();
        }

        [Test]
        public void NavigateDefaultTest()
        {
            for (int i = 1; i <= 3; i++)    // trying to catch 304 responses early on...
            {
                this.Navigate("/");
                Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup spa</h1>"));
            }
        }

        [Test]
        public void ClickWithStaticRemoteTest()
        {
            this.Navigate("/");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup spa</h1>"));
            this.ClickID("withstatic-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with static remote</h1>"));
        }

        [Test]
        public void ClickWithStorageRemoteTest()
        {
            this.Navigate("/");
            this.ClickID("withstorage-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with remote storage</h1>"));
        }
    }
}