namespace mandelbrot.frontend.guitest

open iselenium
open NUnit.Framework
open OpenQA.Selenium.Chrome
open System.Threading

type DefaultTest() =
    inherit SpaTest<ChromeDriver>()

    [<Test>]
    member this.NavigateDefaultTest() =
        this.Navigate("/") // to be calculated
        Thread.Sleep(60 * 1000) // image rendering to be gazed at
        this.Navigate("/") // from the cache