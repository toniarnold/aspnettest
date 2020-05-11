using iselenium;
using NUnit.Framework;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace minimaltest
{
    /// <summary>
    /// No dependencies on the side of the web application itself except
    /// Application_EndRequest in Global.asax.cs, therefore the client id of
    /// the controls must be known in advance, as member name navigation cannot be used.
    /// Minimality here: Directly inherits from IIE, therefore an explicit
    /// [OneTimeSetUp]/[OneTimeTearDown] for SetUpIE()/TearDownIE() is required.
    /// </summary>
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
            this.Navigate("/minimal.webforms/default.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
        }

        [Test]
        public void ClickWithRootTest()
        {
            this.Navigate("/minimal.webforms/default.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup</h1>"));
            this.ClickID("withroot-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with root</h1>"));
        }

        [Test]
        public void ClickWithstorageTest()
        {
            this.Navigate("/minimal.webforms/default.aspx");
            this.ClickID("withstorage-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }

        //[Test]
        //[Ignore("System.Runtime.Remoting.RemotingException in TestEngine.GetRunner")]
        public void RecursiveTestTest()
        {
            this.Navigate("/minimal.webforms/default.aspx");
            // If this would work here, it would be evocative of Goethe's "The Sorcerer's Apprentice"...
            this.ClickID("testButton");
        }
    }
}