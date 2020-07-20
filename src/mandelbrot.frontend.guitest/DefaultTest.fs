namespace mandelbrot.frontend.guitest

open NUnit.Framework
open OpenQA.Selenium.Chrome
open iselenium

type DefaultTest() =
    inherit SpaTest<ChromeDriver>()

    [<Test>]
    member this.NavigateDefaultTest() =
        this.Navigate("/")     