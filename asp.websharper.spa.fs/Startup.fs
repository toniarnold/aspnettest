namespace asp.websharper.spa.fs

open asplib.Model.Db
open asplib.Remoting
open iselenium
open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open WebSharper.AspNetCore

type Startup private () =
    new (env: IWebHostEnvironment, configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration
        this.Environment <- env
        let conf =  this.Configuration
        do RemotingContext.Set(env, conf) |> ignore
        do ASP_DBEntities.ConnectionString <- conf.["ASP_DBEntities"]

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddDistributedMemoryCache() |> ignore
        services.AddSession(fun options ->
            options.IdleTimeout <- TimeSpan.FromMinutes(30.0)
            options.Cookie.HttpOnly <- true
        ) |> ignore
        services.AddLogging() |> ignore
        services.AddSitelet(TestResultSite.Main) |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseHsts() |> ignore

        app.UseMiddleware<IIEMiddleware>() |> ignore
        app.UseDefaultFiles() |> ignore
        app.UseHttpsRedirection() |> ignore
        app.UseStaticFiles() |> ignore
        app.UseSession() |> ignore
        app.UseWebSharper() |> ignore

        app.Run(fun context ->
            context.Response.StatusCode <- 404
            context.Response.WriteAsync("Page not found")) |> ignore

    member val Configuration : IConfiguration = null with get, set
    member val Environment : IWebHostEnvironment = null with get, set

module Program =
    let exitCode = 0

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
                webBuilder.CaptureStartupErrors(true) |> ignore
                webBuilder.UseSetting("detailedErrors", "true") |> ignore
            )

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args).Build().Run()

        exitCode