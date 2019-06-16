using asp.websharper.spa.Model;
using asp.websharper.spa.Remoting;
using asplib.Remoting;
using System;
using System.Collections.Generic;
using WebSharper;
using WebSharper.UI;
using WebSharper.UI.Client;
using static asplib.View.TagHelper;
using static WebSharper.UI.V;

namespace asp.websharper.spa.Client
{
    /// <summary>
    /// Class with methods for displaying the corresponding sub-pages.
    /// </summary>
    [JavaScript]
    public static class CalculatorDoc
    {
        public static WebSharper.UI.Doc MainDoc(View<CalculatorViewModel> viewCalculator,
                                            Var<CalculatorViewModel> varCalculator,
                                            Var<string> page,
                                            string id = null)
        {
            var varOperand = Var.Create("");

            return WebSharper.UI.Doc.ConcatMixed(
                // The state-dependent parts which shape the calculator sub-application
                HeaderDoc(viewCalculator, varCalculator, page),
                TitleDoc(viewCalculator),
                SplashDoc(viewCalculator),
                EnterDoc(viewCalculator, varOperand, id),
                CalculateDoc(viewCalculator, varCalculator, id),
                ErrorDoc(viewCalculator),
                FooterDoc(viewCalculator, varCalculator, varOperand, id)
                );
        }

        public static object HeaderDoc(View<CalculatorViewModel> viewCalculator,
                                            Var<CalculatorViewModel> varCalculator,
                                            Var<string> page) =>
            V(viewCalculator.V.SessionStorage).Map(thisStorage =>
            {
                return new Template.Header.Main()
                    .Storage(V(viewCalculator.V.VSessionStorage))
                    .StorageLink(async (el, ev) =>
                    {
                        // WebSharper compiler doesn't accept switch() here
                        if (page.Value == IndexDoc.Single)
                        {
                            // Single calculator -> Switch to the triptych page
                            page.Set(IndexDoc.Triptych);
                        }
                        else if (page.Value == IndexDoc.Triptych)
                        {
                            // Change storage globally
                            await StorageServer.SetStorage(thisStorage);

                            // Switch back to the single calculator page
                            page.Set(IndexDoc.Single);
                        }
                        else
                        {
                            throw new Exception(string.Format(  // JS: no NotImplementedException
                                "Page {0}", page.Value));
                        }
                    })
                    .Doc();
            });

        /// <summary>
        /// Title: The calculator is read only, passing the View suffices.
        /// The document part is visible in all states.
        /// </summary>
        /// <param name="viewCalculator">The calculator View.</param>
        /// <returns></returns>
        public static WebSharper.UI.Doc TitleDoc(View<CalculatorViewModel> viewCalculator)
        {
            return new Template.Title.Main()
                .State(V(viewCalculator.V.State))
                .Doc();
        }

        /// <summary>
        /// Splash: The calculator is read only, passing the View suffices.
        /// The document part is only visible in state Splash.
        /// </summary>
        /// <returns></returns>
        public static object SplashDoc(View<CalculatorViewModel> viewCalculator) =>
            V(viewCalculator.V.State).Map2(V(viewCalculator.V.Map1.Splash), (state, splash) =>
            {
                return (state == splash) ?
                    new Template.Splash.Main()
                        .Doc()
                    : WebSharper.UI.Doc.Empty;
            });

        /// <summary>
        /// Enter: The calculator is read only, passing the View suffices.
        /// The new operand is mapped to the global varOperand parameter.
        /// </summary>
        /// <param name="viewCalculator">The calculator.</param>
        /// <param name="varOperand">The variable operand.</param>
        /// <returns></returns>
        public static object EnterDoc(View<CalculatorViewModel> viewCalculator,
                                      Var<string> varOperand,
                                      string idParent) =>
            V(viewCalculator.V.State).Map2(V(viewCalculator.V.Map1.Enter), (state, enter) =>
            {
                return (state == enter) ?
                    new Template.Enter.Main()
                        .IdOperandTextbox(Id(idParent, OperandTextbox))
                        .Operand(varOperand)
                        .Doc()
                    : WebSharper.UI.Doc.Empty;
            });

        public const string OperandTextbox = "OperandTextbox";

