namespace mandelbrot.frontend

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Templating
open mandelbrot.image

[<JavaScript>]
module Main =

    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let HaveImage coordinates =
        let id = ImageViewModel.Id coordinates
        JS.Document.GetElementById(id) <> null

    let Render (config: ConfigServer.Configuration) (xOffset: int64) (yOffset: int64) (z: int64) =
        async {
            for x = 1L to config.Tiles do
                for y = 1L to config.Tiles do
                    let coordinates = Coordinates(x  - xOffset, y - yOffset, z)
                    if not (HaveImage coordinates) then
                        let img = Image.Main(config, coordinates, x, y)
                        img.RunAppend("grid")
        } |> Async.Start

    [<SPAEntryPoint>]
    let Main () =
        let config = ConfigServer.GetConfiguration()
        let initialOffset = int64 (Math.Ceil(float config.Tiles / 2.0))
        let varXOffset = Var.Create(initialOffset)
        let varYOffset = Var.Create(initialOffset)
        let varZ = Var.Create(2L)
        IndexTemplate
            .Main()
            .Tiles(string config.Tiles)
            .ImageWidth(string config.ImageWith)
            .ImageHeight(string config.ImageHeigth)
            .Doc()
        |> Doc.RunById "main"

        // Mouse Wheel Zoom
        let grid = JS.Document.GetElementById("grid")
        grid.AddEventListener("wheel", fun (evt: Dom.Event) ->
            let evt = evt :?> Dom.WheelEvent
            let delta = - int64(evt.DeltaY / 100)
            Console.Log("Wheel event zoom delta: " + string delta)
            varZ.Set(varZ.Value + delta)
            Render config varXOffset.Value varYOffset.Value varZ.Value
        )

        // Reactively render after anything is set up
        Render config varXOffset.Value varYOffset.Value varZ.Value