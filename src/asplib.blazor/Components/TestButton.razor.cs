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

        public string TestResult { get; set; } = "";

        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private IWebHostEnvironment Environment { get; set; }

        [Inject]
        private IHttpContextAccessor Http { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        private int Port { get; set; } = 0;

        protected override void OnInitialized()
        {
            Port = (int)Http.HttpContext.Request.Host.Port;
        }

        public async Task Test()
        {
            TestResult = "Running...";
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