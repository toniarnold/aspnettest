namespace mandelbrot.frontend.test

open NUnit.Framework
open mandelbrot.frontend.Grid

module GridTest =

    [<Test>]
    let NewGridTest () =
        let nine = NewGrid(3L, 0L)
        Assert.That(nine |> Seq.concat |> Seq.length, Is.EqualTo(9))