using iie;
using minimal.Controllers;
using minimal.Models;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace minimaltest.core
{
    [TestFixture]
    public class ExceptionDumpTest : IETest
    {
        /// <summary>
        /// Typed accessor for the controller under thest
        /// </summary>
        public WithStorageController Controller
        {
            get { return this.GetController<WithStorageController>(); }
        }

        /// <summary>
        /// Typed accessor for the only ViewModel used in the app
        /// </summary>
        public WithStorageViewModel Model
        {
            get { return this.Controller.Model; }
        }

        [Test]
        public void ThrowRetrieveDumpTest()
        {
            this.Navigate("/WithStorage");
            this.Write("ContentTextBox", "a benign content line");
            this.Click("submitButton");
            this.AssertBenignLine();
            this.Write("ContentTextBox", "Except");
            this.Click("submitButton", expectedStatusCode: 200);      // UseDeveloperExceptionPage yields no 500
            Assert.That(this.Html(), Does.Contain("Malicious Content Exception"));
            // The benign content in the ViewState is lost on the Error page,
            // but a link to the stored session has been smuggled into the request header.
            // -> Navigate to the core dump of the controller.
            string coredumpPath = null;
            var reUrl = new Regex(@"_ERROR_PAGE.*?http:\/\/[^\/]+([^<]+)<", RegexOptions.Singleline);
            var match = reUrl.Match(this.Html());
            if (match != null)
            {
                coredumpPath = match.Groups[1].Value;
            }
            Assert.That(coredumpPath, Does.StartWith("/WithStorage?session="));
            this.Navigate(coredumpPath);
            this.AssertBenignLine();    // restored from the dump before the exception
            this.TearDownIE();
            // Next week the bug is still unresolved -> do more postmortem debugging
            this.SetUpIE();
            this.Navigate(coredumpPath);
            this.AssertBenignLine();    // restored again in a new Internet Explorer instance
        }

        private void AssertBenignLine()
        {
            Assert.Multiple(() =>
            {
                // Model
                Assert.That(this.Model.Content, Has.Exactly(1).Items);
                Assert.That(this.Model.Content[0], Is.EqualTo("a benign content line"));
                // Controller
                Assert.That(this.Controller.ContentList, Has.Exactly(1).Items);
                Assert.That(this.Controller.ContentList[0], Is.EqualTo("a benign content line"));
            });
        }
    }
}