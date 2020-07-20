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

    let Cell config =
        IndexTemplate.CellTemplate()
            .Id("cell-id")
            .Image(Image.Main(config, Coordinates()))
            .Doc()

    let Row =
        IndexTemplate.RowTemplate()
            .Id("row-id")
            .Doc()

    let Grid config =
        IndexTemplate.GridTemplate()
            .Row(
                Cell config
            )
            .Doc()

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate
            .Main()
            .Grid(
                Grid(ConfigServer.GetConfiguration())
            )
            .Doc()
        |> Doc.RunById "main"