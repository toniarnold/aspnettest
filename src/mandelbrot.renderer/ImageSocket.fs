namespace mandelbrot.renderer

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System
open System.Net.WebSockets
open System.Runtime.CompilerServices
open System.Threading
open mandelbrot.image
open statemap
open Microsoft.Extensions.Configuration

module EchoSocket =
    let Endpoint = PathString("/ws-echo")

    /// <summary>
    /// Diagnostic Echo smoke test from the EchoApp in the WebSockets
    /// documentation. Returns the raw bytes it received before until
    /// the socket gets closed by the client.
    /// </summary>
    let Echo (ctx: HttpContext, webSocket: WebSocket) =
        task {
            let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4)
            let! firstresult = webSocket.ReceiveAsync(buffer, CancellationToken.None)
            let mutable result = firstresult
            while not result.CloseStatus.HasValue do
                do! webSocket.SendAsync(buffer.Slice(0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None)
                let! newresult = webSocket.ReceiveAsync(buffer, CancellationToken.None) |> Async.AwaitTask
                result <- newresult
            do! webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None)
        }

/// <summary>
/// Handler class with a steady WebSocket member echo back the image progress on
/// ImageStateChanged events
/// </summary>
type ImageSocketHandler(socket: WebSocket,
                        service: ImageService,
                        config: IConfiguration,
                        loggerFactory : ILoggerFactory) =
    let logger = loggerFactory.CreateLogger<ImageSocketHandler>()
    static let endRequestEvent = new AutoResetEvent(false)
    interface IDisposable with
        member this.Dispose() =
            endRequestEvent.Dispose()

    /// <summary>
    /// The SMC StateChangeEventHandler itself
    /// </summary>
    member this.ImageStateChanged(sender: obj , args: StateChangeEventArgs ) =
        let currentImage = (sender :?> ImageContext).Owner
        logger.LogDebug(String.Format("Image {0}/{1}/{2} progress to {3}",
                                currentImage.Coordinates.X,
                                currentImage.Coordinates.Y,
                                currentImage.Coordinates.Z,
                                currentImage.State))
        let currentViewModel = new ImageViewModel(currentImage);
        socket.SendAsync(currentViewModel.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None)
        |> Async.AwaitTask |> Async.RunSynchronously
        if currentViewModel.IsReady then // end of rendering progress echoing
            socket.CloseAsync(WebSocketCloseStatus.NormalClosure , "Image.IsReady", CancellationToken.None)
            |> Async.AwaitTask |> Async.RunSynchronously
            endRequestEvent.Set() |> ignore

    /// <summary>
    /// Gets an Image query in state Parameters from the mandelbrot.frontend.ImageSocket.
    /// Returns the Image on the socket, either ready or in progress.
    /// </summary>
    member this.DoRequest () =
        task {
            let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4)
            let! imageQuery = socket.ReceiveAsync(buffer, CancellationToken.None)
            let queryModel = ImageViewModel.FromArraySegment<ImageViewModel>(buffer.Slice(0, imageQuery.Count))
            logger.LogInformation(
                String.Format("Received Image query with coordinates X={0} Y={1} Z={2}",
                    queryModel.Coordinates.X, queryModel.Coordinates.Y, queryModel.Coordinates.Z))
            let image = service.Get(queryModel.Main.Specification) // implicitly starts rendering
            image.Coordinates <- queryModel.Coordinates // unavailable to Service.Get itself
            if not image.IsReady then // not directly from the cache -> echo back rendering progress
                image.AddStateChangedHandler(fun sender args -> this.ImageStateChanged(sender, args))
            endRequestEvent.WaitOne(TimeSpan.FromSeconds(config.GetValue<float>("ImageSocketTimeout"))) |> ignore
            logger.LogInformation("End of WebSocket request")
        }

type ImageSocketMiddleware(imageEndpoint: PathString, next: RequestDelegate,
                            imageService: ImageService,
                            config: IConfiguration, loggerFactory : ILoggerFactory
                            ) =
    let Logger = loggerFactory.CreateLogger<ImageSocketMiddleware>()

    member this.LogRequest(pathstring: string) =
        Logger.LogInformation(String.Format("Received WebSocket Request to {0}", pathstring))

    member this.Invoke (ctx : HttpContext) =
        task {
            if ctx.WebSockets.IsWebSocketRequest then
                let path = ctx.Request.Path
                match path with
                | _ when path = EchoSocket.Endpoint ->
                    this.LogRequest(path.Value)
                    let! echoScoket = ctx.WebSockets.AcceptWebSocketAsync()
                    do! EchoSocket.Echo(ctx, echoScoket)
                | _ when path = imageEndpoint ->
                    this.LogRequest(path.Value)
                    use! imageSocket = ctx.WebSockets.AcceptWebSocketAsync()
                    use handler = new ImageSocketHandler(imageSocket, imageService, config, loggerFactory)
                    do! handler.DoRequest()
                | _ -> return! next.Invoke ctx
            else
                return! next.Invoke ctx
        }

[<Extension>]
type ImageSocketExtension =
    /// <summary>
    /// Adds the ImageSocket WebSocket at the path /ws-imag
    /// </summary>
    [<Extension>]
    static member inline UseImageSocket(app: IApplicationBuilder, endpoint: PathString) =
        app.UseWebSockets()
            .UseMiddleware<ImageSocketMiddleware>(endpoint)