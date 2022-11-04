using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NUnit.Engine;
using System;

namespace iselenium
{
    public class TestRunner : TestRunnerBase, ITestEventListener
    {
        protected IConfiguration Configuration { get; } = default!;
        protected IWebHostEnvironment Environment { get; } = default!;

        public TestRunner(IConfiguration config, IWebHostEnvironment env, int port) : base(port)
        {
            Configuration = config;
            Environment = env;

            this.Configure(Configuration.GetValue<int>("RequestTimeout"),
                           Configuration.GetValue<bool>("IEVisible"),
                           String.IsNullOrWhiteSpace(config?["TestWriteThrottle"]) ? 0 :
                                config.GetValue<int>("TestWriteThrottle")
                );
        }

        public TestRunner(int port) : base(port)
        { }

        /// <summary>
        /// Run the test suite in the given project (dll and project name) with
        /// the configured TestFilterWhere. For the path resolution to work,
        /// the test project must be located in the same directory as the
        /// asp.net project. For the Where syntax, see
        /// NUnit.Engine.TestSelectionParser.ParseFilterElement()
        /// </summary>
        /// <param name="testproject"></param>
        public void Run(string testproject, ITestEventListener? listener = null)
        {
            base.Run(testproject, Environment.ContentRootPath, Configuration["TestFilterWhere"], listener);
        }
    }
}