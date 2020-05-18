using iselenium;
using minimal;
using NUnit.Framework;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace minimaltest
{
    /// <summary>
    /// Additionally to DefaultTest, the main Control must inherit from IStorageControl
    /// such that it maintains its own storage mechanism.
    /// </summary>
    [TestFixture]
    public class WithStorageTest : StorageTest<ContentStorage>
    {
        /// <summary>
        /// There could be a manually generated row in IE's current cookie, thus explicitly delete.
        /// </summary>
        [OneTimeSetUp]
        public void ClearDatabaseStorage()
        {
            this.Navigate(String.Format("/withstorage.aspx?clear=true&storage=database&endresponse=true"));
        }

        [Test]
        public void NavigateWithStorageTest()
        {
            this.Navigate("/withstorage.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }

        // Assert that all three storage methods behave as expected with respect to surviving certain actions:
        // ViewState: Even reload clears the storage
        // Session: Survives reload
        // Database: Survives restarting Internet Explorer

        [Test]
        public void StorageViewStateTest()
        {
            this.Navigate("/withstorage.aspx");
            var storageList = (RadioButtonList)this.GetControl("storageList");
            Assert.That(storageList.SelectedValue, Is.EqualTo("ViewState"));    // default
            this.WriteContentTest(() => this.Nop());
        }

        [Test]
        public void StorageSessionTest()
        {
            this.Navigate("/withstorage.aspx");
            this.Select("storageList", "Session", expectPostBack: true);
            this.WriteContentTest(() => this.Reload());
        }

        [Test]
        public void StorageDatabaseTest()
        {
            this.Navigate("/withstorage.aspx");
            this.Select("storageList", "Database", expectPostBack: true);
            this.WriteContentTest(() => this.RestartIE());
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
            this.Navigate("/withstorage.aspx");
            this.Select("storageList", "Session", expectPostBack: true);
        }

        /// <summary>
        /// Restart Internet Explorer and navigate to the page, database storage should survive
        /// </summary>
        private void RestartIE()
        {
            this.TearDownIE();
            this.SetUpIE();
            this.Navigate("/withstorage.aspx");
            this.Select("storageList", "Database", expectPostBack: true);
        }

        /// <summary>
        /// Basically the same test as WithRootTest.WriteContentTest(),
        /// but parametrized with the storage
        /// </summary>
        public void WriteContentTest(Action survives)
        {
            this.Write("contentTextBox", "a first content line");
            this.Click("submitButton");
            Assert.That(((TextBox)this.GetControl("contentTextBox")).Text, Is.Empty);
            // Assertions on the View level
            Assert.That(((BulletedList)this.GetControl("contentList")).Items, Has.Exactly(1).Items);
            Assert.That(((BulletedList)this.GetControl("contentList")).Items[0].Text, Is.EqualTo("a first content line"));
            // Assertions on the Model level
            Assert.That(this.Main.Content, Has.Exactly(1).Items);
            Assert.That(this.Main.Content[0], Is.EqualTo("a first content line"));

            survives(); // Reload() or RestartIE()

            this.Write("contentTextBox", "a second content line");
            this.Click("submitButton");
            Assert.That(((TextBox)this.GetControl("contentTextBox")).Text, Is.Empty);
            // Assertions on the View level
            Assert.That(((BulletedList)this.GetControl("contentList")).Items, Has.Exactly(2).Items);
            Assert.That(((BulletedList)this.GetControl("contentList")).Items[0].Text, Is.EqualTo("a first content line"));
            Assert.That(((BulletedList)this.GetControl("contentList")).Items[1].Text, Is.EqualTo("a second content line"));
            // Assertions on the Model level
            Assert.That(this.Main.Content, Has.Exactly(2).Items);
            Assert.That(this.Main.Content[0], Is.EqualTo("a first content line"));
            Assert.That(this.Main.Content[1], Is.EqualTo("a second content line"));
        }

        /// <summary>
        /// Verify that explicitly clearing the current storage item by a specific GET request
        /// actually does remove the stored entry.
        /// </summary>
        [Test]
        public void ClearStorageTest()
        {
            // Local setup: Store a value into the database
            this.Navigate("/withstorage.aspx");
            this.Select("storageList", "Database", expectPostBack: true);
            this.Write("contentTextBox", "a stored content line");
            this.Click("submitButton");
            Assert.That(this.Main.Content, Has.Exactly(1).Items);
            Assert.That(this.Main.Content[0], Is.EqualTo("a stored content line"));

            // Confirm that the line persistent
            this.RestartIE();
            Assert.That(this.Main.Content, Has.Exactly(1).Items);
            Assert.That(this.Main.Content[0], Is.EqualTo("a stored content line"));

            // Method under test: explicitly clear the database storage
            this.ClearDatabaseStorage();

            // Confirm that the content is now empty
            this.RestartIE();
            Assert.That(this.Main.Content, Has.Exactly(0).Items);
        }
    }
}