using asplib.Services;
using iselenium;
using minimal.blazor.Models;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

namespace minimaltest.blazor
{
    [TestFixture(typeof(ChromeDriver))]
    public class WithStaticTest<TWebDriver> : SpaTest<TWebDriver>
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// The submit button stays on the same page
        /// </summary>
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
            get { return MainAccessor<Main>.Instance; }
        }

        [Test]
        public void NavigatekWithStaticTest()
        {
            this.Navigate("/Withstatic");
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup with static main</h1>"));
        }

        [Test]
        public void WriteContentTest()
        {
            this.Navigate("/Withstatic");
            this.Write("contentTextBox", "a first content line");
            this.Click("submitButton");
            this.AssertPoll(() => this.Content, () => Has.Exactly(1).Items);
            var firstString = this.Content[0];
            Assert.That(firstString, Is.EqualTo("a first content line"));

            this.Write("contentTextBox", "a second content line");
            this.Click("submitButton");
            this.AssertPoll(() => this.Content, () => Has.Exactly(2).Items);
            var firstString2 = this.Content[0];
            Assert.That(firstString2, Is.EqualTo("a first content line"));
            var secondString = this.Content[1];
            Assert.That(secondString, Is.EqualTo("a second content line"));
        }
    }
}