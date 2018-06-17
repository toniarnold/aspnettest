using asplib.View;
using iie;
using NUnit.Framework;
using System;
using testie.asp.calculator;

namespace testie.asp
{
    [TestFixture]
    [Category("SHDocVw.InternetExplorer")]
    public class ExceptionDumpTest : CalculatorTestBase
    {

        [OneTimeSetUp]
        public void SetUpStorage()
        {
            ControlStorageExtension.SessionStorage = Storage.ViewState;
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            ControlStorageExtension.SessionStorage = null;
        }

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
                // The non-initial State and the random number must come from the database with Storage.ViewState
                Assert.That(ControlStorageExtension.SessionStorage, Is.EqualTo(Storage.ViewState));
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
                Assert.That(this.Stack, Does.Contain(unique));
            });
        }
    }
}