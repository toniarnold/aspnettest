namespace mandelbrot.frontend

open System
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open WebSharper.AspNetCore
open asplib.Remoting
open asplib.Services
open iselenium

type Startup(env: IWebHostEnvironment, configuration: IConfiguration) =
    do RemotingContext.Set(env, configuration) |> ignore

    new(configuration: IConfiguration) = // ServiceProvider constructor for tests
        Startup(null, configuration)

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddSingleton(configuration)
            .AddDistributedMemoryCache() // session
            .AddImageCache()    // by Image.Coordinates
            .AddMemoryCache()   // by Image.Gid
            .AddImageService()  // Singleton for static access
            .AddSingleton<IWebSocketClient>(fun _ ->
                new ServerWebSocketClient(new Uri(configuration.GetValue<string>("RendererHost") +
                                            mandelbrot.renderer.Site.RendererWebSocket))
                :> IWebSocketClient
            )
            .AddHttpClient()    // IHttpClientFactory for GET image
            .AddSession(fun options ->
                options.IdleTimeout <- TimeSpan.FromMinutes(30.0)
                options.Cookie.HttpOnly <- true
            )
            .AddSitelet(Site.Main)
            .AddLogging()
        |> ignore

    member this.OnShutDown(disposable: IDisposable) =
        disposable.Dispose()

    member this.Configure(app: IApplicationBuilder,
                            env: IWebHostEnvironment,
                            applifetime: IHostApplicationLifetime,
                            disposable: ImageService) =
        if (env.IsDevelopment()) then
            WebSharper.Web.Remoting.DisableCsrfProtection() |> ignore
            app.UseDeveloperExceptionPage()
                .UseNoCache() |> ignore
            StaticServiceProvider.SetProvider(app.ApplicationServices);
        else
            app.UseHttpsRedirection()
                .UseHsts() |> ignore

        applifetime.ApplicationStopping.Register(fun _ -> this.OnShutDown(disposable)) |> ignore

        app.UseISelenium()
            .UseImageSocket(Site.FrontendWebSocket)
            .UseDefaultFiles()  // [<SPAEntryPoint>]
            .UseStaticFiles()   // [<SPAEntryPoint>]
            .UseSession()
            .UseRequestQuerySession()
            .UseWebSharper()
            .Run(fun context ->
                context.Response.StatusCode <- 404
                context.Response.WriteAsync("Page not found"))

module Program =
    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder
                    .UseStartup<Startup>()
                    .CaptureStartupErrors(true)
                    .ConfigureLogging(fun logging ->
                        logging.AddConsole()
                                .AddDebug() |> ignore
                    )
                    .UseSetting("detailedErrors", "true") |> ignore
            )

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args).Build().Run()
        0