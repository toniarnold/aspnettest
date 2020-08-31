namespace mandelbrot.frontend

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
open asplib.Services
open System.Net.Http

/// <summary>
/// Endpoints paths from mandelbrot.renderer
/// </summary>
module RemoteEndpoints =
    let RemdererWebSocket = mandelbrot.renderer.Site.RendererWebSocket
    let Image = "/Img/{0}"

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
/// Handler class with a steady WebSocket member to echo back the image progress
/// (on remote ImageStateChanged events on the renderer service server) to the
/// JS front end.
/// </summary>
type ImageSocketHandler(frontendSocket: WebSocket,
                        imageService: ImageService,
                        rendererClient: IWebSocketClient,
                        rendererImageClientFactory: IHttpClientFactory,
                        config: IConfiguration,
                        loggerFactory : ILoggerFactory) =
    let logger = loggerFactory.CreateLogger<ImageSocketHandler>()

    /// <summary>
    /// Fetch the binary image from the remote server to cache it locally
    /// </summary>
    member this.GetImage(guid: Guid) =
        task {
            let uri = (new UriBuilder(config.GetValue<string>("RendererHost"),
                                        Path = String.Format(RemoteEndpoints.Image, guid))).Uri
            use client = rendererImageClientFactory.CreateClient()
            return! client.GetByteArrayAsync(uri)
        }

    /// <summary>
    /// Asynchronously wait for the image progress on the rendererSocket. When a
    /// new image state arrives, update the cache and echo it back to the JS
    /// front end. Returns after the rendererSocket was closed on the remote end.
    /// </summary>
    member this.EchoProgress(buffer: ArraySegment<byte>, frontendSocket: WebSocket, rendererSocket: WebSocket) =
        task {
            let! rendererResult = rendererSocket.ReceiveAsync(buffer, CancellationToken.None)
            let nextImage = ImageViewModel.FromArraySegment<ImageViewModel>(buffer.Slice(0, rendererResult.Count)).Main
            logger.LogDebug(String.Format("Received progress State={0}", nextImage.State))
            if nextImage.IsReady then
                let! bytes = this.GetImage(nextImage.Guid) // from the remote cache
                nextImage.Bytes <- bytes
            imageService.ReplaceInCache(nextImage)
            if not nextImage.IsReady then // continue forwarding progress until ready
                do! frontendSocket.SendAsync(buffer.Slice(0, rendererResult.Count), WebSocketMessageType.Text, true, CancellationToken.None)
                logger.LogDebug(String.Format("Forwarded {0}/{1}/{2} progress to {3}",
                                                nextImage.Coordinates.X,
                                                nextImage.Coordinates.Y,
                                                nextImage.Coordinates.Z,
                                                nextImage.State))
                do! this.EchoProgress(buffer, frontendSocket, rendererSocket)
        }

    /// <summary>
    /// Gets an Image query with the Coordinates from the mandelbrot.frontend JS
    /// and forwards the query to the mandelbrot.renderer.ImageSocket.
    /// Sends back the image either immediately from the cache in Ready state
    /// or piecewise progress step by progress step
    /// </summary>
    member this.DoRequest() =
        task {
            let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4)
            let! imageQuery = frontendSocket.ReceiveAsync(buffer, CancellationToken.None)
            let queryModel = ImageViewModel.FromArraySegment<ImageViewModel>(buffer.Slice(0, imageQuery.Count))
            logger.LogInformation(
                String.Format("Received Image query with coordinates X={0} Y={1} Z={2}",
                    queryModel.Coordinates.X, queryModel.Coordinates.Y, queryModel.Coordinates.Z))
            let image = imageService.Get(queryModel.Coordinates)
            if not image.IsReady then
                // Get the image from the renderer service through rendererSocket from the
                // concrete rendererClient received through DI
                use! rendererSocket = rendererClient.ConnectAsync()
                if image.IsEmpty then
                    // Image never seen by the renderer -> compute the parameters...
                    let configuration = ConfigServer.GetConfig(config)
                    let resolution = Resolution(Width = configuration.ImageWith, Height = configuration.ImageHeigth)
                    image.ComputeParams(resolution)
                // Kick off a new rendering or attach to the current rendering by sending the
                // parametrized ViewModel (as key for its cache) to the renderer
                let imageyModel = ImageViewModel(image)
                do! rendererSocket.SendAsync(imageyModel.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None)
                do! this.EchoProgress(buffer, frontendSocket, rendererSocket) // loop until Image.IsReady
                do! rendererSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "init close", CancellationToken.None)
            // Send the ready image back to the client and expect it to close the socket
            let imageyModel = ImageViewModel(imageService.Get(queryModel.Coordinates)) // from our cache
            do! frontendSocket.SendAsync(imageyModel.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None)
            let! close = frontendSocket.ReceiveAsync(buffer, CancellationToken.None)
            let status = if close.CloseStatus.HasValue then Some close.CloseStatus.Value else None
            match status with
            | Some closeStatus ->
                match closeStatus with
                | WebSocketCloseStatus.NormalClosure -> ()  // OK
                | _ -> logger.LogError(String.Format("Expected NormalClosure, but was {0}", closeStatus))
            | None -> logger.LogError("Expected close message after image.IsReady")
            do! frontendSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "ack client close", CancellationToken.None)
            logger.LogInformation("End of WebSocket request")
        }

type ImageSocketMiddleware(next: RequestDelegate,
                        imageEndpoint: PathString,
                        imageService: ImageService,
                        rendererImageClientFactory: IHttpClientFactory,
                        config: IConfiguration,
                        loggerFactory : ILoggerFactory) =
    let Logger = loggerFactory.CreateLogger<ImageSocketMiddleware>()

    member this.LogRequest(pathstring: string) =
        Logger.LogInformation(String.Format("Received WebSocket Request to {0}", pathstring))

    member this.Invoke (ctx : HttpContext, rendererClient: IWebSocketClient) =
        task {
            if ctx.WebSockets.IsWebSocketRequest then
                let path = ctx.Request.Path
                match path with
                | _ when path = EchoSocket.Endpoint ->
                    this.LogRequest(path.Value)
                    use! echoScoket = ctx.WebSockets.AcceptWebSocketAsync()
                    do! EchoSocket.Echo(ctx, echoScoket)
                | _ when path = imageEndpoint ->
                    this.LogRequest(path.Value)
                    use! frontendSocket = ctx.WebSockets.AcceptWebSocketAsync()
                    let handler = new ImageSocketHandler(frontendSocket, imageService,
                                    rendererClient, rendererImageClientFactory, config, loggerFactory)
                    do! handler.DoRequest() // await EchoProgress recursion to finish
                | _ -> return! next.Invoke ctx // another WebSocket Request
            else
                return! next.Invoke ctx
        }

[<Extension>]
type ImageSocketExtension =
    /// <summary>
    /// Adds the ImageSocket WebSocket at the path Site.FrontendWebSocket = /ws-image
    /// </summary>
    [<Extension>]
    static member inline UseImageSocket(app: IApplicationBuilder, endpoint: PathString) =
        app.UseWebSockets()
            .UseMiddleware<ImageSocketMiddleware>(endpoint)