using System;
using System.Collections.Generic;
using System.Text;
using iie;
using System.Reflection;
using System.IO;
using NUnit.Engine;
using NUnit.Framework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace iie
{
    public class TestRunner : TestRunnerBase
    {
        private IConfigurationRoot Configuration { get; }
        private IHostingEnvironment Environment { get; }

        public TestRunner(IConfigurationRoot config, IHostingEnvironment env, int port) : base(port)
        {
            Configuration = config;
            Environment = env;

            this.Configure(Configuration.GetValue<int>("RequestTimeout"));
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
            var approot = Environment.ContentRootPath;
            var bin = Path.Combine(approot, String.Format(@"..\{0}\bin", testproject));
#if DEBUG
            string folder = "Debug";
#else
            string folder = "Release";
#endif
            var dll = Path.Combine(bin, folder, "netcoreapp2.1", testproject + ".dll");
            var package = new TestPackage(dll); // no TestEngineActivator.CreateInstance() in nunit.engine.netstandard
            // NUnit.EnginePackageSettings
            package.AddSetting("ProcessModel", "InProcess");
            package.AddSetting("DomainUsage", "None");  // irrelevant in core

            var x = IEExtension.Port;
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
