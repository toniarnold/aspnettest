namespace  mandelbrot.frontend.test

open asplib.Services
open mandelbrot.frontend
open mandelbrot.renderer
open Microsoft.AspNetCore.TestHost
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open System.Net.Http
open NUnit.Framework
open System.IO

module ServiceProvider =

    let mutable RendererServer =  Option<TestServer>.None
    let mutable FrontendServer =  Option<TestServer>.None

    let DisposeServers() =
        match RendererServer with
        | Some server -> server.Dispose()
        | None -> ()
        match FrontendServer with
        | Some server -> server.Dispose()
        | None -> ()

    let GetConfiguration(projectname) =
        ConfigurationBuilder()
            .SetBasePath(
                Path.Join(
                    TestContext.CurrentContext.TestDirectory,
                    "..", "..","..", "..",  // .src/
                    projectname
                )
            )
            .AddJsonFile("appsettings.json", optional=false, reloadOnChange=true)
            .Build()

    let ServiceProvider =
        let startup = new mandelbrot.frontend.Startup(GetConfiguration("mandelbrot.frontend"))
        let sc = new ServiceCollection()
        startup.ConfigureServices(sc)
        sc.BuildServiceProvider()

    let Get<'T>() =
        ServiceProvider.GetService<'T>()

    let CreateRendererServer() =
        let builder = WebHostBuilder()
        builder.UseEnvironment("Development")
            .UseConfiguration(GetConfiguration("mandelbrot.renderer"))
            .ConfigureLogging(fun logging ->
                logging.AddConsole()
                        .AddDebug() |> ignore
            )
            .UseStartup<mandelbrot.renderer.Startup>() |> ignore
        RendererServer <- Some (new TestServer(builder))
        RendererServer.Value

    /// <summary>
    /// Create a FrontendServer/RendererServer TestServer pair, FrontendServer
    /// forwarding requests to the RendererServer
    /// </summary>
    let  CreateFrontendServer () =
        let renderer = CreateRendererServer()
        let builder = new WebHostBuilder()
        builder.UseEnvironment("Development")
            .UseConfiguration(GetConfiguration("mandelbrot.frontend"))
            .ConfigureLogging(fun logging ->
                logging.AddConsole()
                        .AddDebug() |> ignore
            )
            .UseStartup<mandelbrot.frontend.Startup>() |> ignore
        // No C# extension methods in F#, thus non-fluently override the test service mocks:
        WebHostBuilderExtensions.ConfigureTestServices(builder, fun services ->
            services.AddScoped<IWebSocketClient>(fun _ ->
                new TestServerWebSocketClient(renderer,
                    mandelbrot.renderer.Site.RendererWebSocket.ToString()) :> IWebSocketClient
            ).AddScoped<IHttpClientFactory>(fun _ ->
                new TestServerClientFactory(renderer) :> IHttpClientFactory
            )|> ignore
        ) |> ignore
        FrontendServer <- Some (new TestServer(builder))
        FrontendServer.Value