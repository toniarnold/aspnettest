namespace mandelbrot.frontend.guitest.runner

open iselenium
open NUnit.Framework
open OpenQA.Selenium.Edge
open System
open Microsoft.Extensions.Configuration

type Runner() =
    inherit SpaTest<EdgeDriver>()
    interface ITestServer with
        member val ServerProcesses = null with get, set
        member val driver = null with get, set

    member this.Config = this.GetConfig()

    [<SetUp>]
    member this.SetUp() =
        this.StartServer(this.Config,
            server=this.Config.["RendererServer"],
            root=this.Config.["RendererRoot"],
            port=Nullable<int>(this.Config.GetValue<int>("RendererPort")))
        this.StartServer()

    [<TearDown>]
    member this.TearDown() =
        this.StopServer()

    [<Test>]
    member this.RunTests() =
        this.Navigate("/test")