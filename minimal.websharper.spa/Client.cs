using System.Threading.Tasks;
using WebSharper;
using WebSharper.UI;
using WebSharper.UI.Client;
using static WebSharper.UI.Client.Html;

namespace minimal.websharper.spa
{
    [JavaScript]
    public class App
    {
        static Var<string> testResult = Var.Create("");

        [SPAEntryPoint]
        public static void ClientMain()
        {
            new Template.Index.Main()
                .Test(async (el, ev) =>
                {
                    var result = await Remoting.Test();
                    testResult.Value = result.PassedString;
                    //if (!result.Passed)
                })
                .TestResult(testResult.View)
                .Doc()
                .RunById("main");
        }
    }
}
