using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;
using System.Diagnostics;

namespace asptestrunner
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
            this.Navigate("/default.aspx", pause: 5000); // very long 1st load time after rebuild
            this.driver.Navigate().Refresh();
            this.ClickID("ContentPlaceHolder1_testButton"); // manually look up WebForms id with out of process tests
            this.AssertTestsOK();
        }
    }
}