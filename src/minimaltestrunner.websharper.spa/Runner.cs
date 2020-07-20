using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;
using System.Diagnostics;

namespace minimaltestrunner
{
    [TestFixture]
    [Category("ITestServer")]
    public class Runner : SpaTest<EdgeDriver>, ITestServer
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
            this.Navigate("/", delay: 7000); // server not yet ready on 1st run
            this.driver.Navigate().Refresh(); // and the page sometimes doesn't load the 1st time
            this.AssertPoll(() => this.GetHTMLElementById("testButton").Displayed, () => Is.True);
            this.Click("testButton", awaitRemoved: false);
            this.AssertTestsOK();
        }
    }
}