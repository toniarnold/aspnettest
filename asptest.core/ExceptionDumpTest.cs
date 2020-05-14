using asplib.Model;
using asptest.Calculator;
using iselenium;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace asptest
{
    [TestFixture]
    public class ExceptionDumpTest : CalculatorTestBase
    {
        [OneTimeSetUp]
        public void SetUpStorage()
        {
            StorageImplementation.SessionStorage = Storage.ViewState;
        }

        [OneTimeTearDown]
        public void TearDownStorage()
        {
            StorageImplementation.SessionStorage = null;
        }

        [Test]
        public void ThrowDumpTest()
        {
            // Create a unique test number to store with the exception
            var rnd = new Random();
            var unique = rnd.NextDouble().ToString();
            this.Navigate("/");
            this.Click("EnterButton");
            this.Write("Operand", unique);
            this.Click("EnterButton");
            Assert.That(this.Stack, Does.Contain(unique));

            // Deliberately throw an exception
            this.Click("EnterButton");
            this.Write("Operand", "except");
            this.Click("EnterButton", expectedStatusCode: 200);      // UseDeveloperExceptionPage yields no 500
            Assert.That(this.Html(), Does.Contain("Deliberate Exception"));

            // Navigate to the Code dump on the (again blue) bsod Page
            string coredumpPath = null;
            var reUrl = new Regex(@"_CORE_DUMP.*?http:\/\/[^\/]+([^<]+)<", RegexOptions.Singleline);
            var match = reUrl.Match(this.Html());
            if (match != null)
            {
                coredumpPath = match.Groups[1].Value;
            }
            Assert.That(coredumpPath, Does.StartWith("/Calculator?session="));
            this.Navigate(coredumpPath);

            Assert.Multiple(() =>
            {
                // The non-initial State and the random number must come from the database with Storage.ViewState
                Assert.That(StorageImplementation.SessionStorage, Is.EqualTo(Storage.ViewState));
                Assert.That(this.State, Is.EqualTo(CalculatorContext.Map1.Enter));
                Assert.That(this.Stack, Does.Contain(unique));
            });
        }
    }
}