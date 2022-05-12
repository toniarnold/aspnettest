using iselenium;
using minimal.blazor.Models;
using minimal.blazor.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace minimaltest.blazor
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    public class WithStaticTest<TWebDriver> : StaticComponentTest<TWebDriver, WithStatic, Main>
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void NavigatekWithStaticTest()
        {
            this.Navigate("/Withstatic");
            Assert.That(this.Html(), Does.Contain(">minimalist test setup with static main</h1>"));
        }

        [Test]
        public void WriteContentTest()
        {
            this.Navigate("/Withstatic");
            this.Write(Component.contentTextBox, "a first content line");
            this.Click(Component.submitButton);
            Assert.That(Main, Has.Exactly(1).Items);
            var firstString = Main[0];
            Assert.That(firstString, Is.EqualTo("a first content line"));

            this.Write(Component.contentTextBox, "a second content line");
            this.Click(Component.submitButton);
            Assert.That(Main, Has.Exactly(2).Items);
            var firstString2 = Main[0];
            Assert.That(firstString2, Is.EqualTo("a first content line"));
            var secondString = Main[1];
            Assert.That(secondString, Is.EqualTo("a second content line"));
        }
    }
}