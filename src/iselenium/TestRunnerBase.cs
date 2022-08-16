using NUnit.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        protected void Configure(int requestTimeout, bool ieVisible, int throttle)
        {
            SeleniumExtensionBase.RequestTimeout = requestTimeout;
            SeleniumExtensionBase.IEVisible = ieVisible;
            SeleniumExtensionBase.WriteThrottle = throttle;
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
        /// Filtered if the tests didn't pass
        /// </summary>
        public static string ResultXml
        {
            get
            {
                using (var stringwriter = new StringWriter())
                using (var xmlwriter = XmlWriter.Create(stringwriter, XmlSettings()))
                {
                    if (Passed)
                    {
                        Result.WriteTo(xmlwriter);
                    }
                    else if (IsRunnable)
                    {
                        switch (TestStatus)
                        {
                            case TestStatus.Failed:
                            case TestStatus.Inconclusive:
                            case TestStatus.Passed:
                            case TestStatus.Skipped:
                            case TestStatus.Warning:
                                TestResultFiltered(Result, TestStatus).WriteTo(xmlwriter);
                                break;

                            default:     // TestStatus.Unknown
                                Result.WriteTo(xmlwriter);
                                break;
                        }
                    }
                    else    // unfiltered when tests could not be run
                    {
                        Result.WriteTo(xmlwriter);
                    }
                    xmlwriter.Flush();
                    return stringwriter.ToString();
                }
            }
        }

        private static XmlWriterSettings XmlSettings()
        {
            var settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            return settings;
        }

        /// <summary>
        /// True when the test suite passed
        /// </summary>
        public static bool Passed
        {
            get => TestStatus == TestStatus.Passed;
        }

        /// <summary>
        /// The overall status of the whole tests suite reported by NUnit
        /// </summary>
        /// <returns></returns>
        internal static TestStatus TestStatus
        {
            get
            {
                var result = Result.Attributes["result"].Value;
                TestStatus status;
                TestStatus.TryParse(result, out status);
                return status;
            }
        }

        /// <summary>
        /// Used to avoid filtering when the test-suite runstate="NotRunnable"
        /// </summary>
        internal static bool IsRunnable
        {
            get
            {
                var isRunnable = false;
                var testSuite = from XmlNode c in Result.ChildNodes
                                where c.Name == "test-suite"
                                select c;
                if (testSuite.Any())
                {
                    var runstate = from XmlAttribute a in testSuite.FirstOrDefault().Attributes
                                   where a.Name == "runstate"
                                   select a;
                    isRunnable = runstate.Any() ? runstate.FirstOrDefault().Value != "NotRunnable" : false;
                }
                return isRunnable;
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
            get => string.Join("<br />", this.Summary);
        }

        /// <summary>
        /// Load the assembly testproject.dll from the bin directory (side by side deploy)
        /// </summary>
        /// <param name="testproject">DLL name (without suffix) of the test project</param>
        /// <param name="approot">unused</param>
        /// <param name="testFilterWhere">NUnit TestFilter WHERE string, e.g. name==TestName</param>
        /// <param name="listener">Test event callback</param>
        public void Run(string testproject, string approot, string testFilterWhere, ITestEventListener listener = null)
        {
            try
            {
                // To avoid a cyclic project dependency, the test DLL must be read
                // from an explicit path in the file system, and in .NET Code,
                // it additionally must be formally referenced, therefore the
                // diff / if errorlevel 1 xcopy construct in the post build event
                // to avoid endlessly recompiling a newer, but identical DLL
                // in a cyclic dependency loop.
                // Assembly.location is isolated in .NET Framework, therefore use .CodeBase which points to ./bin
                var binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
                var dll = Path.Combine(binPath, testproject + ".dll");
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
                    TestServerIPC.CreateOrOpenMmmfs();  // Open when run as child process, Create otherwise
                    TestServerIPC.TestStatus = TestStatus;
                    TestServerIPC.TestSummary = String.Join("\n", this.Summary);
                    TestServerIPC.TestResultXml = ResultXml;
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
        public virtual void OnTestEvent(string report)
        {
            Reports.Add(report);
        }

        private static List<string> Reports { get; set; }

        /// <summary>
        /// To be assigned by the concrete TestRunner: TestRunnerBase.Result = runner.Run(this, filter);
        /// </summary>
        internal static XmlNode Result { get; set; }

        /// <summary>
        /// Recursion entry point for filtering the test results according to status
        /// </summary>
        /// <param name="testResult">complete test result coming from NUnit</param>
        /// <param name="status">status filter</param>
        /// <returns>only the test results with the given status</returns>

        internal static XmlNode TestResultFiltered(XmlNode testResult, TestStatus status)
        {
            var filtered = new XmlDocument();
            TestResultFiltered(testResult, status, filtered, filtered);
            return filtered;
        }

        /// <summary>
        /// Recursively descend the "all" tree and only append the elements with
        /// result="Failed" to the "failed" tree.
        /// </summary>
        /// <param name="all">all test results</param>
        /// <param name="status">status filter</param>
        /// <param name="filtered">initially empty document for only the failed test results</param>
        /// <param name="filteredDoc">document root of the filtered node</param>
        private static void TestResultFiltered(XmlNode all, TestStatus status, XmlNode filtered, XmlDocument filteredDoc)
        {
            foreach (XmlNode child in all.ChildNodes)
            {
                var result = child.Attributes["result"];
                if (result != null && result.Value == status.ToString())
                {
                    if (child.Name == "test-case")  // is leaf?
                    {
                        var failedTestCase = filteredDoc.ImportNode(child, true);  // incl. message and stack-trace
                        filtered.AppendChild(failedTestCase); // terminate
                    }
                    else
                    {
                        var failedChild = filteredDoc.ImportNode(child, false);   // only the node itself
                        filtered.AppendChild(failedChild);
                        TestResultFiltered(child, status, failedChild, filteredDoc);  // continue depth-first with the empty failedChild
                    }
                }
            }
        }
    }
}