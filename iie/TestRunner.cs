using NUnit.Engine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using System.Xml;

namespace iie
{
    [TestOfAttribute("SHDocVw.InternetExplorer")]   // ensures that NUnit.Framework.dll gets included
    public class TestRunner : ITestEventListener
    {
        public TestRunner(int port)
        {
            IEExtension.Port = port;
        }

        private XmlNode result;
        private List<string> reports = new List<string>();

        /// <summary>
        /// Return the result as XML string
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string ResultString
        {
            get
            {
                using (var stringwriter = new StringWriter())
                using (var xmlwriter = new XmlTextWriter(stringwriter))
                {
                    this.result.WriteTo(xmlwriter);
                    return stringwriter.ToString();
                }
            }
        }

        public bool Passed
        {
            get
            {
                return this.result.Attributes["result"].Value == "Passed";
            }
        }

        public string PassedString
        {
            get
            {
                return string.Format("Passed<br/>Tests: {0}<br/>Asserts: {1}<br/>Duration: {2}",
                    this.result.Attributes["total"].Value,
                    this.result.Attributes["asserts"].Value,
                    this.result.Attributes["duration"].Value
                    );
            }
        }

        public List<string> Reports
        {
            get { return this.reports; }
        }

        public void OnTestEvent(string report)
        {
            this.reports.Add(report);
        }

        /// <summary>
        /// Run the test suite in the given project name with the given TestFilterWhere
        /// For the Where syntax, see NUnit.Engine.TestSelectionParser.ParseFilterElement()
        /// </summary>
        /// <param name="testproject"></param>
        public void Run(string testproject)
        {
            // To avoid a cyclic project dependency, the dll must be read from an explicit path in the filesystem
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("IE tests must run in the w3wp.exe address space");
            }
            var approot = HttpContext.Current.Server.MapPath("~");
            var bin = Path.Combine(approot, String.Format(@"..\{0}\bin", testproject));
#if DEBUG
            string folder = "Debug";
#else
            string folder = "Release";
#endif
            var dll = Path.Combine(bin, folder, testproject + ".dll");
            var package = new TestPackage(dll);
            // NUnit.EnginePackageSettings
            package.AddSetting("ProcessModel", "InProcess");
            package.AddSetting("DomainUsage", "None");

            var x = IEExtension.Port;
            using (var engine = TestEngineActivator.CreateInstance())
            using (var runner = engine.GetRunner(package))
            {
                var filter = TestFilter.Empty;
                var where = ConfigurationManager.AppSettings["TestFilterWhere"];
                if (!String.IsNullOrWhiteSpace(where))
                {
                    var builder = new TestFilterBuilder();
                    builder.SelectWhere(where);
                    filter = builder.GetFilter();   // returns TestFilter.Empty when no TestFilterWhere is given
                }
                this.result = runner.Run(this, filter);
            }
        }
    }
}