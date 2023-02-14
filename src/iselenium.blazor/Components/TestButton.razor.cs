using asplib.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace iselenium.Components
{
    public partial class TestButton
    {
        private string[] _testprojects = new string[0];

        [Parameter]
        public string? src { get; set; } = null;

        /// <summary>
        /// Test project assembly name or a space separated list of assemblies to choose from
        /// </summary>
        [Parameter]
        public string testproject
        {
            set
            {
                _testprojects = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [Parameter]
        public string tabindex { get; set; } = "9999";

        public string TestResult { get; set; } = "";

        [Inject]
        private IWebHostEnvironment Environment { get; set; } = default!;

        [Inject]
        private IHttpContextAccessor Http { get; set; } = default!;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        protected string spin { get; set; } = "";

        private string testprojectListVisibility = "visible";
        protected string imageText => State.ToString();

        private const string _contentPath = "/_content/aspnettest.iselenium.blazor";

        /// <summary>
        /// Display the NUnit image color according to the FSM state
        /// </summary>
        protected string NUnitImgSrc
        {
            get
            {
                if (src != null)
                {
                    return src;
                }
                switch (State)
                {
                    case var s when s == TestRunnerFsmContext.Map1.RunningWarning ||
                                    s == TestRunnerFsmContext.Map1.CompletedWarning:
                        return $"{_contentPath}/nunit-warning.png";

                    case var s when s == TestRunnerFsmContext.Map1.RunningError ||
                                    s == TestRunnerFsmContext.Map1.CompletedError:
                        return $"{_contentPath}/nunit-error.png";

                    default:
                        return $"{_contentPath}/nunit.png";
                }
            }
        }

        protected override void OnInitialized()
        {
            SetStorage(Storage.ViewState); // disable serialization for the component
            SeleniumExtensionBase.Port = (int)(Http.HttpContext?.Request.Host.Port ?? 0);
        }

        protected override void RenderMain()
        {
            // Display the last warning/failure end leave it there
            // The same status conditions as in TestRunnerFsm.sm
            switch (Main.LastTestStatus)
            {
                case TestStatus.Warning:
                case TestStatus.Inconclusive:
                case TestStatus.Skipped:
                    TestResult = $"Warning....<br />{Main.LastTestName}";
                    break;

                case TestStatus.Failed:
                case TestStatus.Unknown:    // Exception
                    TestResult = $"Failure...<br />{Main.LastTestName}";
                    break;
            }
        }

        public async Task Test(string testproject)
        {
            Fsm.RunTests();
            TestResult = "Running...";
            spin = "spin";
            testprojectListVisibility = "hidden";
            await Task.Run(() => RunTests(testproject));
            spin = "";
            this.StateHasChanged();
        }

        public async Task RunTests(string testproject)
        {
            Main.Run(testproject);
            Fsm.Complete();
            Fsm.Report();
            switch (State)
            {
                case var s when s == TestRunnerFsmContext.Map1.Passed:
                    TestResult = Main.SummaryHtml;
                    break;

                case var s when s == TestRunnerFsmContext.Map1.ResultXml:
                    await ShowResultXml();
                    break;
            }
        }

        public async Task ShowResultXml()
        {
            var module = await JS.InvokeAsync<IJSObjectReference>("import", $"{_contentPath}/openXml.js");
            await module.InvokeVoidAsync("openXml", TestRunner.ResultXml);
        }
    }
}