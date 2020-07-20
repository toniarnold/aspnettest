namespace  mandelbrot.renderer.test

open System
open NUnit.Framework
open mandelbrot.renderer
open System.Threading
open System.Net.WebSockets
open mandelbrot.image

// ImageSocket with a TestServer mandelbrot.renderer
type ImageSocketTest() =
    let Server = ServiceProvider.CreateTestServer()

    [<OneTimeTearDown>]
    member this.DisposeServer() =
        Server.Dispose()

    [<Test>]
    member this.EchoTest() =
        let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4)
        let client = Server.CreateWebSocketClient()
        let uri = UriBuilder(Server.BaseAddress, Scheme = "ws", Path = EchoSocket.Endpoint.Value).Uri
        let socket = client.ConnectAsync(uri, CancellationToken.None).Result
        let msg =  [| 1uy; 2uy; 3uy; 4uy|]
        socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Binary, true, CancellationToken.None)
        |> Async.AwaitTask |> ignore
        let response = socket.ReceiveAsync(buffer, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
        let echo = buffer.Slice(0, response.Count).ToArray()
        Assert.That(echo, Is.EqualTo(msg))
        socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "test done", CancellationToken.None)
        |> Async.AwaitTask |> ignore

    [<Test>]
    member this.ImageSocketHandlerTest() =
        let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4)
        let client = Server.CreateWebSocketClient()
        let uri = UriBuilder(Server.BaseAddress, Scheme = "ws", Path = Site.RendererWebSocket.ToString()).Uri
        let coord = Coordinates(X = 0L, Y = 0L, Z = 1L)
        let queryImage = new Image(coord) // mandelbrot.frontend client JS
        queryImage.Fsm.ComputeParams(Resolution(Width = 16, Height = 16)) // mandelbrot.frontend F#/.NET
        let queryModel = new ImageViewModel(queryImage)
        // Send query with the computed Params
        let socket = client.ConnectAsync(uri, CancellationToken.None).Result
        socket.SendAsync(queryModel.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None)
        |> Async.AwaitTask |> ignore
        let mutable result = socket.ReceiveAsync(buffer, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
        let mutable resultImage = new ImageViewModel() // empty init
        let mutable i = 0
        while not result.CloseStatus.HasValue do
            // Count and get the new images for each ImageStateChanged event
            i <- i + 1
            resultImage <- ImageViewModel.FromArraySegment<ImageViewModel>(buffer.Slice(0, result.Count))
            Assert.That(resultImage.Main.Coordinates.Z, Is.EqualTo(coord.Z)) // preserved, but a copy
            result <- socket.ReceiveAsync(buffer, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
        socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "image rendered", CancellationToken.None)
        |> Async.AwaitTask |> ignore
        Assert.That(resultImage.IsReady)
        Assert.That(i, Is.EqualTo(6))   // number of state transitions from Parameters to Ready