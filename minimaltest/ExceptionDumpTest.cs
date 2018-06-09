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
    /// Exception dumps into the database require storage (IStorageControl), but 
    /// work also when the site's storage is only Viewstate.
    /// </summary>
    [TestFixture]
    public class ExceptionDumpTest : IIE
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
        public void ThrowRetrieveDumpTest()
        {
            this.Navigate("/minimal/withstorage.aspx");
            this.Write("contentTextBox", "a benign content line");
            this.Click("submitButton");
            this.AssertBenignLine();
            this.Write("contentTextBox", "Except");
            this.Click("submitButton", expectedStatusCode: 500);
            Assert.That(this.Html(), Does.Contain("Malicious Content Exception")); 

            // The benign content in the Viewstate is lost -> Navigate to the Main dump on the ysod-Page
            var linkToDump = this.GetHTMLElement(IEExtension.EXCEPTION_LINK_ID);
            Assert.That(linkToDump.getAttribute("href"), Does.Contain("/withstorage.aspx?session="));
            this.ClickID(IEExtension.EXCEPTION_LINK_ID);
            this.AssertBenignLine();    // restored from the dump before the exception
        }

        private void AssertBenignLine()
        {
            var benign = ((BulletedList)this.GetControl("contentList")).Items[0];
            Assert.Multiple(() =>
            {
                Assert.That(((BulletedList)this.GetControl("contentList")).Items.Count, Is.EqualTo(1));
                Assert.That(benign.Text, Is.EqualTo("a benign content line"));
            });
        }
    }
}
