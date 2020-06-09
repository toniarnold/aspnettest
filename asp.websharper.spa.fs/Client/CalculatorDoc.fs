namespace asp.websharper.spa.fs

open asp.websharper.spa.fs.Model
open asp.websharper.spa.fs.Server
open asplib.Remoting
open asplib.View
open WebSharper
open WebSharper.UI
open WebSharper.UI.Templating
open WebSharper.UI.Html
open System
open System.Collections.Generic

[<JavaScript>]
module CalculatorDoc =

    type Header = Template<"wwwroot/calculator/header.html">
    type Title = Template<"wwwroot/calculator/title.html">
    type Splash = Template<"wwwroot/calculator/splash.html">
    type Enter = Template<"wwwroot/calculator/enter.html">
    type Calculate = Template<"wwwroot/calculator/calculate.html">
    type Error = Template<"wwwroot/calculator/error.html">
    type Footer = Template<"wwwroot/calculator/footer.html">

    [<Literal>]
    let StorageLink = "StorageLink"
    [<Literal>]
    let OperandTextbox = "OperandTextbox"
    [<Literal>]
    let AddButton = "AddButton"
    [<Literal>]
    let SubButton = "SubButton"
    [<Literal>]
    let MulButton = "MulButton"
    [<Literal>]
    let DivButton = "DivButton"
    [<Literal>]
    let PowButton = "PowButton"
    [<Literal>]
    let SqrtButton = "SqrtButton"
    [<Literal>]
    let ClrButton = "ClrButton"
    [<Literal>]
    let ClrAllButton = "ClrAllButton"
    [<Literal>]
    let EnterButton = "EnterButton";

    let HeaderDoc (viewCalculator : View<CalculatorViewModel>,
                    page : Var<string>, idParent: string ) =
        V(viewCalculator.V.SessionStorage).Map(fun thisStorage ->
            Header.Main()
                .IdStorageLink(TagHelper.Id(idParent, StorageLink))
                .Storage(V(viewCalculator.V.VSessionStorage))
                .StorageLink(fun _ ->
                    async {
                        match page.Value with
                        | Pagename.Single -> page.Set(Pagename.Triptych)
                        | Pagename.Triptych ->
                            StorageServer.SetStorage(thisStorage) |> Async.AwaitTask |> ignore
                            page.Set(Pagename.Single)
                        | _ -> raise (new Exception(String.Format("Page {0}", page.Value)))
                    } |> Async.Start
                )
                .Doc()
        )

    let TitleDoc (viewCalculator : View<CalculatorViewModel>) =
        Title.Main()
            .State(V(viewCalculator.V.State))
            .Doc()

    let SplashDoc (viewCalculator : View<CalculatorViewModel>) =
        V(viewCalculator.V.State).Map2(V(viewCalculator.V.Map1.Splash), fun state splash ->
            if state = splash
                then Splash.Main().Doc()
                else WebSharper.UI.Doc.Empty
        )

    let EnterDoc (viewCalculator : View<CalculatorViewModel>,
                    varOperand : Var<string>,
                    idParent: string ) =
        V(viewCalculator.V.State).Map2(V(viewCalculator.V.Map1.Enter), fun state enter ->
            if state = enter
                then Enter.Main()
                        .IdOperandTextbox(TagHelper.Id(idParent, OperandTextbox) : string)
                        .Operand(varOperand)
                        .Doc()
                else WebSharper.UI.Doc.Empty
        )

    let CalculateDoc(viewCalculator : View<CalculatorViewModel>,
                        varCalculator : Var<CalculatorViewModel>,
                        idParent: string ) =
            V(viewCalculator.V.ViewState).Map(fun viewState ->
            V(viewCalculator.V.State).Map2(V(viewCalculator.V.Map1.Calculate), fun state calculate  ->
                if state = calculate
                    then Calculate.Main()
                            .IdAddButton(TagHelper.Id(idParent, AddButton) : string)
                            .IdSubButton(TagHelper.Id(idParent, SubButton) : string)
                            .IdMulButton(TagHelper.Id(idParent, MulButton) : string)
                            .IdDivButton(TagHelper.Id(idParent, DivButton) : string)
                            .IdPowButton(TagHelper.Id(idParent, PowButton) : string)
                            .IdSqrtButton(TagHelper.Id(idParent, SqrtButton) : string)
                            .IdClrButton(TagHelper.Id(idParent, ClrButton) : string)
                            .IdClrAllButton(TagHelper.Id(idParent, ClrAllButton) : string)
                            .Add(fun _ ->
                                async {
                                    let! model = CalculatorServer.Add(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )
                            .Sub(fun _ ->
                                async {
                                    let! model = CalculatorServer.Mul(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )
                            .Mul(fun _ ->
                                async {
                                    let! model = CalculatorServer.Mul(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )
                            .Div(fun _ ->
                                async {
                                    let! model = CalculatorServer.Div(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )
                            .Pow(fun _ ->
                                async {
                                    let! model = CalculatorServer.Pow(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )
                            .Sqrt(fun _ ->
                                async {
                                    let! model = CalculatorServer.Sqrt(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )
                            .Clr(fun _ ->
                                async {
                                    let! model = CalculatorServer.Clr(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )
                            .ClrAll(fun _ ->
                                async {
                                    let! model = CalculatorServer.ClrAll(viewState) |> Async.AwaitTask
                                    varCalculator.Set(model)
                                } |> Async.Start
                            )

                            .StackContainer(
                                V(viewCalculator.V.Stack :> IEnumerable<string>).DocSeqCached(fun (x : string) ->
                                     Calculate.StackItem().Line(x).Doc()
                                )
                             )
                            .Doc()
                    else WebSharper.UI.Doc.Empty
            ))

    let ErrorDoc (viewCalculator : View<CalculatorViewModel>) =
        V(viewCalculator.V.State).Map(fun state ->
        V(viewCalculator.V.Map1.ErrorEmpty).Map(fun empty ->
        V(viewCalculator.V.Map1.ErrorNumeric).Map(fun numeric ->
        V(viewCalculator.V.Map1.ErrorTuple).Map(fun tuple ->
            if state = empty ||
                state = numeric ||
                state = tuple
                then Error.Main()
                        .Msg(
                        if state = empty then "Need a value on the stack to compute."
                        elif state = numeric then "The input was not numeric."
                        elif state = tuple then  "Need two values on the stack to compute."
                        else ""
                        : string)
                        .Doc()
                else WebSharper.UI.Doc.Empty
        ))))

    let FooterDoc (viewCalculator : View<CalculatorViewModel>,
                    varCalculator : Var<CalculatorViewModel>,
                    varOperand : Var<string>,
                    idParent : string) =
        V(viewCalculator.V.ViewState).Map(fun viewState ->
            Footer.Main()
                .IdEnterButton(TagHelper.Id(idParent, EnterButton))
                .Enter(fun _ ->
                    async {
                        let! model = CalculatorServer.Enter (viewState, varOperand.Value) |> Async.AwaitTask
                        varCalculator.Set(model)
                        varOperand.Set("") // Reactively clears the text box
                    } |> Async.Start
                )
                .Doc()
        )

    let MainDoc (viewCalculator : View<CalculatorViewModel>,
                    varCalculator : Var<CalculatorViewModel>,
                    page : Var<string>,
                    id : string) =
        let varOperand = Var.Create("")
        WebSharper.UI.Doc.ConcatMixed(
            HeaderDoc(viewCalculator, page, id),
            TitleDoc(viewCalculator),
            SplashDoc(viewCalculator),
            EnterDoc(viewCalculator, varOperand, id),
            CalculateDoc(viewCalculator, varCalculator, id),
            ErrorDoc(viewCalculator),
            FooterDoc(viewCalculator, varCalculator, varOperand, id)
        )