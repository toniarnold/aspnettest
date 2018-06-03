using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using iie;

using asplib.View;

using testie.asp.calculator;


namespace testie.asp
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public class ExceptionDumpTest : TestBase
    {
        [Test]
        public void ThrowDumpTest()
        {
            // Create a unique test number to store with the exception
            var rnd = new Random();
            var unique = rnd.NextDouble().ToString();
            this.Navigate("/asp/default.aspx");
            this.Click("footer.enterButton");
            this.Write("enter.operandTextBox", unique);
            this.Click("footer.enterButton");
            Assert.That(this.Stack, Does.Contain(unique));

            // Deliberately throw an exception
            this.Click("footer.enterButton");
            this.Write("enter.operandTextBox", "except");
            this.Click("footer.enterButton", expectedStatusCode: 500);
            Assert.That(this.Html(), Does.Contain("Deliberate Exception"));

            // Navigate to the Main dump on the ysod-Page
            var linkToDump = this.GetHTMLElement(IEExtension.EXCEPTION_LINK_ID);
            Assert.That(linkToDump.getAttribute("href"), Does.Contain("/default.aspx?session="));
            this.ClickID(IEExtension.EXCEPTION_LINK_ID);

            Assert.Multiple(() =>
            {
                // The non-initial State and the random number must come from the database with Storage.Viewstate
                Assert.That(ControlMainExtension.SessionStorage, Is.EqualTo(Storage.Viewstate));
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
                Assert.That(this.Stack, Does.Contain(unique));
            });
        }
    }
}
