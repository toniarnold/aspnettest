using NUnit.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace iselenium
{
    public class TestRunner : TestRunnerBase, ITestEventListener
    {
        public TestRunner(int port) : base(port)
        {
            // Configure with defaults:
            this.Configure(
                String.IsNullOrWhiteSpace(
                    ConfigurationManager.AppSettings["RequestTimeout"]) ? 1 :
                    int.Parse(ConfigurationManager.AppSettings["RequestTimeout"]),
                String.IsNullOrWhiteSpace(
                    ConfigurationManager.AppSettings["IEVisible"]) ? true :
                    bool.Parse(ConfigurationManager.AppSettings["IEVisible"]),
                String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TestWriteThrottle"]) ? 0 :
                    int.Parse(ConfigurationManager.AppSettings["TestWriteThrottle"])
                );
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
            base.Run(testproject, HttpContext.Current.Server.MapPath("~"),
                     ConfigurationManager.AppSettings["TestFilterWhere"]);
        }

        // TestEngineActivator specific for .NET Framework
        protected override ITestEngine CreateTestEngine()
        {
            return TestEngineActivator.CreateInstance();
        }
    }
}