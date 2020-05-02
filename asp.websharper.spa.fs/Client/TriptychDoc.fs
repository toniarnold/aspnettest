﻿namespace asp.websharper.spa.fs

open asp.websharper.spa.fs.Model
open asp.websharper.spa.fs.Server
open WebSharper
open WebSharper.UI
open WebSharper.UI.Templating
open System.Threading.Tasks

[<JavaScript>]
module TriptychDoc =

    type Triptych  = Template<"wwwroot/triptych.html">

    [<Literal>]
    let ViewStateDoc = "ViewStateDoc"
    [<Literal>]
    let SessionDoc = "SessionDoc"
    [<Literal>]
    let DatabaseDoc = "DatabaseDoc"

    let MainDoc (page : Var<string> ) =
        let varViewStateCalculator = Var.Create<CalculatorViewModel>(new CalculatorViewModel())
        let viewViewStateCalculator = varViewStateCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(fun c ->
            if c.State = ""
                then CalculatorServer.LoadNew()
                else Task.FromResult(c)
        )

        let varSessionCalculator = Var.Create<CalculatorViewModel>(new CalculatorViewModel())
        let viewSessionCalculator = varSessionCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(fun c ->
            if c.State = ""
                then CalculatorServer.LoadNew()
                else Task.FromResult(c)
        )

        let varDatabaseCalculator = Var.Create<CalculatorViewModel>(new CalculatorViewModel())
        let viewDatabaseCalculator = varDatabaseCalculator.View.MapAsync<CalculatorViewModel, CalculatorViewModel>(fun c ->
             if c.State = ""
                 then CalculatorServer.LoadNew()
                 else Task.FromResult(c)
        )

        Triptych.Main()
            .ViewState(CalculatorDoc.MainDoc(viewViewStateCalculator, varViewStateCalculator, page, ViewStateDoc))
            .Session(CalculatorDoc.MainDoc(viewSessionCalculator, varSessionCalculator, page, SessionDoc))
            .Database(CalculatorDoc.MainDoc(viewDatabaseCalculator, varDatabaseCalculator, page, DatabaseDoc))
            .Doc()