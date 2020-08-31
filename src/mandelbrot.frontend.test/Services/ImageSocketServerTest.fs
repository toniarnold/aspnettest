namespace mandelbrot.frontend.test

open System
open NUnit.Framework
open System.Threading
open System.Net.WebSockets
open Microsoft.Extensions.Configuration
open mandelbrot.image
open mandelbrot.frontend
open mandelbrot.renderer
open iselenium
open asplib.Services
open System.Threading.Tasks

// ImageSocket with the real servers for mandelbrot.frontend and mandelbrot.renderer
[<Category("ITestServer")>]
type ImageSocketServerTest() =
    interface ITestServer with
        member val ServerProcesses = null with get, set
        member val driver = null with get, set

    member this.Config = this.GetConfig("testsettings.json")

    [<OneTimeSetUp>]
    member this.SetUpServers() =
        this.StartServer(this.Config,
            server=this.Config.["RendererServer"],
            root=this.Config.["RendererRoot"],
            port=Nullable<int>(this.Config.GetValue<int>("RendererPort")))
        this.StartServer(this.Config)

    [<OneTimeTearDown>]
    member this.TearDownServers() =
        this.StopServer()

    [<Test>]
    member this.EchoTest() =
        let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4)
        let uri = UriBuilder(this.Config.["FrontendHost"], Scheme = "ws", Path = EchoSocket.Endpoint.Value).Uri
        let client = new ServerWebSocketClient(uri)
        let socket = client.ConnectAsync().Result
        let msg =  [| 1uy; 2uy; 3uy; 4uy|]
        socket.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Binary, true, CancellationToken.None)
        |> Async.AwaitTask |> ignore
        let response = socket.ReceiveAsync(buffer, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
        let echo = buffer.Slice(0, response.Count).ToArray()
        Assert.That(echo, Is.EqualTo(msg))
        socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "test done", CancellationToken.None)
        |> Async.AwaitTask |> Async.RunSynchronously

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
        let buffer = WebSocket.CreateClientBuffer(1024 * 4, 1024 * 4)
        let coord = Coordinates(X = int64 thread, Y = 0L, Z = 3L)
        let resol = Resolution(16, 16)
        let queryModel = new ImageViewModel(coord, resol) // mandelbrot.frontend client JS
        // Send query with the raw Coordinates to the FrontendHost
        let uri = UriBuilder(this.Config.["FrontendHost"], Scheme = "ws", Path = Site.FrontendWebSocket.Value).Uri
        use client = new ServerWebSocketClient(uri)
        use socket = client.ConnectAsync().Result
        socket.SendAsync(queryModel.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None)
        |> Async.AwaitTask |> Async.RunSynchronously
        let mutable resultImage = new ImageViewModel() // empty init
        let mutable i = 0
        while not resultImage.IsReady do
            // Get and count the new images for each ImageStateChanged event
            let mutable result = socket.ReceiveAsync(buffer, CancellationToken.None) |> Async.AwaitTask |> Async.RunSynchronously
            i <- i + 1
            resultImage <- ImageViewModel.FromArraySegment<ImageViewModel>(buffer.Slice(0, result.Count))
            let fthread = if thread > 0 then String.Format("Thread {0}: ", thread) else String.Empty
            TestContext.Progress.WriteLine(
                String.Format("{0}Received resultImage.State {1}", fthread, resultImage.State))
            Assert.That(resultImage.Main.Coordinates.X, Is.EqualTo(thread))
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