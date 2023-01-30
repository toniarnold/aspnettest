using iselenium;
using minimal;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Web.UI.WebControls;

namespace minimaltest
{
    /// <summary>
    /// Exception dumps into the database require storage (IStorageControl), but
    /// work also when the site's storage is only ViewState, therefore
    /// inherit from SeleniumDbTest for automatic database cleanup.
    /// </summary>
    [TestFixture(typeof(EdgeDriver))]
    public class ExceptionDumpTestt<TWebDriver> : SeleniumDbTest<TWebDriver, ContentStorage>, ISelenium
        where TWebDriver : IWebDriver, new()
    {
        [Test]
        public void ThrowRetrieveDumpTest()
        {
            this.Navigate("/withstorage.aspx");
            this.Write("contentTextBox", "a benign content line");
            this.Click("submitButton");
            this.AssertBenignLine();
            this.Write("contentTextBox", "Except");
            this.Click("submitButton", expectedStatusCode: 500);
            Assert.That(this.Html(), Does.Contain("Malicious Content Exception"));
            // The benign content in the ViewState is lost on the ysod-Page -> Click the core dump of Main
            var linkToDump = this.GetHTMLElement(SeleniumExtension.EXCEPTION_LINK_ID);
            var coredumpUrl = (string)linkToDump.GetAttribute("href");
            Assert.That(coredumpUrl, Does.Contain("/withstorage.aspx?session="));
            // AS IE was forcibly disabled, this does not work no more:
            //#pragma warning disable CS0618 // IIE obsolete
            //            this.ClickID(IEExtension.EXCEPTION_LINK_ID);
            //            this.AssertBenignLine();    // restored from the dump before the exception
            //            this.TearDownIE();
            //            // Next week the bug is still unresolved -> do more postmortem debugging
            //            this.SetUpIE();
            //#pragma warning restore CS0618 // IIE obsolete
            //            this.NavigateURL(coredumpUrl);
            //this.AssertBenignLine();    // restored again in a new Internet Explorer instance
        }

        private void AssertBenignLine()
        {
            Assert.Multiple(() =>
            {
                // Model
                Assert.That(this.Main.Content, Has.Exactly(1).Items);
                Assert.That(this.Main.Content[0], Is.EqualTo("a benign content line"));
                // View
                Assert.That(((BulletedList)this.GetControl("contentList")).Items.Count, Is.EqualTo(1));
                Assert.That(((BulletedList)this.GetControl("contentList")).Items[0].Text, Is.EqualTo("a benign content line"));
            });
        }
    }
}