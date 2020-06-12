using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NUnit.Engine;
using System;
using System.IO;

namespace iie
{
    public class TestRunner : TestRunnerBase, ITestEventListener
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public TestRunner(IConfiguration config, IWebHostEnvironment env, int port) : base(port)
        {
            Configuration = config;
            Environment = env;

            this.Configure(Configuration.GetValue<int>("RequestTimeout"),
                           Configuration.GetValue<bool>("IEVisible"));
        }

        /// <summary>
        /// Run the test suite in the given project (dll and project name) with
        /// the configured TestFilterWhere. For the path resolution to work,
        /// the test project must be located in the same directory as the
        /// asp.net project. For the Where syntax, see
        /// NUnit.Engine.TestSelectionParser.ParseFilterElement()
        /// </summary>
        /// <param name="testproject"></param>
        public void Run(string testproject)
        {
            // To avoid a cyclic project dependency, the test DLL must be read
            // from an explicit path in the file system, and in .NET Code,
            // it additionally must be formally referenced, therefore the
            // diff / if errorlevel 1 xcopy construct in the post build event
            // to avoid endlessly recompiling a newer, but identical DLL
            // in a cyclic dependency loop.
            var approot = Environment.ContentRootPath;
            var dll = Path.Combine(approot, @"..\bin", testproject + ".dll");
            var package = new TestPackage(dll); // no TestEngineActivator.CreateInstance() in nunit.engine.netstandard
            // NUnit.EnginePackageSettings
            package.AddSetting("ProcessModel", "Single");
            package.AddSetting("DomainUsage", "None");  // irrelevant in core

            using (var engine = new TestEngine())
            using (var runner = engine.GetRunner(package))
            {
                var filter = TestFilter.Empty;
                var where = Configuration["TestFilterWhere"];
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