using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Diagnostics;

namespace asptestrunner
{
    // EdgeDriver ignores App.config "RequestTimeout" in
    // driver.Manage().Timeouts().PageLoad -> use ChromeDriver for now
    [TestFixture]
    [Category("ITestServer")]
    public class Runner : SeleniumTest<ChromeDriver>, ITestServer
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
        public void RunSeleniumTests()
        {
            this.Navigate("/default.aspx", pause: 5000); // very long 1st load time after rebuild
            this.driver.Navigate().Refresh();
            this.ClickID("ContentPlaceHolder1_testButton"); // manually look up WebForms id with out of process tests
            this.AssertTestsOK();
        }

        [Test]
        public void RunSpecFlowTests()
        {
            this.Navigate("/default.aspx", pause: 5000);
            this.driver.Navigate().Refresh();
            this.ClickID("ContentPlaceHolder1_testButtonSpecFlow");
            this.AssertTestsOK();
        }
    }
}