using asplib.Controllers;
using asplib.Model;
using iselenium;
using minimal.Controllers;
using minimal.Models;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System;

namespace minimaltest
{
    [TestFixture]
    public class WithStorageControllerTest : SeleniumDbTest<EdgeDriver, WithStorageController>
    {
        /// <summary>
        /// Typed accessor for the only ViewModel used in the app
        /// </summary>
        public WithStorageViewModel Model
        {
            get { return this.Controller.Model; }
        }

        /// <summary>
        /// IIE: There could be a manually generated row in IE's current cookie, thus explicitly delete.
        /// SeleniumTest: Due to DeleteAllCookies() as [SetUp], this is not necessary no more as [OneTimeSetUp],
        /// but method under test in ClearStorageTest()
        /// </summary>
        [OneTimeSetUp]
        public void ClearDatabaseStorage()
        {
            this.Navigate(String.Format("/WithStorage?clear=true&storage=database"));
        }

        [Test]
        public void NavigateWithStorageControllerTest()
        {
            this.Navigate("/WithStorage");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }

        // Assert that all three storage methods behave as expected with respect to surviving certain actions:
        // ViewState: Even reload clears the storage
        // Session: Survives reload
        // Database: Survives restarting the browser

        [Test]
        public void StorageViewStateTest()
        {
            this.Navigate("/WithStorage");
            // No ViewModel present yet, thus can't assert anything except text
            // on the inital view, only the initial storage
            Assert.That(this.Controller.GetStorage(), Is.EqualTo(Storage.ViewState));    // default
            this.WriteContentTest(() => this.Nop());
        }

        [Test]
        public void StorageSessionTest()
        {
            this.Navigate("/WithStorage");
            this.Select("Storage", "Session", expectPostBack: true);
            this.WriteContentTest(() => this.Reload());
        }

        //[Test]
        //[Ignore("Internet Explorer has been forcibly disabled")]
        public void StorageDatabaseTest()
        {
            this.Navigate("/WithStorage");
            this.Select("Storage", "Database", expectPostBack: true);
            this.WriteContentTest(() => this.RestartBrowser());
        }

        // "survives()"-Method implementations with explicit storage selection,
        // as the storage type itself is not persisted.

        /// <summary>
        /// ViewState does not survive navigation
        /// </summary>
        private void Nop()
        { }

        /// <summary>
        /// Reload the page, session storage should survive
        /// </summary>
        private void Reload()
        {
            this.Navigate("/WithStorage");
            this.Select("Storage", "Session", expectPostBack: true);
        }

        /// <summary>
        /// Restart the browser and navigate to the page, database storage should survive.
        /// This worked only with Internet Explorer which didn't run in private mode with selenium.
        /// </summary>
        private void RestartBrowser()
        {
            this.TearDownBrowser();
            this.SetUpBrowser<EdgeDriver>();
            this.Navigate("/WithStorage");
            this.Select("Storage", "Database", expectPostBack: true);
        }

        /// <summary>
        /// Basically the same test as WithRootTest.WriteContentTest(),
        /// but parametrized with the storage
        /// </summary>
        public void WriteContentTest(Action survives)
        {
            this.Write("ContentTextBox", "a first content line");
            this.Click("SubmitButton");
            // Unlike as in WebForms, the Is.Empty assertion lies about the generated HTML:
            Assert.That(this.Model.ContentTextBox, Is.Empty);
            // Assertions on the ViewModel level (= View level in WebForms)
            Assert.That(this.Model.Content, Has.Exactly(1).Items);
            Assert.That(this.Model.Content[0], Is.EqualTo("a first content line"));
            // Assertions on the Controller level (= Model level in WebForms)
            Assert.That(this.Controller.ContentList, Has.Exactly(1).Items);
            Assert.That(this.Controller.ContentList[0], Is.EqualTo("a first content line"));

            survives(); // Reload() or formerly RestartIE()

            this.Write("ContentTextBox", "a second content line");
            this.Click("SubmitButton");
            Assert.That(this.Model.ContentTextBox, Is.Empty);
            // Assertions on the ViewModel level (= View level in WebForms)
            Assert.That(this.Model.Content, Has.Exactly(2).Items);
            Assert.That(this.Model.Content[0], Is.EqualTo("a first content line"));
            Assert.That(this.Model.Content[1], Is.EqualTo("a second content line"));
            // Assertions on the Controller level (= Model level in WebForms)
            Assert.That(this.Controller.ContentList, Has.Exactly(2).Items);
            Assert.That(this.Controller.ContentList[0], Is.EqualTo("a first content line"));
            Assert.That(this.Controller.ContentList[1], Is.EqualTo("a second content line"));
        }

        /// <summary>
        /// Verify that explicitly clearing the current storage item by a specific GET request
        /// actually does remove the stored entry.
        /// </summary>
        [Test]
        public void ClearStorageTest()
        {
            // Local setup: Store a value into the database
            this.Navigate("/WithStorage");
            this.Select("Storage", "Database", expectPostBack: true);
            this.Write("ContentTextBox", "a stored content line");
            this.Click("SubmitButton");
            Assert.That(this.Model.Content, Has.Exactly(1).Items);
            Assert.That(this.Model.Content[0], Is.EqualTo("a stored content line"));

            // Method under test: explicitly clear the database storage
            this.ClearDatabaseStorage();

            // Confirm that the content is now empty
            //this.RestartBrowser();
            Assert.That(this.Model.Content, Has.Exactly(0).Items);
        }
    }
}