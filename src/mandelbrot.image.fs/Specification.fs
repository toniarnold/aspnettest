namespace mandelbrot.image

open System
open WebSharper

/// <summary>
/// Complete Image specification sufficiently specifying the image bitmap.
/// Serves as cache Key for getting the image the renderer
/// </summary>
[<JavaScript>]
[<Serializable>]
type Specification(parameters: Params, resolution: Resolution) =
    member val Params = parameters with get, set
    member val Resolution = resolution with get, set

    new () = Specification(Params(), Resolution())

    /// <summary>
    /// Memory aligned N * 64bit struct without padding as composite hash key
    /// </summary>
    member this.Key = (
        this.Params.Location.Real,
        this.Params.Location.Imag,
        this.Params.Magnification,
        this.Params.EscapeRadius,
        this.Resolution.Width <<< 32 ||| this.Resolution.Height
        )