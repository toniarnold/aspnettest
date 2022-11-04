using asplib.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit;
using System.Xml;

namespace iselenium
{
    /// <summary>
    /// Implement a Testrunner as SMC IAppClass for live updates of the status.
    /// </summary>
    public class TestRunnerFsm : TestRunner, IAppClass<TestRunnerFsmContext, TestRunnerFsmContext.TestRunnerFsmState>
    {
        protected TestRunnerFsmContext _fsm = default!;

        public TestRunnerFsm(IConfiguration config, IWebHostEnvironment env, int port) : base(config, env, port)
        {
            this.Construct();
        }

        [ActivatorUtilitiesConstructor]
        public TestRunnerFsm(IConfiguration config, IWebHostEnvironment env) : base(config, env, 0)
        {
            this.Construct();
        }

        public TestRunnerFsm() : base(0)
        {
            this.Construct();
        }

        /// <summary>
        /// Separate constructor method for inheriting in NUnit
        /// </summary>
        protected void Construct()
        {
            this._fsm = new TestRunnerFsmContext(this);
        }

        public TestRunnerFsmContext Fsm
        {
            get { return this._fsm; }
        }

        public TestRunnerFsmContext.TestRunnerFsmState State
        {
            get { return this._fsm.State; }
            set { this._fsm.State = value; }
        }

        public string LastTestName { get; private set; } = String.Empty;
        public TestStatus LastTestStatus { get; private set; } = TestStatus.Passed;

        public override void OnTestEvent(string report)
        {
            base.OnTestEvent(report);

            // See NUnit.ConsoleRunner.TestEventHandler
            var doc = new XmlDocument();
            doc.LoadXml(report);
            var testEvent = doc.FirstChild;
            if (testEvent?.Name == "test-case")
            {
                LastTestName = testEvent?.Attributes?["fullname"]?.Value ?? String.Empty;
                var status = testEvent?.GetAttribute("label") ?? testEvent?.GetAttribute("result");
                TestStatus resultStatus;
                TestStatus.TryParse(status, out resultStatus);
                LastTestStatus = resultStatus;

                Fsm.OnTestEvent(resultStatus);
            }
        }
    }
}