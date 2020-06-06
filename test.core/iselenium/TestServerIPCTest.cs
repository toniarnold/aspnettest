using iselenium;
using NUnit.Framework;
using System;

namespace test.iselenium
{
    [TestFixture]
    public class TestServerIPCTest
    {
        [SetUp]
        public void CreateMmfs()
        {
            TestServerIPC.CreateOrOpenMmmfs();
        }

        [TearDown]
        public void DisposeMmfs()
        {
            TestServerIPC.Dispose();
        }

        [Test]
        public void IsTestRunningTest()
        {
            Assert.That(TestServerIPC.IsTestRunning, Is.True);  // always true on initialization
            TestServerIPC.IsTestRunning = false;
            Assert.That(TestServerIPC.IsTestRunning, Is.False);
        }

        [Test]
        public void IsTestRunningDisposedTest()
        {
            this.DisposeMmfs();
            this.DisposeMmfs();
            // Default: uninitialized must not throw
            Assert.That(TestServerIPC.IsTestRunning, Is.False);
            // finally {} in TestRunnerBase.Run after an exception must not throw
            TestServerIPC.IsTestRunning = false;
            // But false claims to be running must throw
            Assert.That(() => TestServerIPC.IsTestRunning = true,
                Throws.Exception.TypeOf<NullReferenceException>());
        }

        [Test]
        public void TestSummaryTest()
        {
            const string RESULT = @"
Passed
Tests: 11
Asserts: 128
Duration: 65.377405";

            TestServerIPC.TestSummary = RESULT;
            Assert.That(TestServerIPC.TestSummary, Is.EqualTo(RESULT));
        }

        [Test]
        public void TestResultXmlTest()
        {
            var xml = TestRunnerBaseTest.ReadTestResult().ToString(); ;
            TestServerIPC.TestResultXml = xml;
            Assert.That(TestServerIPC.TestResultXml, Is.EqualTo(xml));
        }

        [Test]
        public void TestResultFailedXmlTest()
        {
            var xml = TestRunnerBaseTest.ReadTestResult().ToString(); ;
            TestServerIPC.TestResultFailedXml = xml;
            Assert.That(TestServerIPC.TestResultFailedXml, Is.EqualTo(xml));
        }
    }
}