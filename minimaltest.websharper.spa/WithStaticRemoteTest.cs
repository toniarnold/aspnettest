using iie;
using minimal.websharper.spa;
using NUnit.Framework;
using System.Collections.Generic;

namespace minimaltest.websharper.spa
{
    [TestFixture]
    public class WithStaticRemoteTest : IETest
    {
        /// <summary>
        /// Typed accessor to the only model object in the app
        /// </summary>
        public List<string> Content
        {
            get { return StaticRemoting.refContent; }
        }

        // The main objective of an SPA is to withdraw control from the user,
        // i.e. to make deep links impossible...
        [Test]
        public void NavigateWithStaticRemotingTest()
        {
            this.Navigate("/");
            this.ClickID("withstatic-link");
            Assert.That(this.Html(), Does.Contain("<h1>minimalist test setup with static remote</h1>"));
        }

        [Test]
        public void WriteContentTest()
        {
            this.Navigate("/");
            this.ClickID("withstatic-link");
            this.WriteID("contentTextBox", "a first content line");
            this.ClickID("submitButton", expectPostBack: false, pause: 500);

            Assert.That(this.Content.Count, Is.EqualTo(1));
            var firstString = this.Content[0];
            Assert.That(firstString, Is.EqualTo("a first content line"));

            this.WriteID("contentTextBox", "a second content line");
            this.ClickID("submitButton");
            Assert.That(this.Content.Count, Is.EqualTo(2));
            var firstString2 = this.Content[0];
            Assert.That(firstString2, Is.EqualTo("a first content line"));
            var secondString = this.Content[1];
            Assert.That(secondString, Is.EqualTo("a second content line"));
        }
    }
}