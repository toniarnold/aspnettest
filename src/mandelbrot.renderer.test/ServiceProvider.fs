namespace  mandelbrot.renderer.test

open mandelbrot.renderer
open Microsoft.AspNetCore.TestHost
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

module ServiceProvider =

    let Configuration =
        ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional=false, reloadOnChange=true)
            .Build()

    let ServiceProvider =
        let startup = new Startup(Configuration)
        let sc = new ServiceCollection()
        startup.ConfigureServices(sc)
        sc.BuildServiceProvider()

    let Get<'T>() =
        ServiceProvider.GetService<'T>()

    let  CreateTestServer () =
        let builder = WebHostBuilder()
        builder.UseEnvironment("Development")
            .UseConfiguration(Configuration)
            .ConfigureLogging(fun logging ->
                logging.AddConsole()
                        .AddDebug() |> ignore
            )
            .UseStartup<Startup>() |> ignore
        new TestServer(builder)