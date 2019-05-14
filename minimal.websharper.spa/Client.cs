using iie;
using System.Threading.Tasks;
using WebSharper;
using WebSharper.JavaScript;
using WebSharper.UI;
using WebSharper.UI.Client;
using static WebSharper.UI.Client.Html;

namespace minimal.websharper.spa
{
    [JavaScript]
    public class App
    {
        [SPAEntryPoint]
        public static void ClientMain()
        {
            var summary = new ListModel<string, string>(s => s);

            new Template.Index.Main()
                .Test(async (el, ev) =>
                {
                    var result = await TestServer.Test("minimaltest.websharper.spa");
                    summary.AppendMany(result.Summary);
                    if (!result.Passed)
                    {
                        JS.Window.Location.Assign(TestResult.Path);
                    }
                })
                .TestSummaryContainer(
                    summary.View.DocSeqCached((string x) =>
                        new Template.Index.TestSummaryItem().Line(x).Doc()
                    )
                )
                .Doc()
                .RunById("main");
        }
    }
}
