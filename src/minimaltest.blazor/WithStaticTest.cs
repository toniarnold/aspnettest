using asplib.Components;
using asplib.Services;
using iselenium;
using minimal.blazor.Models;
using minimal.blazor.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Threading;

namespace minimaltest.blazor
{
    [TestFixture(typeof(ChromeDriver))]
    public class WithStaticTest<TWebDriver> : StaticComponentTest<TWebDriver, WithStatic, Main>
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
            Thread.Sleep(2);
            this.Write(C.contentTextBox, "a first content line");
            this.Click(C.submitButton);
            this.AssertPoll(() => Main, () => Has.Exactly(1).Items);
            var firstString = Main[0];
            Assert.That(firstString, Is.EqualTo("a first content line"));

            this.Write(C.contentTextBox, "a second content line");
            this.Click(C.submitButton);
            this.AssertPoll(() => Main, () => Has.Exactly(2).Items);
            var firstString2 = Main[0];
            Assert.That(firstString2, Is.EqualTo("a first content line"));
            var secondString = Main[1];
            Assert.That(secondString, Is.EqualTo("a second content line"));
        }
    }
}