namespace mandelbrot.image

open System
open WebSharper
open FractalSharp.Algorithms.Fractals
open FractalSharp.Numerics.Generic

[<JavaScript>]
[<Serializable>]
type Complex(real: double, imag: double) =
    member val Real = real with get, set
    member val Imag = imag with get, set

    new() = Complex(0.0, 0.0)

/// <summary>
/// JavaScrip-table FractalSharp EscapeTimeParams<double>
/// </summary>
[<JavaScript>]
[<Serializable>]
type Params() =
    member val Magnification = 0.0 with get, set
    member val Location = new Complex(Real = 0.0, Imag = 0.0) with get, set
    member val EscapeRadius = 0.0 with get, set
    member val MaxIterations = 0 with get, set

    /// <summary>
    /// Constructor with calculated MaxIterations as
    /// Math.Log(magnification) * 40 + 100
    /// </summary>
    new(magnification, location, escaperadius) as this =
        Params()
        then
            this.Magnification <- magnification
            this.Location <- location
            this.EscapeRadius <- escaperadius
            // From https://github.com/blu3r4y/Mandelray/blob/master/Mandelray/src/Datastructures/MandelPos.cs
            this.MaxIterations <- (int)(Math.Log((if magnification < 1.0 then 1.0 else magnification)) * 40.0 + 100.0)

/// <summary>
/// Static Construction/Converter methods outside of [JavaScript]
/// </summary>
module ParamsConvert =

    let FromEscapeTimeParams(value: EscapeTimeParams<double>) =
        Params(
            Magnification = value.Magnification.Value,
            Location = Complex(
                Real = value.Location.Real.Value,
                Imag = value.Location.Imag.Value
                ),
            EscapeRadius = value.EscapeRadius.Value,
            MaxIterations = value.MaxIterations
            )

    let ToEscapeTimeParams(value: Params) =
        EscapeTimeParams<double>(
            Magnification = Number<double>(value.Magnification),
            Location = Complex<double>(
                Number<double>(value.Location.Real),
                Number<double>(value.Location.Imag)
                ),
            EscapeRadius = Number<double>(value.EscapeRadius),
            MaxIterations = value.MaxIterations
            )