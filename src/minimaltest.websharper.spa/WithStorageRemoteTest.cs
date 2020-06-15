using iselenium;
using minimal.websharper.spa;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;

// using OpenQA.Selenium.Firefox;

namespace minimaltest
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(InternetExplorerDriver))]
    public class WithStorageRemoteTest<TWebDriver> : SpaStorageTest<TWebDriver>
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
            get { return StorageRemoting.ViewModel.Content; }
        }

        /// <summary>
        /// SPA - no direct navigation possible
        /// </summary>
        private void NavigatekWithStorageRemote()
        {
            this.Navigate("/");
            this.Click("withstorage-link");
        }

        [Test]
        public void NavigatekWithStorageRemoteTest()
        {
            this.NavigatekWithStorageRemote();
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with remote storage</h1>"));
        }

        // Assert that all three storage methods behave as expected with respect to surviving certain actions:
        // ViewState: Even reload clears the storage
        // Session: Survives reload
        // Database: Survives restarting Internet Explorer

        [Test]
        public void StorageViewStateTest()
        {
            this.NavigatekWithStorageRemote();
            // No ViewModel present yet, thus can't assert anything except text
            // on the inital view, only the initial storage
            // TODO Assert.That(this.Controller.GetStorage(), Is.EqualTo(Storage.ViewState));    // default
            this.WriteContentTest(() => this.Nop());
        }

        [Test]
        public void StorageSessionTest()
        {
            this.NavigatekWithStorageRemote();
            this.Click("storageSession");
            this.WriteContentTest(() => this.Reload());
        }

        [Test]
        public void StorageDatabaseTest()
        {
            // Chrome is "user friendly", no persistent cookies in selenium remote  mode
            if (!(this.driver is ChromeDriver))
            {
                this.NavigatekWithStorageRemote();
                this.Click("storageDatabase");
                this.WriteContentTest(() => this.RestartBrowser());
            }
        }

        // "survives()"-Method implementations with explicit storage selection,
        // as the storage type itself is not persisted.

        /// <summary>
        /// ViewState does not survive navigation
        /// </summary>
        private void Nop() { }

        /// <summary>
        /// Reload the page, session storage should survive
        /// </summary>
        private void Reload()
        {
            this.NavigatekWithStorageRemote();
            this.Click("storageSession");
        }

        /// <summary>
        /// Restart the Browser and navigate to the storage page, database storage should survive
        /// </summary>
        private void RestartBrowser()
        {
            this.OneTimeTearDownBrowser();
            this.OneTimeSetUpBrowser();
            this.NavigatekWithStorageRemote();
            this.Click("storageDatabase");
        }

        /// <summary>
        /// Basically the same test as WithRootTest.WriteContentTest(),
        /// but parametrized with the storage
        /// </summary>
        public void WriteContentTest(Action survives)
        {
            this.Write("contentTextBox", "a first content line");
            this.Click("submitButton");
            this.AssertPoll(() => this.Content, () => Has.Exactly(1).Items);
            Assert.That(this.Content[0], Is.EqualTo("a first content line"));

            survives(); // Reload() or RestartIE()
            this.AssertPoll(() => this.Html(), () => Does.Contain("a first content line"));

            this.Write("contentTextBox", "a second content line");
            this.Click("submitButton");
            this.AssertPoll(() => this.Content, () => Has.Exactly(2).Items);
            Assert.That(this.Content[0], Is.EqualTo("a first content line"));
            Assert.That(this.Content[1], Is.EqualTo("a second content line"));
        }

        // ClearStorageTest()
        // To support ClearStorageTest() via GET-String as in the other implementations
        // would make that minimal.websharper.spa not so "minimal" no more, thus omit.
        // Thanks to DeleteAllCookies(), it is no more needed as [OneTimeSetUp] anyway.
    }
}