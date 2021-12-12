using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;
using System.Diagnostics;

namespace minimaltestrunner
{
    [TestFixture]
    [Category("ITestServer")]
    public class Runner : SeleniumTest<EdgeDriver>, ITestServer
    {
        public List<Process> ServerProcesses { get; set; }

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
            this.driver.Navigate().Refresh();
            this.ClickID("testButton");
            this.AssertTestsOK();
        }
    }
}