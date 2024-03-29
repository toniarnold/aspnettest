﻿using iselenium;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using System.Diagnostics;

namespace BlazorApp1SeleniumTestRunner
{
    [Category("ITestServer")]
    public class Runner : SeleniumTest<EdgeDriver>, ITestServer
    {
        public List<Process> ServerProcesses { get; set; } = new();

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
            this.Navigate("/", pause: 200); // allow the testButton time to render
            this.ClickID("testButton"); // same as testButton-BlazorApp1SeleniumTest
            this.AssertTestsOK();
        }

        [Test]
        public void RunSpecFlowTests()
        {
            this.Navigate("/", pause: 200);
            this.ClickID("testButton-BlazorApp1SpecFlowTest");
            this.AssertTestsOK();
        }
    }
}