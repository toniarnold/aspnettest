using iie;
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
    public class WithStorageTest : IIE
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.SetUpIE();
            this.ClearStorage("Session");
            this.ClearStorage("Database");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.ClearStorage("Session");
            this.ClearStorage("Database");
            this.TearDownIE();
        }

        private void ClearStorage(string storage)
        {
            this.Navigate(String.Format("/minimal/withstorage.aspx?clear=true&endresponse=true&storage={0}", storage));
        }

        [Test]
        public void NavigateWithStorageTest()
        {
            this.Navigate("/minimal/withstorage.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with storage</h1>"));
        }

        [Test]
        public void StorageViewstateTest()
        {
            this.Navigate("/minimal/withstorage.aspx");
            var storageList = (RadioButtonList)this.GetControl("storageList");
            Assert.That(storageList.SelectedValue, Is.EqualTo("Viewstate"));    // default
            this.WriteContentTest(() => this.Nop());
        }

        [Test]
        public void StorageSessionTest()
        {
            this.Navigate("/minimal/withstorage.aspx");
            this.Select("storageList", "Session", expectPostBack: true);
            this.WriteContentTest(() => this.Reload());
        }

        [Test]
        public void StorageDatabaseTest()
        {
            this.Navigate("/minimal/withstorage.aspx");
            this.Select("storageList", "Database", expectPostBack: true);
            this.WriteContentTest(() => this.RestartIE());
        }

        // "survives()"-Method implementations with explicit storage selection,
        // as the storage type itself is not persisted.

        /// <summary>
        /// Viewstate does not survive navigation
        /// </summary>
        private void Nop() { }

        /// <summary>
        /// Reload the page, session storage should survive
        /// </summary>
        private void Reload()
        {
            this.Navigate("/minimal/withstorage.aspx");
            this.Select("storageList", "Session", expectPostBack: true);
        }

        /// <summary>
        /// Restart Internet explorer and navigate to the page, database storage should survive
        /// </summary>
        private void RestartIE()
        {
            this.TearDownIE();
            this.SetUpIE();
            this.Navigate("/minimal/withstorage.aspx");
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
            Assert.That(((BulletedList)this.GetControl("contentList")).Items.Count, Is.EqualTo(1));
            var firstItem = ((BulletedList)this.GetControl("contentList")).Items[0];
            Assert.That(firstItem.Text, Is.EqualTo("a first content line"));

            survives(); // Reload() or RestartIE()

            this.Write("contentTextBox", "a second content line");
            this.Click("submitButton");
            Assert.That(((TextBox)this.GetControl("contentTextBox")).Text, Is.Empty);
            Assert.That(((BulletedList)this.GetControl("contentList")).Items.Count, Is.EqualTo(2));
            var firstItem2 = ((BulletedList)this.GetControl("contentList")).Items[0];
            Assert.That(firstItem2.Text, Is.EqualTo("a first content line"));
            var secondItem = ((BulletedList)this.GetControl("contentList")).Items[1];
            Assert.That(secondItem.Text, Is.EqualTo("a second content line"));
        }
    }
}