        /// <summary>
        /// Calculate: Mutates the calculator's persistent Stack, therefore
        /// View and Variable required.
        /// </summary>
        /// <param name="viewCalculator">The calculator.</param>
        /// <param name="varCalculator">The variable calculator.</param>
        /// <returns></returns>
        public static object CalculateDoc(View<CalculatorViewModel> viewCalculator,
                                          Var<CalculatorViewModel> varCalculator,
                                          string idParent) =>
            V(viewCalculator.V.ViewState).Map(viewState =>
            V(viewCalculator.V.State).Map2(V(viewCalculator.V.Map1.Calculate), (state, calculate) =>
            {
                return (state == calculate) ?
                    new Template.Calculate.Main()
                        .IdAddButton(Id(idParent, AddButton))
                        .IdSubButton(Id(idParent, SubButton))
                        .IdMulButton(Id(idParent, MulButton))
                        .IdDivButton(Id(idParent, DivButton))
                        .IdPowButton(Id(idParent, PowButton))
                        .IdSqrtButton(Id(idParent, SqrtButton))
                        .IdClrButton(Id(idParent, ClrButton))
                        .IdClrAllButton(Id(idParent, ClrAllButton))
                        .Add(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.Add(viewState));
                        })
                        .Sub(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.Sub(viewState));
                        })
                        .Mul(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.Mul(viewState));
                        })
                        .Div(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.Div(viewState));
                        })
                        .Pow(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.Pow(viewState));
                        })
                        .Sqrt(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.Sqrt(viewState));
                        })
                        .Clr(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.Clr(viewState));
                        })
                        .ClrAll(async (el, ev) =>
                        {
                            varCalculator.Set(
                                await CalculatorServer.ClrAll(viewState));
                        })

                        .StackContainer(
                            V((IEnumerable<string>)viewCalculator.V.Stack).DocSeqCached((string x) =>
                                new Template.Index.TestSummaryItem().Line(x).Doc()
                            )
                        )
                        .Doc()
                    : WebSharper.UI.Doc.Empty;
            }));

        public const string AddButton = "AddButton";
        public const string SubButton = "SubButton";
        public const string MulButton = "MulButton";
        public const string DivButton = "DivButton";
        public const string PowButton = "PowButton";
        public const string SqrtButton = "SqrtButton";
        public const string ClrButton = "ClrButton";
        public const string ClrAllButton = "ClrAllButton";

        /// <summary>
        /// Error: The calculator is read only, passing the View suffices.
        /// Visible in 3 distinct states that determine the concrete message to
        /// show.
        /// </summary>
        /// <param name="viewCalculator">The calculator.</param>
        /// <returns></returns>
        public static object ErrorDoc(View<CalculatorViewModel> viewCalculator) =>
            V(viewCalculator.V.State).Map(state =>
            V(viewCalculator.V.Map1.ErrorEmpty).Map(empty =>
            V(viewCalculator.V.Map1.ErrorNumeric).Map(numeric =>
            V(viewCalculator.V.Map1.ErrorTuple).Map(tuple =>
            {
                return (
                    state == empty ||
                    state == numeric ||
                    state == tuple
                    ) ?
                    new Template.Error.Main()
                        .Msg(
                            (state == empty) ? "Need a value on the stack to compute." :
                            (state == numeric) ? "The input was not numeric." :
                            (state == tuple) ? "Need two values on the stack to compute." :
                            "")
                            .Doc()
                    : WebSharper.UI.Doc.Empty;
            }))));

        /// <summary>
        /// Footer: The Enter button mutates the calculator state, therefore
        /// View and Variable required.
        /// </summary>
        /// <param name="viewCalculator">The calculator.</param>
        /// <param name="varCalculator">The variable calculator.</param>
        /// <returns></returns>
        public static object FooterDoc(View<CalculatorViewModel> viewCalculator,
                                       Var<CalculatorViewModel> varCalculator,
                                       Var<string> varOperand,
                                       string idParent) =>
            V(viewCalculator.V.ViewState).Map(viewState =>
            {
                return new Template.Footer.Main()
                    .IdEnterButton(Id(idParent, EnterButton))
                    .Enter(async (el, ev) =>
                    {
                        varCalculator.Set(
                            await CalculatorServer.Enter(viewState, varOperand.Value));
                        varOperand.Set(""); // Reactively clears the text box
                    })
                    .Doc();
            });

        public const string EnterButton = "EnterButton";
    }
}