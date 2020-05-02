namespace asp.websharper.spa.fs

open asp.websharper.spa.fs.Model
open asp.websharper.spa.fs.Server
open iie
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Templating
open System
open System.Threading.Tasks
type JSConsole = WebSharper.JavaScript.Console

[<JavaScript>]
module IndexDoc =

    type Index  = Template<"wwwroot/index.html">

    let Page (page : Var<string>, viewCalculator : View<CalculatorViewModel>, varCalculator : Var<CalculatorViewModel> ) =
        V(page.V).Map(fun showPage ->
            match showPage with
            | Pagename.Single ->
                varCalculator.Set(CalculatorViewModel())
                CalculatorDoc.MainDoc(viewCalculator, varCalculator, page, "")
            | Pagename.Triptych ->
                TriptychDoc.MainDoc(page)
            | _ -> raise (new Exception(String.Format("Page not defined: {0}", showPage)))
        )

    let MainDoc (viewCalculator : View<CalculatorViewModel>,  varCalculator : Var<CalculatorViewModel>)  =
         let page = Var.Create(Pagename.Single)
         let viewState = Var.Create("")
         let testSummary = new ListModel<string, string>(fun s -> s)
         WebSharper.UI.Doc.ConcatMixed(
            input [ Attr.Concat [
                attr.value viewState.Value
                attr.``type`` "hidden"
            ]] []
            ,
            Index.Main()
                .Test(fun _ ->
                    async {
                        let! result = TestServer.Test("asptest.websharper.spa") |> Async.AwaitTask
                        testSummary.Clear()
                        testSummary.AppendMany(result.Summary)
                        if not result.Passed then
                            JS.Window.Location.Assign(TestResult.Path)
                    }
                    |> Async.Start
                )
                .TestSummaryContainer()
                .Doc()
            ,
            Page(page, viewCalculator, varCalculator)
            )

    [<SPAEntryPoint>]
    let ClientMain () =
        let varCalculator = Var.Create(CalculatorViewModel())
        let viewCalculator = varCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(fun c ->
            if c.IsNew
                then
                    JSConsole.Log("c.IsNew")
                    CalculatorServer.LoadNew()
                else
                    JSConsole.Log("Task.FromResult(c)")
                    Task.FromResult(c)
        )

        MainDoc(viewCalculator, varCalculator).RunById "main"