using NUnit.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace iselenium
{
    /// <summary>
    /// Common base class for the distinct .NET Framework and .NET Core TestRunners
    /// Handles basic NUnit test runner configuration and test results
    /// </summary>
    public abstract class TestRunnerBase : ITestEventListener
    {
        public TestRunnerBase(int port)
        {
            // Selenium will be started by the NUnit test runner itself, need static configure surrogate
            SeleniumExtensionBase.Port = port;
            // Initialize static fields when creating a new instance
            Reports = new List<string>();
            Result = null;
        }

        /// <summary>
        /// Directly configure the test engine without dependency on a specific
        /// configuration framework.
        /// </summary>
        protected void Configure(int requestTimeout, bool ieVisible)
        {
            SeleniumExtensionBase.RequestTimeout = requestTimeout;
            SeleniumExtensionBase.IEVisible = ieVisible;
        }

        #region IIE Compatibility

        /// <summary>
        /// Return the result as XML string
        /// </summary>
        [Obsolete("Replaced by ResultXml")]
        public string ResultString
        {
            get { return StaticResultString; }
        }

        /// <summary>
        /// Return the result as XML string, static for later retrieval after
        /// the tests ran (ASP.NET Core without internal ViewState)
        /// </summary>
        [Obsolete("Replaced by ResultXml")]
        public static string StaticResultString
        {
            get
            {
                using (var stringwriter = new StringWriter())
                using (var xmlwriter = new XmlTextWriter(stringwriter))
                {
                    Result.WriteTo(xmlwriter);
                    return stringwriter.ToString();
                }
            }
        }

        #endregion IIE Compatibility

        /// <summary>
        /// Return the test results as XML string
        /// static for later retrieval after the tests ran
        /// </summary>
        public static string ResultXml
        {
            get
            {
                using (var stringwriter = new StringWriter())
                using (var xmlwriter = new XmlTextWriter(stringwriter))
                {
                    Result.WriteTo(xmlwriter);
                    return stringwriter.ToString();
                }
            }
        }

        /// <summary>
        /// Return only the failed test results as XML string
        /// static for later retrieval after the tests ran
        /// </summary>
        public static string ResultFailedXml
        {
            get
            {
                using (var stringwriter = new StringWriter())
                using (var xmlwriter = new XmlTextWriter(stringwriter))
                {
                    ResultFailures.WriteTo(xmlwriter);
                    return stringwriter.ToString();
                }
            }
        }

        /// <summary>
        /// True when the test suite passed
        /// </summary>
        public bool Passed
        {
            get
            {
                return Result.Attributes["result"].Value == "Passed";
            }
        }

        /// <summary>
        /// Summary to display instead of the result XML file as list of lines
        /// </summary>
        public List<string> Summary
        {
            get
            {
                var retwal = new List<string>();
                retwal.Add("Passed");
                retwal.Add(string.Format("Tests: {0}", Result.Attributes["total"].Value));
                retwal.Add(string.Format("Asserts: {0}", Result.Attributes["asserts"].Value));
                retwal.Add(string.Format("Duration: {0}", Result.Attributes["duration"].Value));
                return retwal;
            }
        }

        /// <summary>
        /// Summary to display instead of the result XML file as HTML snippet
        /// </summary>
        public string SummaryHtml
        {
            get
            {
                return string.Join("<br />", this.Summary);
            }
        }

        public void Run(string testproject, string approot, string testFilterWhere)
        {
            try
            {
                TestServerIPC.CreateOrOpenMmmfs();

                // To avoid a cyclic project dependency, the test DLL must be read
                // from an explicit path in the file system, and in .NET Code,
                // it additionally must be formally referenced, therefore the
                // diff / if errorlevel 1 xcopy construct in the post build event
                // to avoid endlessly recompiling a newer, but identical DLL
                // in a cyclic dependency loop.
                var dll = Path.Combine(approot, @"..\bin", testproject + ".dll");
                var package = new TestPackage(dll);
                // NUnit.EnginePackageSettings
                package.AddSetting("ProcessModel", "Single");
                package.AddSetting("DomainUsage", "None");  // irrelevant in core
                using (var engine = CreateTestEngine())
                using (var runner = engine.GetRunner(package))
                {
                    var filter = TestFilter.Empty;
                    if (!String.IsNullOrWhiteSpace(testFilterWhere))
                    {
                        var builder = new TestFilterBuilder();
                        builder.SelectWhere(testFilterWhere);
                        filter = builder.GetFilter();   // returns TestFilter.Empty when no TestFilterWhere is given
                    }
                    Result = runner.Run(this, filter);
                    // Communicate results back to the caller process (additionally to the static Result).
                    TestServerIPC.TestSummary = String.Join("\n", this.Summary);
                    TestServerIPC.TestResultXml = ResultXml;
                    TestServerIPC.TestResultFailedXml = ResultFailedXml;
                }
            }
            finally
            {
                TestServerIPC.IsTestRunning = false;
            }
        }

        // Only for .NET Core
        protected virtual ITestEngine CreateTestEngine()
        {
            return TestEngineActivator.CreateInstance();
        }

        /// <summary>
        /// Event raised by the NUnit test runner
        /// </summary>
        /// <param name="report"></param>
        public void OnTestEvent(string report)
        {
            Reports.Add(report);
        }

        private static List<string> Reports { get; set; }

        /// <summary>
        /// To be assigned by the concrete TestRunner: TestRunnerBase.Result = runner.Run(this, filter);
        /// </summary>
        internal static XmlNode Result { get; set; }

        // lazy computation out of the static Result
        private static XmlNode ResultFailures
        {
            get
            {
                return OnlyFailed(Result);
            }
        }

        /// <summary>
        /// Recursion entry point for filtering only the failed test results
        /// </summary>
        /// <param name="testResult">complete test result coming from NUnit</param>
        /// <returns>only the failed test results</returns>
        internal static XmlNode OnlyFailed(XmlNode testResult)
        {
            var failed = new XmlDocument();
            OnlyFailed(testResult, failed, failed);
            return failed;
        }

        /// <summary>
        /// Recursively descend the "all" tree and only append the elements with
        /// result="Failed" to the "failed" tree.
        /// </summary>
        /// <param name="all">all test results</param>
        /// <param name="failed">initially empty document for only the failed test results</param>
        private static void OnlyFailed(XmlNode all, XmlNode failed, XmlDocument failedDoc)
        {
            foreach (XmlNode child in all.ChildNodes)
            {
                var result = child.Attributes["result"];
                if (result != null && result.Value == "Failed")
                {
                    if (child.Name == "test-case")  // is leaf?
                    {
                        var failedTestCase = failedDoc.ImportNode(child, true);  // incl. message and stack-trace
                        failed.AppendChild(failedTestCase); // terminate
                    }
                    else
                    {
                        var failedChild = failedDoc.ImportNode(child, false);   // only the node itself
                        failed.AppendChild(failedChild);
                        OnlyFailed(child, failedChild, failedDoc);  // continue depth-first with the empty failedChild
                    }
                }
            }
        }
    }
}