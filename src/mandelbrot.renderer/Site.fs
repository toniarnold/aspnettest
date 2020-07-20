namespace mandelbrot.renderer

open WebSharper
open WebSharper.Sitelets
open System
open System.IO
open WebSharper.UI.Html
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open mandelbrot.image

module Site =

    let RendererWebSocket = PathString("/ws-image")

    type Endpoints =
        | [<EndPoint "GET /">] Index
        | Img of guid: Guid

    [<Website>]
    let ImageBinary =
        Application.MultiPage(fun (ctx: Context<Endpoints>) endpoint ->
            match endpoint with
            | Endpoints.Index ->
                Content.Page(
                    Title = "mandelbrot.renderer",
                    Body = [h1 [] [text "Web Service mandelbrot.renderer"]])
            | Endpoints.Img guid ->
                Content.Custom(
                    Status = Http.Status.Ok,
                    Headers =[Http.Header.Custom "Content-Type" "image/png"],
                    WriteBody = fun stream ->
                        let provider = ctx.Environment.["WebSharper.AspNetCore.Services"] :?> ServiceProvider
                        let service = provider.GetService<ImageService>()
                        let image = match service.Get(guid) with
                                    | null -> new Image()   // silently deliver an empty placeholder
                                    | instance -> instance
                        use writer = new BinaryWriter(stream)
                        writer.Write(image.Bytes)
                )
        )