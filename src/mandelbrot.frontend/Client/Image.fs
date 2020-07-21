namespace mandelbrot.frontend

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open mandelbrot.image

[<JavaScript>]
module Image =

    let Main(config: ConfigServer.Configuration, coordinates: Coordinates) =
        let resolution = Resolution(config.ImageWith, config.ImageHeigth)
        let varImage = Var.Create(new ImageViewModel(coordinates, resolution))
        let varImageSrc = Var.Create(varImage.Value.ImgSrc())
        let varStateDisplay = Var.Create("display: inline;")

        let HandleMessage (socket: WebSocket) (msg: MessageEvent) =
            let image = ImageViewModel.FromJson(string msg.Data)
            varImage.Set(image)
            Console.Log(varImage.Value.State)
            Console.Log(varImage.Value.IsReady)
            Console.Dir(varImage.Value)
            if image.IsReady then
                socket.Close()
                varImageSrc.Set(varImage.Value.ImgSrc()) // explicitly set to enforce reload
                varStateDisplay.Set("display: none;")

        let socketAddr = config.FrontendWebSocket
        Console.Log("Connecting to WebSocket " + socketAddr)
        let socket = WebSocket(socketAddr)
        socket.Onmessage <- fun event -> // rendering progress
            HandleMessage socket event
        socket.Onopen <- fun () ->
            socket.Send(varImage.Value.JSToJson()) // query this server for the image

        div [] [
            img [ Attr.Concat [
                attr.``class`` "image"
                attr.width config.sImageWith
                attr.height config.sImageHeigth
                attr.srcDyn (V(varImageSrc.V))
                ]
            ] []
            p [ Attr.Concat [
                attr.``class`` "state"
                attr.styleDyn (V(varStateDisplay.V))
                ]
            ] [ textView (V("State = " + varImage.V.State)) ]
            ul [Attr.Concat [
                attr.``class`` "params"
                attr.styleDyn (V(varStateDisplay.V))
                ]
            ] [
                li [] [ textView (V("Magnification = " + varImage.V.Magnification)) ]
                li [] [ textView (V("Location.Real = " + varImage.V.Real)) ]
                li [] [ textView (V("Location.Imag = " + varImage.V.Imag)) ]
                li [] [ textView (V("EscapeRadius = " + varImage.V.EscapeRadius)) ]
                li [] [ textView (V("MaxIterations = " + varImage.V.MaxIterations)) ]
            ]
        ]