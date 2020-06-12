using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Diagnostics;

namespace asptestrunner
{
    [TestFixture]
    [Category("ITestServer")]
    public class FSharpRunner : SpaTest<EdgeDriver>, ITestServer
    {
        public Process ServerProcess { get; set; }

        [SetUp]
        public void SetUp()
        {
            var config = this.GetConfig();
            this.StartServer(config, server: config["ServerFSharp"], root: config["RootCFharp"]);
        }

        [TearDown]
        public void TearDown()
        {
            this.StopServer();
        }

        [Test]
        public void RunTests()
        {
            this.Navigate("/", delay: 7000);    // server not yet ready on 1st run
            this.AssertPoll(() => this.GetHTMLElementById("testButton").Displayed, () => Is.True);
            this.Click("testButton");
            this.AssertTestsOK();
        }
    }
}