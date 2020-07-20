namespace mandelbrot.renderer

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open WebSharper.AspNetCore
open asplib.Remoting
open asplib.Services
open System

type Startup(configuration: IConfiguration) =

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddSingleton(configuration)
            .AddImageCache()    // by Image.Key
            .AddMemoryCache()   // by Image.Gid
            .AddImageService()  // Singleton for static access
            .AddLogging()
            .AddSitelet(Site.ImageBinary)
            .AddAuthentication("WebSharper")
            .AddCookie("WebSharper", fun options -> ())
        |> ignore

    member this.OnShutDown(disposable: IDisposable) =
        disposable.Dispose()

    member this.Configure(app: IApplicationBuilder,
                            env: IWebHostEnvironment,
                            applifetime: IHostApplicationLifetime,
                            disposable: ImageService) =
        if env.IsDevelopment() then
            WebSharper.Web.Remoting.DisableCsrfProtection() |> ignore
            app.UseDeveloperExceptionPage() |> ignore
            // Thanks to context.Environment.["WebSharper.AspNetCore.Services"] only needed for tests:
            StaticServiceProvider.SetProvider(app.ApplicationServices);

        applifetime.ApplicationStopping.Register(fun _ -> this.OnShutDown(disposable)) |> ignore

        app.UseAuthentication()
            .UseImageSocket(Site.RendererWebSocket)
            .UseRequestQuerySession()
            .UseWebSharper()
            .Run(fun context ->
                context.Response.StatusCode <- 404
                context.Response.WriteAsync("Page not found"))

module Program =
    let BuildWebHost args =
        WebHost
            .CreateDefaultBuilder(args)
            .ConfigureLogging(fun logging ->
                logging.AddConsole()
                        .AddDebug() |> ignore
            )
            .UseStartup<Startup>()
            .Build()

    [<EntryPoint>]
    let main args =
        BuildWebHost(args).Run()
        0