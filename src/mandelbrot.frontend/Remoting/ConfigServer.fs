namespace mandelbrot.frontend

open asplib.Remoting
open WebSharper
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Http
open System

module ConfigServer =

    // Configuration also read by the JS front end
    [<JavaScript>]
    type Configuration(tiles: int64,
                        imageWith: int,
                        imageHeight: int,
                        frontendHost: string
                        ) =
        member this.Tiles = tiles
        member this.ImageWith = imageWith
        member this.ImageHeigth = imageHeight
        member this.FrontendHost = frontendHost
        member this.FrontendWebSocket = this.FrontendHost + "/ws-image" // = Site.FrontendWebSocket

    let GetConfig(config: IConfiguration) =
        Configuration(
            config.GetValue<int64>("Tiles"),
            config.GetValue<string>("Resolution").Split('x').[0] |> int,
            config.GetValue<string>("Resolution").Split('x').[1] |> int,
            config.GetValue<string>("FrontendHost")
        )

    [<Rpc>]
    let GetConfiguration() =
        GetConfig(RemotingContext.Configuration)