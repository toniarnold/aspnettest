using Microsoft.AspNetCore.Components;

namespace asplib.Components
{
    public partial class TestButtonBase : ComponentBase
    {
        [Parameter]
        public string src { get; set; } = "/_content/asplib.blazor/nunit.png";

        [Parameter]
        public string TestResult { get; set; } = "";

        public void Test()
        {
            throw new NotImplementedException();
        }
    }
}