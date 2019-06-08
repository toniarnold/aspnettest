using asp.websharper.spa.Model;
using asp.websharper.spa.Remoting;
using System.Collections.Generic;
using WebSharper;
using WebSharper.UI;
using WebSharper.UI.Client;
using static WebSharper.UI.V;

namespace asp.websharper.spa.Client
{
    /// <summary>
    /// Class with methods for displaying the corresponding sub-pages.
    /// </summary>
    [JavaScript]
    public static class Components
    {
        /// <summary>
        /// Title: The calculator is read only, passing the View suffices.
        /// The document part is visible in all states.
        /// </summary>
        /// <param name="calculator">The calculator View.</param>
        /// <returns></returns>
        public static WebSharper.UI.Doc TitleDoc(View<CalculatorViewModel> calculator)
        {
            var state = V(calculator.V.State);

            return new Template.Title.Main()
                .State(state)
                .Doc();
        }

        /// <summary>
        /// Splash: The calculator is read only, passing the View suffices.
        /// The document part is only visible in state Splash.
        /// </summary>
        /// <returns></returns>
        public static object SplashDoc(View<CalculatorViewModel> calculator) =>
            V(calculator.V.State).Map2(V(calculator.V.Map1.Splash), (state, splash) =>
            {
                return (state == splash) ?
                    new Template.Splash.Main()
                        .Doc()
                    :
                    WebSharper.UI.Doc.Empty;
            });

        /// <summary>
        /// Enter: The calculator is read only, passing the View suffices.
        /// The new operand is mapped to the global varOperand parameter.
        /// </summary>
        /// <param name="calculator">The calculator.</param>
        /// <param name="varOperand">The variable operand.</param>
        /// <returns></returns>
        public static object EnterDoc(View<CalculatorViewModel> calculator,
                                      Var<string> varOperand) =>
            V(calculator.V.State).Map2(V(calculator.V.Map1.Enter), (state, enter) =>
            {
                return
                    (state == enter) ?
                    new Template.Enter.Main()
                        .Operand(varOperand)
                        .Doc()
                    :
                    WebSharper.UI.Doc.Empty;
            });

        /// <summary>
        /// Calculate: Mutates the calculator's persistent Stack, therefore
        /// View and Variable required.
        /// </summary>
        /// <param name="calculator">The calculator.</param>
        /// <param name="varCalculator">The variable calculator.</param>
        /// <returns></returns>
        public static object CalculateDoc(View<CalculatorViewModel> calculator,
                                          Var<CalculatorViewModel> varCalculator) =>
                    V(calculator.V.ViewState).Map(viewState =>
                    V(calculator.V.State).Map2(V(calculator.V.Map1.Calculate), (state, calculate) =>
                    {
                        return
                            (state == calculate) ?
                            new Template.Calculate.Main()
                                .Add(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.Add(viewState);
                                    varCalculator.Set(newCalc);
                                })
                                .Sub(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.Sub(viewState);
                                    varCalculator.Set(newCalc);
                                })
                                .Mul(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.Mul(viewState);
                                    varCalculator.Set(newCalc);
                                })
                                .Div(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.Div(viewState);
                                    varCalculator.Set(newCalc);
                                })
                                .Pow(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.Pow(viewState);
                                    varCalculator.Set(newCalc);
                                })
                                .Sqrt(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.Sqrt(viewState);
                                    varCalculator.Set(newCalc);
                                })
                                .Clr(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.Clr(viewState);
                                    varCalculator.Set(newCalc);
                                })
                                .ClrAll(async (el, ev) =>
                                {
                                    var newCalc = await CalculatorRemoting.ClrAll(viewState);
                                    varCalculator.Set(newCalc);
                                })

                                .StackContainer(
                                    V((IEnumerable<string>)calculator.V.Stack).DocSeqCached((string x) =>
                                        new Template.Index.TestSummaryItem().Line(x).Doc()
                                    )
                                )
                                .Doc()
                            :
                            WebSharper.UI.Doc.Empty;
                    }));

        /// <summary>
        /// Error: The calculator is read only, passing the View suffices.
        /// Visible in 3 distinct states that determine the concrete message to
        /// show.
        /// </summary>
        /// <param name="calculator">The calculator.</param>
        /// <returns></returns>
        public static object ErrorDoc(View<CalculatorViewModel> calculator) =>
                    V(calculator.V.State).Map(state =>
                    V(calculator.V.Map1.ErrorEmpty).Map(empty =>
                    V(calculator.V.Map1.ErrorNumeric).Map(numeric =>
                    V(calculator.V.Map1.ErrorTuple).Map(tuple =>
                    {
                        return
                            (state == empty ||
                             state == numeric ||
                             state == tuple) ?
                            new Template.Error.Main()
                                .Msg(
                                    (state == empty) ? "Need a value on the stack to compute." :
                                    (state == numeric) ? "The input was not numeric." :
                                    (state == tuple) ? "Need two values on the stack to compute." : ""
                                    )
                                    .Doc()
                                :
                                WebSharper.UI.Doc.Empty;
                    }))));

        /// <summary>
        /// Footer: The Enter button mutates the calculator state, therefore
        /// View and Variable required.
        /// </summary>
        /// <param name="calculator">The calculator.</param>
        /// <param name="varCalculator">The variable calculator.</param>
        /// <returns></returns>
        public static object FooterDoc(View<CalculatorViewModel> calculator,
                                       Var<CalculatorViewModel> varCalculator,
                                       Var<string> varOperand) =>
            V(calculator.V.ViewState).Map(viewState =>
            {
                return new Template.Footer.Main()
                    .Enter(async (el, ev) =>
                    {
                        var newCalc = await CalculatorRemoting.Enter(viewState, varOperand.Value);
                        varCalculator.Set(newCalc);
                        varOperand.Set(""); // Reactively clears the text box
                    })
                    .Doc();
            });
    }
}