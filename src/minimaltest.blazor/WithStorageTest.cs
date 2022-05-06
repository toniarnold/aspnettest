using asplib.Services;
using iselenium;
using minimal.blazor.Models;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;

namespace minimaltest.blazor
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    public class WithStorageTest<TWebDriver> : SpaDbTest<TWebDriver>
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
        public void NavigatekWithStorageTest()
        {
            this.Navigate("/Withstorage");
            this.AssertPoll(() => this.Html(), () => Does.Contain(">minimalist test setup with storage</h1>"));
        }

        // Assert that all three storage methods behave as expected with respect to surviving certain actions:
        // Blazor only: Even reload clears the storage
        // Database: Survives restarting the browser
        // SessionStorage: Survives reload, but not restart
        // LocalStorage: Survives restarting

        [Test]
        public void StorageBlazorTest()
        {
            this.Navigate("/Withstorage");
            this.WriteContentTest(() => this.Nop());
        }

        [Test]
        public void StorageSessionStorageTest()
        {
            this.Navigate("/Withstorage");
            this.Click("storageSessionStorage");
            this.Click("clearButton");
            this.WriteContentTest(() => this.Reload());
        }

        // As (at least) of FireFox 101, there seems to be also no persistence
        // no more when run by Selenium, thus these persistence tests can no
        // more be executed at all.

        //[Test]
        //public void StorageDatabaseTest()
        //{
        //    // Chrome is "user friendly", no persistent cookies in selenium remote mode
        //    if (this.driver is FirefoxDriver)
        //    {
        //        this.Navigate("/Withstorage");
        //        this.Click("storageDatabase");
        //        this.Click("clearButton");
        //        this.WriteContentTest(() => this.RestartBrowser());
        //    }
        //    else Assert.Inconclusive("No persistence in Chrome");
        //}

        //[Test]
        //public void StorageLocalStorageTest()
        //{
        //    if (this.driver is FirefoxDriver)
        //    {
        //        this.Navigate("/Withstorage");
        //        this.Click("storageLocalStorage");
        //        this.Click("clearButton");
        //        this.WriteContentTest(() => this.RestartBrowser());
        //    }
        //    else Assert.Inconclusive("No persistence in Chrome");
        //}

        /// <summary>
        /// Basically the same test as WithStaticTest.WriteContentTest(),
        /// but parametrized with the action the page has to "survive",
        /// e.g. Reload() or RestartBrowser()
        /// </summary>
        public void WriteContentTest(Action survives)
        {
            this.Write("contentTextBox", "a first content line");
            this.Click("submitButton");
            this.AssertPoll(() => this.Content, () => Has.Exactly(1).Items);
            Assert.That(this.Content[0], Is.EqualTo("a first content line"));

            survives();
            this.AssertPoll(() => this.Html(), () => Does.Contain("a first content line"));

            this.Write("contentTextBox", "a second content line");
            this.Click("submitButton");
            this.AssertPoll(() => this.Content, () => Has.Exactly(2).Items);
            Assert.That(this.Content[0], Is.EqualTo("a first content line"));
            Assert.That(this.Content[1], Is.EqualTo("a second content line"));

            survives(); // Reload() or RestartBrowser()
            Assert.That(this.Content[0], Is.EqualTo("a first content line"));
            Assert.That(this.Content[1], Is.EqualTo("a second content line"));
        }

        /// <summary>
        /// survives() action:
        /// survives nothing
        /// </summary>
        private void Nop()
        { }

        /// <summary>
        /// survives() action: Selenium driver reload on the same URL
        /// Reload the page, session storage should survive
        /// </summary>
        private void Reload()
        {
            this.Refresh();
        }

        /// <summary>
        /// Restart the Browser and navigate to the storage page, database storage should survive
        /// </summary>
        private void RestartBrowser()
        {
            this.OneTimeTearDownBrowser();
            this.OneTimeSetUpBrowser();
            this.Navigate("/Withstorage");
        }

        private void ClearLocalStorage()
        {
            this.Click("clearButton");
        }
    }
}