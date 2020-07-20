namespace mandelbrot.frontend

open System
open WebSharper

module Grid =

    [<JavaScript>]
    type Cell(x: int64, y: int64, z:int64, image:Guid) =
        let X = x
        let Y = y
        let Z = z
        let Image = image

    /// <summary>
    /// Create a plane display Grid as a square of the given size with vertical
    /// coordinate z with one center element at coordinates 0/0/z
    /// </summary>
    /// <param name="size">width of the square as number of elements, even
    /// numbers will be rounded up to the next odd number for one center
    /// element</param>
    let NewGrid(size: int64, z: int64) =
        let oddsize =
            match size % 2L with
            | 0L -> size + 1L
            | _ -> size
        let radius = (oddsize - 1L) / 2L   // minus the center element 0/0
        let range = [ - radius .. radius ]
        [ for x in range -> [for y in range -> Cell(x, y, z, Guid.Empty) ]]