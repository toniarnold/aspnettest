using iselenium;
using NUnit.Framework;
using System.Web.UI.WebControls;

namespace minimaltest
{
#pragma warning disable CS0618 // IIE obsolete

    /// <summary>
    /// Additionally to DefaultTest, the main Control must inherit from IRootControl
    /// to be able to navigate through the control hierarchy.
    /// </summary>
    [TestFixture]
    public class WithRootTest : IETest
#pragma warning restore CS0618 // IIE obsolete
    {
        [Test]
        public void NavigateWithRootTest()
        {
            this.Navigate("/withroot.aspx");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with root</h1>"));
        }

        [Test]
        public void WriteContentTest()
        {
            this.Navigate("/withroot.aspx");

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