using iselenium;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;

namespace minimaltest
{
    /// <summary>
    /// No dependencies on the side of the web application itself except
    /// Application_EndRequest in Global.asax.cs, therefore the client id of the
    /// controls must be known in advance, as member name navigation cannot be
    /// used. Minimality here: No inheritance, is only marked as
    /// ISeleniumtherefore an explicit [OneTimeSetUp]/[OneTimeTearDown] for
    /// SetUpIE()/TearDownIE() and the vars/js/driver properties are required.
    /// </summary>
    [TestFixture]
    public class DefaultTest : ISelenium
    {
        public IDictionary<string, object> vars { get; set; }
        public IJavaScriptExecutor js { get; set; }
        public IWebDriver driver { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUpBrowser()
        {
            this.SetUpBrowser<EdgeDriver>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBrowser()
        {
            this.TearDownBrowser();
        }

        [Test]
        public void NavigateDefaultTest()
        {
            this.Navigate("/default.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
        }

        [Test]
        public void ClickWithRootTest()
        {
            this.Navigate("/default.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
            this.ClickID("withroot-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with root</h1>"));
        }

        [Test]
        public void ClickWithstorageTest()
        {
            this.Navigate("/default.aspx");
            this.ClickID("withstorage-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }

        //[Test]
        //[Ignore("System.Runtime.Remoting.RemotingException in TestEngine.GetRunner")]
        public void RecursiveTestTest()
        {
            this.Navigate("/default.aspx");
            // If this would work here, it would be evocative of Goethe's "The Sorcerer's Apprentice"...
            this.ClickID("testButton");
        }
    }
}