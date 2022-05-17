using asplib.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace iselenium.Components
{
    public partial class TestButton
    {
        private const int TESTRESULT_MINWIDTH = 10; // em, determines the position of the result link from the right

        [Parameter]
        public string? src { get; set; } = null;

        [Parameter]
        public string testproject { get; set; } = "";

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
        protected string imageText => State.ToString();

        private const string _contentPath = "/_content/iselenium.blazor";

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
                    case var s when s == TestRunnerFsmContext.Map1.RunningWarning:
                        return $"{_contentPath}/nunit-warning.png";

                    case var s when s == TestRunnerFsmContext.Map1.RunningError:
                        return $"{_contentPath}/nunit-error.png";

                    default:
                        return $"{_contentPath}/nunit.png";
                }
            }
        }

        protected override void OnInitialized()
        {
            SetStorage(Storage.ViewState); // TestRunnerFsm is not serializable -> disable serialization for the component
            SeleniumExtensionBase.Port = (int)(Http.HttpContext?.Request.Host.Port ?? 0);
        }

        protected override void ReRender()
        {
            // Display the last warning/failure end leave it there
            switch (Main.LastTestStatus)
            {
                case TestStatus.Warning:
                    TestResult = $"Warning....<br />{Main.LastTestName}";
                    break;

                case TestStatus.Failed:
                    TestResult = $"Failure...<br />{Main.LastTestName}";
                    break;
            }
        }

        public async Task Test()
        {
            Fsm.RunTests();
            TestResult = "Running...";
            spin = "spin";
            await Task.Run(() => RunTests());
            spin = "";
        }

        public async Task RunTests()
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