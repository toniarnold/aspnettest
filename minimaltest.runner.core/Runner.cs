using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.IE;
using System.Diagnostics;

namespace minimaltest.runner
{
    [TestFixture]
    [Category("ITestServer")]
    public class Runner : SeleniumTest<InternetExplorerDriver>, ITestServer
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
            this.Navigate("/");
            this.ClickID("testButton");
            this.AssertTestsOK();
        }
    }
}