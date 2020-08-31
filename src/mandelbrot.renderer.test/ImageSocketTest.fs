namespace  mandelbrot.renderer.test

open System
open NUnit.Framework
open mandelbrot.renderer
open System.Threading
open System.Net.WebSockets
open mandelbrot.image
open System.Threading.Tasks

// ImageSocket with a TestServer mandelbrot.renderer
type ImageSocketTest() =
    let Server = ServiceProvider.CreateTestServer()

    [<OneTimeTearDown>]
    member this.DisposeServer() =
        Server.Dispose()

    [<Test>]
    member this.EchoTest() =
        let client = Server.CreateWebSocketClient()
        let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4) // per test thread
        let uri = UriBuilder(Server.BaseAddress, Scheme = "ws", Path = EchoSocket.Endpoint.Value).Uri
        let socket = client.ConnectAsync(uri, CancellationToken.None).Result
        let msg =  [| 1uy; 2uy; 3uy; 4uy|]
        socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Binary, true, CancellationToken.None)
        |> Async.AwaitTask |> Async.RunSynchronously
        let response = socket.ReceiveAsync(buffer, CancellationToken.None)
                       |> Async.AwaitTask |> Async.RunSynchronously
        let echo = buffer.Slice(0, response.Count).ToArray()
        Assert.That(echo, Is.EqualTo(msg))
        socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "test done", CancellationToken.None)
        |> Async.AwaitTask |> ignore

    [<Test>]
    member this.EchoParallelTest() =
        let threads = [1..9]
        let result = Parallel.ForEach(threads, fun _ ->
            this.EchoTest()
        )
        Assert.That(result.IsCompleted, Is.True)

    [<Test>]
    member this.ImageSocketHandlerTest() =
        this.DoImageSocketHandlerTest 0

    member this.DoImageSocketHandlerTest thread  =
        let client = Server.CreateWebSocketClient()
        let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4) // per test thread
        let uri = UriBuilder(Server.BaseAddress, Scheme = "ws", Path = Site.RendererWebSocket.ToString()).Uri
        // Increment coordinates to compute another image for each thread
        let coord = Coordinates(X = int64 thread, Y = 0L, Z = 3L)
        let queryImage = new Image(coord) // mandelbrot.frontend client JS
        queryImage.Fsm.ComputeParams(Resolution(Width = 16, Height = 16)) // mandelbrot.frontend F#/.NET
        let queryModel = new ImageViewModel(queryImage)
        // Send query with the computed Params
        let socket = client.ConnectAsync(uri, CancellationToken.None).Result
        socket.SendAsync(queryModel.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None)
        |> Async.AwaitTask |> Async.RunSynchronously
        let mutable resultImage = new ImageViewModel() // empty init
        let mutable i = 0
        while not resultImage.IsReady do
            // Get and count the new images for each ImageStateChanged event
            let mutable result = socket.ReceiveAsync(buffer, CancellationToken.None)
                                 |> Async.AwaitTask |> Async.RunSynchronously
            i <- i + 1
            resultImage <- ImageViewModel.FromArraySegment<ImageViewModel>(buffer.Slice(0, result.Count))
            let fthread = if thread > 0 then String.Format("Thread {0}: ", thread) else String.Empty
            TestContext.Progress.WriteLine(
                String.Format("{0}Received resultImage.State {1}", fthread, resultImage.State))
            Assert.That(resultImage.Main.Coordinates.X, Is.EqualTo(thread))
        // Now resultImage.IsReady -> the client closes the connection
        // https://mcguirev10.com/2019/08/17/how-to-close-websocket-correctly.html
        socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "init close", CancellationToken.None)
        |> Async.AwaitTask |> Async.RunSynchronously
        Assert.That(resultImage.IsReady)
        Assert.That(i, Is.EqualTo(6))   // number of state transitions from Parameters to Ready

    [<Test>]
    member this.ImageSocketParallelTest() =
        let threads = [1..9]
        let result = Parallel.ForEach(threads, fun i ->
            this.DoImageSocketHandlerTest i
        )
        Assert.That(result.IsCompleted, Is.True)