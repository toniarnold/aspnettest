using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Diagnostics;

namespace minimaltestrunner
{
    [TestFixture]
    [Category("ITestServer")]
    public class Runner : SeleniumTest<EdgeDriver>, ITestServer
    {
        public Process ServerProcess { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.StartServer();
        }

        [TearDown]
        public void TearDown()
        {
            this.StopServer();
        }

        [Test]
        public void RunTests()
        {
            this.Navigate("/default.aspx");
            this.driver.Navigate().Refresh();
            this.ClickID("testButton"); // manually look up WebForms id with out of process tests
            this.AssertTestsOK();
        }
    }
}