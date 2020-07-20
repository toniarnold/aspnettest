namespace mandelbrot.image

open System
open WebSharper

[<JavaScript>]
[<Serializable>]
type Coordinates(x: int64, y: int64, z:int64) =

    /// <summary>
    /// X-Coordinate of the image tile within the current zoom factor
    /// </summary>
    member val X  = x with get, set

    /// <summary>
    /// Y-Coordinate of the image tile within the current zoom factor
    /// </summary>
    member val Y  = y with get, set

    /// <summary>
    /// Z-Coordinate of the image tile determines the odd zoom factor for X and Y
    /// such that the result grid always has a center tile.
    /// Negative values are converted to reciprocal zoom factor, thus:
    ///  0 denotes a zoom factor 1
    /// +1 denotes a zoom factor 3
    /// +2 denotes a zoom factor 5
    /// -1 denotes a zoom factor 0.3333...
    /// -2 denotes a zoom factor 0.2
    /// </summary>
    member val Z  = z with get, set

    new() = Coordinates(0L, 0L, 0L)

    /// <summary>
    /// Memory aligned N * 64bit struct as composite hash key
    /// </summary>
    member this.Key =
        (this.X, this.Y, this.Z)