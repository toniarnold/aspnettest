﻿namespace mandelbrot.frontend

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Html
open System
open System.IO
open Microsoft.Extensions.DependencyInjection
open mandelbrot.image
open iselenium
open WebSharper.UI
open WebSharper.JavaScript
open Microsoft.AspNetCore.Http

module Site =

    let FrontendWebSocket = PathString("/ws-image")

    type Endpoints =
        | Img of guid: Guid
        | [<EndPoint "GET /test">] Test
        | [<EndPoint "GET /testresult">] TestResult

    [<Website>]
    let Main =
        Application.MultiPage(fun (ctx: Context<Endpoints>) endpoint ->
            match endpoint with
            | Endpoints.Img guid ->
                Content.Custom(
                    Status = Http.Status.Ok,
                    Headers =[Http.Header.Custom "Content-Type" "image/png"],
                    WriteBody = fun stream ->
                        let provider = ctx.Environment.["WebSharper.AspNetCore.Services"] :?> IServiceProvider
                        let service = provider.GetService<ImageService>()
                        let image = match service.Get(guid) with
                                    | null -> new Image()   // silently deliver an empty placeholder
                                    | instance -> instance
                        use writer = new BinaryWriter(stream)
                        writer.Write(image.Bytes)
                )
            | Test ->
                let testSummary = new ListModel<string, string>(fun s -> s)
                async {
                    let! result = RemoteTestRunner.Run("mandelbrot.frontend.guitest") |> Async.AwaitTask
                    testSummary.Clear()
                    testSummary.AppendMany(result.Summary)
                    if not result.Passed then
                        JS.Window.Location.Assign("/testresult")
                }
                |> Async.Start
                Content.Page(
                    Title = "Passed",
                    Body = [
                        h1 [] [text "Summary klappt nicht"]
                    ]
                )

            | TestResult ->
                Content.Custom(
                    Status = Http.Status.Ok,
                    Headers =[Http.Header.Custom "Content-Type" "image/png"],
                    WriteBody = fun stream ->
                       use writer = new StreamWriter(stream)
                       writer.Write(TestRunner.ResultXml)
                )
        )