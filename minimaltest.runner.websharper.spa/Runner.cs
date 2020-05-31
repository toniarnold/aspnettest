using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Diagnostics;

namespace minimaltest.runner.websharper.spa
{
    [TestFixture]
    [Category("ITestServer")]
    public class Runner : SpaTest<EdgeDriver>, ITestServer
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
            this.Navigate("/", delay: 2000);    // server not yet ready on 1st run
            this.Click("testButton");
            this.AssertTestsOK();
        }
    }
}