using iselenium;
using minimal.Controllers;
using minimal.Models;
using NUnit.Framework;

namespace minimaltest
{
    /// <summary>
    /// Additionally to DefaultTest, the main Control must inherit from IRootControl
    /// to be able to navigate through the control hierarchy.
    /// </summary>
    [TestFixture]
    public class WithStaticControllerTest : IETest<WithStaticController>
    {
        /// <summary>
        /// Typed accessor to the only ViewModel for the app
        /// </summary>
        public WithStaticViewModel Model
        {
            get { return this.Controller.Model; }
        }

        [Test]
        public void NavigateWithStaticControllerTest()
        {
            this.Navigate("/WithStatic");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with static controller</h1>"));
        }

        [Test]
        public void WriteContentTest()
        {
            this.Navigate("/WithStatic");
            this.WriteID("ContentTextBox", "a first content line");
            this.Click("SubmitButton");
            // The following assertion lies: the empty model attribute is not reflected
            // in the GUI, as the updated model surprisingly isn't applied to the Razor view:
            Assert.That(this.Model.ContentTextBox, Is.Empty);
            // The assertions on the Content list in contrast don't lies, as
            // non-bound values are updated in the Razor view:
            Assert.That(this.Model.Content.Count, Is.EqualTo(1));
            var firstString = this.Model.Content[0];
            Assert.That(firstString, Is.EqualTo("a first content line"));

            this.WriteID("ContentTextBox", "a second content line");
            this.Click("SubmitButton");
            Assert.That(this.Model.ContentTextBox, Is.Empty);
            Assert.That(this.Model.Content.Count, Is.EqualTo(2));
            var firstString2 = this.Model.Content[0];
            Assert.That(firstString2, Is.EqualTo("a first content line"));
            var secondString = this.Model.Content[1];
            Assert.That(secondString, Is.EqualTo("a second content line"));
        }
    }
}