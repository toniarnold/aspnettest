using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

using NUnit.Framework;
using iie;

namespace minimaltest
{
    /// <summary>
    /// Additionally to DefaultTest, the main Control must inherit from IRootControl
    /// to be able to navigate through the control hierarchy.
    /// </summary>
    [TestFixture]
    public class WithRootTest : IIE
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.SetUpIE();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.TearDownIE();
        }

        [Test]
        public void NavigateWithRootTest()
        {
            this.Navigate("/minimal/withroot.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with root</h1>"));
        }

        [Test]
        public void WriteContentTest()
        {
            this.Navigate("/minimal/withroot.aspx");

            this.Write("contentTextBox", "a first content line");
            this.Click("submitButton");
            Assert.That(((TextBox)this.GetControl("contentTextBox")).Text, Is.Empty);
            Assert.That(((BulletedList)this.GetControl("contentList")).Items.Count, Is.EqualTo(1));
            var firstItem = ((BulletedList)this.GetControl("contentList")).Items[0];
            Assert.That(firstItem.Text, Is.EqualTo("a first content line"));

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
