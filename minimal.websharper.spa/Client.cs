using iie;
using System.Diagnostics;
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
            IndexDoc().RunById("content");
        }

 
        public static WebSharper.UI.Doc IndexDoc()
        {
            var testSummary = new ListModel<string, string>(s => s);

            return new Template.Index.Main()

                // Links to sub-pages
                .WithStatic((el, ev) =>
                {
                    WithStaticDoc().RunById("content");
                })
                .WithStorage((el, ev) =>
                {
                    WithStorageDoc().RunById("content");
                })

                // --- Begin test button ---
                .Test(async (el, ev) =>
                {
                    var result = await TestServer.Test("minimaltest.websharper.spa");
                    testSummary.Clear();
                    testSummary.AppendMany(result.Summary);
                    if (!result.Passed)
                    {
                        JS.Window.Location.Assign(TestResult.Path);
                    }
                })
                .TestSummaryContainer(
                    testSummary.View.DocSeqCached((string x) =>
                        new Template.Index.TestSummaryItem().Line(x).Doc()
                    )
                )
                // --- End test button ---

                .Doc();
        }



        public static WebSharper.UI.Doc WithStaticDoc()
        {
            return new Template.Withstatic()
                .Back((el, ev) =>
                {
                    IndexDoc().RunById("content");
                })
                .Doc();
        }


        public static WebSharper.UI.Doc WithStorageDoc()
        {
            return new Template.Withstorage()
                .Back((el, ev) =>
                {
                    IndexDoc().RunById("content");
                })
                .Doc();
        }
    }
}
