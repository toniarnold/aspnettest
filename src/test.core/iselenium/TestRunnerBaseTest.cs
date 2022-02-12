using iselenium;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace test.iselenium
{
    [TestFixture]
    public class TestRunnerBaseTest
    {
        /// <summary>
        /// Mocks testRunnerBase.Result = runner.Run(this, filter);
        /// </summary>
        [OneTimeSetUp]
        public void AssignTestResult()
        {
            TestRunnerBase.Result = ReadTestResult("TestResult-failed.xml");
        }

        /// <summary>
        /// Also used by TestServerIPCTest
        /// </summary>
        /// <returns></returns>
        public static XmlNode ReadTestResult(string name)
        {
            var filename = Path.GetFullPath(
                            Path.Join(TestContext.CurrentContext.WorkDirectory,
                                        "..", "..", "..", "iselenium", "TestResult", name));
            var doc = new XmlDocument();
            doc.Load(filename);
            return doc.LastChild;
        }

        [Test]
        public static void TestStatusPassedTest()
        {
            TestRunnerBase.Result = ReadTestResult("TestResult-passed.xml");
            Assert.That(TestRunnerBase.TestStatus, Is.EqualTo(TestStatus.Passed));
        }

        [Test]
        public static void TestStatusFailedTest()
        {
            TestRunnerBase.Result = ReadTestResult("TestResult-failed.xml");
            Assert.That(TestRunnerBase.TestStatus, Is.EqualTo(TestStatus.Failed));
        }

        [Test]
        public static void TestStatusSkippedTest()
        {
            TestRunnerBase.Result = ReadTestResult("TestResult-skipped.xml");
            Assert.That(TestRunnerBase.TestStatus, Is.EqualTo(TestStatus.Skipped));
        }

        [Test]
        public void TestResultFilteredFailedTest()
        {
            var failures = TestRunnerBase.TestResultFiltered(TestRunnerBase.Result, TestStatus.Failed);
            var all = Flat(TestRunnerBase.Result);
            var allFailures = Flat(failures);
            Assert.That(all.Count(), Is.GreaterThan(allFailures.Count()));
            // <test-run ... failed="5" passed="44" total="49" result="Failed" testcasecount="49" ... >
            Assert.That(all.Count(), Is.EqualTo(49));
            Assert.That(allFailures.Count(), Is.EqualTo(5));

            var failureXml = allFailures.First().ToString();
            Assert.That(failureXml, Does.Contain("<message>"));
            Assert.That(failureXml, Does.Contain("<stack-trace>"));

            Assert.That(failureXml, Does.Contain("result=\"Failed\""));
            Assert.That(failureXml, Does.Not.Contain("result=\"Passed\""));
        }

        private IEnumerable<XNode> Flat(XmlNode node)
        {
            var nested = XElement.Parse(node.OuterXml);
            return nested.DescendantsAndSelf("test-case");
        }

        [Test]
        public void ResultXmlFormatTest()
        {
            var xml = TestRunnerBase.ResultXml;
            Assert.That(!String.IsNullOrEmpty(xml));
            Assert.Pass(xml);
        }

        [Test]
        public void IsRunnableTest()
        {
            TestRunnerBase.Result = ReadTestResult("TestResult-notrunnable.xml");
            Assert.That(TestRunnerBase.IsRunnable, Is.False);
        }
    }
}