using asplib.Components;
using iselenium;
using minimal.blazor.Models;
using minimal.blazor.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Threading;

namespace minimaltest.blazor
{
    //[TestFixture(typeof(ChromeDriver))]
    //[TestFixture(typeof(EdgeDriver))]
    //[TestFixture(typeof(FirefoxDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    public class WithStorageTest<TWebDriver> : StaticComponentDbTest<TWebDriver, WithStorage, Main>
        where TWebDriver : IWebDriver, new()
    {
        /// <summary>
        /// Typed accessor to the only model object in the app
        /// </summary>
        public List<string>? Content
        {
            get { return ((WithStorage?)TestFocus.Component)?.Main; }
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
            // The InputRadios and the clearButton reload the page to reinitialize
            this.Click(By.Id, "storageSessionStorage", expectRequest: true);    // temporary by id
            this.Click(Component.clearButton, expectRequest: true);
            this.WriteContentTest(() => this.Reload());
        }

        // As (at least) of FireFox 101, there seems to be also no persistence
        // no more when run by Selenium, thus these persistence tests can no
        // more be executed over browser restarts with:
        // this.WriteContentTest(() => this.RestartBrowser());

        [Test]
        public void StorageDatabaseTest()
        {
            {
                this.Navigate("/Withstorage");
                this.Click(By.Id, "storageDatabase", expectRequest: true);
                this.Click(Component.clearButton, expectRequest: true);
                this.WriteContentTest(() => this.Reload());
            }
        }

        [Test]
        public void StorageLocalStorageTest()
        {
            this.Navigate("/Withstorage");
            this.Click("storageLocalStorage", expectRequest: true);
            this.Click(Component.clearButton, expectRequest: true);
            this.WriteContentTest(() => this.Reload());
        }

        /// <summary>
        /// Basically the same test as WithStaticTest.WriteContentTest(),
        /// but parametrized with the action the page has to "survive",
        /// e.g. Reload() or RestartBrowser()
        /// </summary>
        public void WriteContentTest(Action survives)
        {
            this.Write(Component.contentTextBox, "a first content line");
            this.Click(Component.submitButton);
            Assert.That(Content, Has.Exactly(1).Items);
            Assert.That(Content[0], Is.EqualTo("a first content line"));

            survives();
            this.AssertPoll(() => this.Html(), () => Does.Contain("a first content line"));

            this.Write(Component.contentTextBox, "a second content line");
            this.Click(Component.submitButton);
            Assert.That(Content, Has.Exactly(2).Items);
            Assert.That(Content[0], Is.EqualTo("a first content line"));
            Assert.That(Content[1], Is.EqualTo("a second content line"));

            survives(); // Reload() or RestartBrowser()
            Assert.That(Content[0], Is.EqualTo("a first content line"));
            Assert.That(Content[1], Is.EqualTo("a second content line"));
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
            this.Click(Component.clearButton);
        }
    }
}