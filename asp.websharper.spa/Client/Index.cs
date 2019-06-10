using asp.websharper.spa.Model;
using asp.websharper.spa.Remoting;
using iie;
using System.Threading.Tasks;
using WebSharper;
using WebSharper.JavaScript;
using WebSharper.UI;
using WebSharper.UI.Client;
using static asp.websharper.spa.Client.Components;
using static WebSharper.UI.Client.Html;

namespace asp.websharper.spa.Client
{
    [JavaScript]
    public class Index
    {
        [SPAEntryPoint]
        public static void ClientMain()
        {
            var varCalculator = Var.Create(new CalculatorViewModel());
            varCalculator.Set(null);    // throw away the dummy instance
            var viewCalculator = varCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(c =>
            {
                // Explicit null-check required by WebSharper anyway:
                if (c == null)
                {
                    // Get a new instance from the server
                    return CalculatorServer.Load();
                }
                else
                {
                    // Return the existing instance
                    return Task.FromResult(c);
                }
            });

            IndexDoc(viewCalculator, varCalculator).RunById("main");
        }

        public static WebSharper.UI.Doc IndexDoc(View<CalculatorViewModel> viewCalculator,
                                                 Var<CalculatorViewModel> varCalculator)
        {
            var varOperand = Var.Create("");

            // Setup the reactive ViewState storage and the test button.
            var viewState = Var.Create("");
            var testSummary = new ListModel<string, string>(s => s);

            return WebSharper.UI.Doc.ConcatMixed(
                input(viewState, attr.type("hidden"))
                ,
                new Template.Index.Main()
                    // Test button with result summary
                    .Test(async (el, ev) =>
                    {
                        var result = await TestServer.Test("asptest.websharper.spa");
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
                    .Doc()
                ,

                // The state-dependent parts which shape the application page
                TitleDoc(viewCalculator),
                SplashDoc(viewCalculator),
                EnterDoc(viewCalculator, varOperand),
                CalculateDoc(viewCalculator, varCalculator),
                ErrorDoc(viewCalculator),
                FooterDoc(viewCalculator, varCalculator, varOperand)
                );
        }
    }
}