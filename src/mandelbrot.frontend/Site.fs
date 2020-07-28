namespace mandelbrot.frontend

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
open Microsoft.Net.Http.Headers

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
                let provider = ctx.Environment.["WebSharper.AspNetCore.Services"] :?> IServiceProvider
                let service = provider.GetService<ImageService>()
                let image = match service.Get(guid) with
                            | null -> new Image()   // silently deliver an empty placeholder
                            | instance -> instance
                let headers =
                    if image.IsReady then
                        [Http.Header.Custom HeaderNames.ContentType "image/png"]
                    else  [ // never cache the empty.png, as it will be overridden by the ready image
                        Http.Header.Custom HeaderNames.CacheControl "no-cache"
                        Http.Header.Custom HeaderNames.Pragma "no-cache"
                        Http.Header.Custom HeaderNames.Expires "-1"
                        Http.Header.Custom HeaderNames.ContentType "image/png"
                    ]
                Content.Custom(
                    Status = Http.Status.Ok,
                    Headers = headers,
                    WriteBody = fun stream ->
                        use writer = new BinaryWriter(stream)
                        writer.Write(image.Bytes)
                )
            | Test ->

                let result = RemoteTestRunner.Run("mandelbrot.frontend.guitest", ctx.RequestUri.Port)
                if not result.Passed then
                    Content.Custom(
                        Status = Http.Status.Ok,
                        Headers =[Http.Header.Custom HeaderNames.ContentType "application/xml; charset=UTF-8"],
                        WriteBody = fun stream ->
                           use writer = new StreamWriter(stream)
                           writer.Write(TestRunner.ResultXml)
                    )
                else
                    let summary = "<b>" + String.Join("<br />", result.Summary) + "</b>"
                    Content.Page(
                        Title = "Passed",
                        Body = [
                            Doc.Verbatim(summary)
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