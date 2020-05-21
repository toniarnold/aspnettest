using iselenium;
using NUnit.Framework;
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
        public void ReadTestResult()
        {
            var filename = Path.GetFullPath(
                            Path.Join(TestContext.CurrentContext.WorkDirectory,
                                    @"..\..\..\iselenium",
                                    "TestResult-example.xml"));
            var doc = new XmlDocument();
            doc.Load(filename);
            TestRunnerBase.Result = doc.LastChild;
        }

        [Test]
        public void OnlyFailedTest()
        {
            var failures = TestRunnerBase.OnlyFailed(TestRunnerBase.Result);
            var all = Flat(TestRunnerBase.Result);
            var allFailures = Flat(failures);
            Assert.That(all.Count(), Is.GreaterThan(allFailures.Count()));
            // <test-run ... failed="5" passed="44" total="49" result="Failed" testcasecount="49" ... >
            Assert.That(all.Count(), Is.EqualTo(49));
            Assert.That(allFailures.Count(), Is.EqualTo(5));

            var failureXml = allFailures.First().ToString();
            Assert.That(failureXml, Does.Contain("<message>"));
            Assert.That(failureXml, Does.Contain("<stack-trace>"));
        }

        private IEnumerable<XNode> Flat(XmlNode node)
        {
            var nested = XElement.Parse(node.OuterXml);
            return nested.DescendantsAndSelf("test-case");
        }
    }
}