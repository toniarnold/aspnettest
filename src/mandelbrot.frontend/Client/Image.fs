namespace mandelbrot.frontend

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open mandelbrot.image

[<JavaScript>]
module Image =

    let Main(config: ConfigServer.Configuration, coordinates: Coordinates, row: int64, column: int64) =
        let resolution = Resolution(config.ImageWith, config.ImageHeigth)
        let varImage = Var.Create(new ImageViewModel(coordinates, resolution))
        let varImageSrc = Var.Create(varImage.Value.ImgSrc())
        let varImageStyle = Var.Create("image-rendering: pixelated; opacity: 0.3;") // empty.pnng
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
                varImageStyle.Set(""); // remove opacity and pixelated

        let socketAddr = config.FrontendWebSocket
        Console.Log("Connecting to WebSocket " + socketAddr)
        let socket = WebSocket(socketAddr)
        socket.Onmessage <- fun event -> // rendering progress
            HandleMessage socket event
        socket.Onopen <- fun () ->
            socket.Send(varImage.Value.JSToJson()) // query this server for the image

        div [ Attr.Concat [
                attr.``class`` "gridcell"
                attr.idDyn (V(varImage.Value.Id()))
                attr.styleDyn (V("grid-row: " + string row + "; grid-column: " + string column + ";"))
                ]
            ] [
            img [ Attr.Concat [
                attr.``class`` "image"
                attr.width (string config.ImageWith)
                attr.height (string config.ImageHeigth)
                attr.srcDyn (V(varImageSrc.V))
                attr.styleDyn (V(varImageStyle.V))
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