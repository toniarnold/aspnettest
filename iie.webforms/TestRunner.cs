using NUnit.Engine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace iie
{
    [TestOfAttribute("SHDocVw.InternetExplorer")]   // ensures that NUnit.Framework.dll gets included
    public class TestRunner : TestRunnerBase, ITestEventListener
    {
        public TestRunner(int port) : base(port)
        {
            this.Configure(String.IsNullOrWhiteSpace(
                ConfigurationManager.AppSettings["RequestTimeout"]) ? 1 :
                int.Parse(ConfigurationManager.AppSettings["RequestTimeout"]));
        }

        private List<string> reports = new List<string>();

        /// <summary>
        /// Run the test suite in the given project (dll and project name) with
        /// the configured TestFilterWhere. For the Where syntax, see
        /// NUnit.Engine.TestSelectionParser.ParseFilterElement()
        /// </summary>
        /// <param name="testproject"></param>
        public void Run(string testproject)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("IE tests must run in the w3wp.exe address space");
            }
            // To avoid a cyclic project dependency, thet test DLL must be read
            // from an explicit path in the file system, but in .NET Framework,
            // it doesn't need to be formally referenced.
            var approot = HttpContext.Current.Server.MapPath("~");
            var dll = Path.Combine(approot, @"..\bin", testproject + ".dll");
            var package = new TestPackage(dll);
            // NUnit.EnginePackageSettings
            package.AddSetting("ProcessModel", "InProcess");
            package.AddSetting("DomainUsage", "None");

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
                TestRunnerBase.Result = runner.Run(this, filter);
            }
        }
    }
}