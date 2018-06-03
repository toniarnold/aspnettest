using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

using NUnit.Framework;
using NUnit.Engine;

namespace iie
{
    [TestOfAttribute("SHDocVw.InternetExplorer")]   // ensures that NUnit.Framework.dll gets included
    public class TestRunner : ITestEventListener
    {
        public TestRunner(int port)
        {
            IEExtension.Port = port;
        }

        XmlNode result;
        List<string> reports = new List<string>();

        /// <summary>
        /// Return the result as XML string
        /// </summary>
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
            // To avoid a cyclic project dependency, the dll must be read from an explict path in the filesystem
            Trace.Assert(HttpContext.Current != null, "IE tests must run in the w3wp.exe adrdress space");
            var approot = HttpContext.Current.Server.MapPath("~");
            var bin = Path.Combine(approot, @"..\testie\bin");
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
                if (!String.IsNullOrEmpty(where))
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
