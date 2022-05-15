using iselenium;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace asplib.Components
{
    public partial class TestButtonBase : ComponentBase
    {
        [Parameter]
        public string src { get; set; } = "/_content/asplib.blazor/nunit.png";

        [Parameter]
        public string testproject { get; set; } = "";

        [Parameter]
        public string tabindex { get; set; } = "9999";

        public string TestResult { get; set; } = "";

        [Inject]
        private IConfiguration Configuration { get; set; } = default!;

        [Inject]
        private IWebHostEnvironment Environment { get; set; } = default!;

        [Inject]
        private IHttpContextAccessor Http { get; set; } = default!;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        private int Port { get; set; } = 0;

        protected string spin { get; set; } = "";

        protected override void OnInitialized()
        {
            Port = (int)(Http.HttpContext?.Request.Host.Port ?? 0);
        }

        public async Task Test()
        {
            TestResult = "Running...";
            spin = "spin";
            await Task.Run(() => RunTests()); // to immediately show the "Running..."
            spin = "";
        }

        public async Task RunTests()
        {
            var testRunner = new TestRunner(Configuration, Environment, Port);
            testRunner.Run(testproject);
            if (TestRunner.Passed)
            {
                TestResult = testRunner.SummaryHtml;
            }
            else
            {
                await ShowResultXml();
            }
        }

        public async Task ShowResultXml()
        {
            var module = await JS.InvokeAsync<IJSObjectReference>("import", "/_content/asplib.blazor/openXml.js");
            await module.InvokeVoidAsync("openXml", TestRunner.ResultXml);
        }
    }
}