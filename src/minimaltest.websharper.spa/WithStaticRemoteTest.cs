using iselenium;
using minimal.websharper.spa;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.Collections.Generic;

namespace minimaltest
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]    // at the speed of continental drift...
    [TestFixture(typeof(InternetExplorerDriver))]
    public class WithStaticRemoteTest<TWebDriver> : SpaTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        [OneTimeSetUp]
        public void NoAwaitRemoved()
        {
            this.awaitRemovedDefault = false;
        }

        /// <summary>
        /// Typed accessor to the only model object in the app
        /// </summary>
        public List<string> Content
        {
            get { return StaticRemoting.Content; }
        }

        // The main objective of an SPA is to withdraw control from the user,
        // i.e. to make deep links impossible...
        [Test]
        public void NavigateWithStaticRemotingTest()
        {
            this.Navigate("/");
            this.Click("withstatic-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with static remote</h1>"));
        }

        [Test]
        public void WriteContentTest()
        {
            this.Navigate("/");
            this.Click("withstatic-link");
            this.Write("contentTextBox", "a first content line");
            this.Click("submitButton");
            // Lambda with caught NullReferenceException, more idiomatic assert below
            this.AssertPoll(() => { return this.Content.Count; }, () => Is.EqualTo(1));
            var firstString = this.Content[0];
            Assert.That(firstString, Is.EqualTo("a first content line"));

            this.Write("contentTextBox", "a second content line");
            this.Click("submitButton");
            // Avoids code block lambda by a better constraint
            this.AssertPoll(() => this.Content, () => Has.Exactly(2).Items);
            var firstString2 = this.Content[0];
            Assert.That(firstString2, Is.EqualTo("a first content line"));
            var secondString = this.Content[1];
            Assert.That(secondString, Is.EqualTo("a second content line"));
        }
    }
}