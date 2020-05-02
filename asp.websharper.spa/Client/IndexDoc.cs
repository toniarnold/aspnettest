using asp.websharper.spa.Model;
using asp.websharper.spa.Remoting;
using iie;
using System;
using System.Threading.Tasks;
using WebSharper;
using WebSharper.JavaScript;
using WebSharper.UI;
using WebSharper.UI.Client;
using static WebSharper.UI.Client.Html;
using static WebSharper.UI.V;
using JSConsole = WebSharper.JavaScript.Console;

namespace asp.websharper.spa.Client
{
    [JavaScript]
    public class IndexDoc
    {
        [SPAEntryPoint]
        public static void ClientMain()
        {
            var varCalculator = Var.Create(new CalculatorViewModel());
            var viewCalculator = varCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(c =>
            {
                // Explicit null-check required by WebSharper only in C# (in F#, it is initialized):
                if (c == null)
                {
                    JSConsole.Log("c == null");
                    return CalculatorServer.Load(); // Get a new instance from the server
                }
                else
                {
                    JSConsole.Log("Task.FromResult(c)");
                    return Task.FromResult(c);      // Return the persistent instance
                }
            });

            MainDoc(viewCalculator, varCalculator).RunById("main");
        }

        public static WebSharper.UI.Doc MainDoc(View<CalculatorViewModel> viewCalculator,
                                              Var<CalculatorViewModel> varCalculator)
        {
            // Page visibility without state machine
            var page = Var.Create(Single);

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

                Page(page, viewCalculator, varCalculator)
                );
        }

        // Sub-Pages as string, the JS compiler complains with an enum in conditionals
        public const string Single = "Single";

        public const string Triptych = "Triptych";

        public static object Page(Var<string> page,
                                View<CalculatorViewModel> viewCalculator,
                                Var<CalculatorViewModel> varCalculator) =>
        V(page.V).Map(showPage =>
        {
            switch (showPage)
            {
                case Single:
                    varCalculator.Set(null);    // enforce reload after storage change
                    return CalculatorDoc.MainDoc(viewCalculator, varCalculator, page);

                case Triptych:
                    return TriptychDoc.MainDoc(page);

                default:
                    throw new Exception(string.Format(  // JS: no NotImplementedException
                        "Page {0}", showPage));
            }
        });
    }
}