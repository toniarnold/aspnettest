using asplib.Controllers;
using asplib.Model;
using iselenium;
using minimal.Controllers;
using minimal.Models;
using NUnit.Framework;
using System;

namespace minimaltest
{
#pragma warning disable CS0618 // IIE obsolete

    [TestFixture]
    public class WithStorageControllerTest : StorageTest<WithStorageController>
#pragma warning restore CS0618 // IIE obsolete
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
        // Database: Survives restarting Internet Explorer

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

        [Test]
        public void StorageDatabaseTest()
        {
            this.Navigate("/WithStorage");
            this.Select("Storage", "Database", expectPostBack: true);
            this.WriteContentTest(() => this.RestartIE());
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
        /// Restart Internet Explorer and navigate to the page, database storage should survive
        /// </summary>
        private void RestartIE()
        {
#pragma warning disable CS0618 // IIE obsolete
            this.TearDownIE();
            this.SetUpIE();
#pragma warning restore CS0618 // IIE obsolete

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

            survives(); // Reload() or RestartIE()

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

            // Confirm that the line persistent
            this.RestartIE();
            Assert.That(this.Model.Content, Has.Exactly(1).Items);
            Assert.That(this.Model.Content[0], Is.EqualTo("a stored content line"));

            // Method under test: explicitly clear the database storage
            this.ClearDatabaseStorage();

            // Confirm that the content is now empty
            this.RestartIE();
            Assert.That(this.Model.Content, Has.Exactly(0).Items);
        }
    }
}