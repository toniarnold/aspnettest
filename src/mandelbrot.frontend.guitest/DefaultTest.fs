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
        Thread.Sleep(2000) // to be gazed at
        this.driver.Navigate().Refresh() // from the cache