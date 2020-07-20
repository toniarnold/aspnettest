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

        let HandleMessage (socket: WebSocket) (msg: MessageEvent) =
            let image = ImageViewModel.FromJson(string msg.Data)
            varImage.Set(image)
            Console.Dir(varImage.Value)
            if image.IsReady then
                socket.Close()

        let socketAddr = config.FrontendWebSocket
        Console.Log("Connecting to WebSocket " + socketAddr)
        let socket = WebSocket(socketAddr)
        socket.Onmessage <- fun event -> // rendering progress
            HandleMessage socket event
        socket.Onopen <- fun () ->
            socket.Send(varImage.Value.ToJson()) // query this server for the image

        div [] [
            img [ Attr.Concat [
                attr.``class`` "image"
                attr.width config.sImageWith
                attr.height config.sImageHeigth
                attr.srcDyn (V(varImage.V.ImgSrc))
                ]
            ] []
            p [ attr.``class`` "state" ] [ textView (V("State = " + varImage.V.State)) ]
            ul [attr.``class`` "params"] [
                li [] [ textView (V("Magnification = " + string varImage.V.Params.Magnification)) ]
                li [] [ textView (V("Location.Real = " + string varImage.V.Params.Location.Real)) ]
                li [] [ textView (V("Location.Imag = " + string varImage.V.Params.Location.Imag)) ]
                li [] [ textView (V("EscapeRadius = " + string varImage.V.Params.EscapeRadius)) ]
                li [] [ textView (V("MaxIterations = " + string varImage.V.Params.MaxIterations)) ]
            ]
        ]