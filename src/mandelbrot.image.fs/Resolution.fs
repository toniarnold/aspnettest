namespace mandelbrot.image

open System
open WebSharper

/// <summary>
/// Resolution of the image configured by the front end
/// </summary>
[<JavaScript>]
[<Serializable>]
type Resolution(width: int, height: int) =
    member val Width = width with get, set
    member val Height = height  with get, set

    new() = Resolution(0, 0)