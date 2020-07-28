namespace mandelbrot.image

open System
open WebSharper
open asplib.Model
open WebSharper.JavaScript

/// <summary>
/// Resolution of the image configured by the front end
/// </summary>
[<JavaScript>]
[<Serializable>]
[<CLIMutable>]
type RenderMap = {
    Empty: string
    Fractal: string
    InnerColors: string
    OuterColors: string
    Image: string
    Ready: string
    }

/// <summary>
/// Stored mutable ViewModel for the Image, passed through WebSockets. Needs to
/// be in a separate project due to cyclic dependencies - but is strictly
/// required, as F# JavaScript doesn't recognize C# JavaScript. Constructor for
/// the JS client to initialize the rendering with coordinates and resolution
/// </summary>
[<JavaScript>]
[<Serializable>]
type ImageViewModel(coordinates: Coordinates, resolution: Resolution) =
    inherit SmcViewModel<Image, ImageContext, ImageContext.ImageState>()

    [<DefaultValue>] val mutable RenderMap : RenderMap

    member val State = "" with get, set // C# state is not accessible by F# JS
    member val Coordinates = coordinates with get, set
    member val Resolution = resolution with get, set
    member val Params = Params() with get, set
    member val Guid  = "00000000-0000-0000-0000-000000000000" with get, set
    member val IsReady = false with get, set
    // Nested objects not available in F# JS and WebSharper lensing too much boilerplate -> flatten image parameters
    member val Magnification = "" with get, set
    member val Real = "" with get, set
    member val Imag = "" with get, set
    member val EscapeRadius = "" with get, set
    member val MaxIterations = "" with get, set

    /// <summary>
    /// Value for img src=
    /// </summary>
    member this.ImgSrc() = "/Img/" + this.Guid // Site.Endpoints.Img

    /// <summary>
    /// CSS id for the given Coordinates
    /// </summary>
    static member Id(coordinates: Coordinates) =
        "c-" + string coordinates.X +
        "-" + string coordinates.Y +
        "-" + string coordinates.Z

    /// <summary>
    /// Value for the CSS id=
    /// </summary>
    member this.Id() =
        ImageViewModel.Id(this.Coordinates)

    /// <summary>
    /// Constructor for WebSocket echo messages
    /// </summary>
    /// <param name="image"></param>
    [<JavaScript false>]
    new (image: Image) as this =
        ImageViewModel(Coordinates(), Resolution())
        then
            this.Main <- image
            this.LoadMembers()
            this.IsNew <- false

    /// <summary>
    /// Deserialization of reference types without parameterless constructor is not supported
    /// </summary>
    new () =
        ImageViewModel(Coordinates(), Resolution())

    /// <summary>
    /// Deserialize from JSON in the front end JS
    /// </summary>
    static member FromJson(json) =
        let obj = JSON.Parse(json)
        let image = ImageViewModel()
        Object.Assign(image, obj) |> ignore
        image

    [<JavaScript false>]
    override this.SaveMembers() =
        base.SaveMembers()
        this.Main.Coordinates <- this.Coordinates // set on the JS client

    [<JavaScript false>]
    override this.LoadMembers() =
        base.LoadMembers()
        this.State <- this.Main.State.ToString() // overrides base.State
        if (box this.Main.Coordinates) <> null then
             this.Coordinates <- this.Main.Coordinates
        if (box this.Main.Specification) <> null then
            this.Params <- this.Main.Specification.Params
            this.Resolution <- this.Main.Specification.Resolution
        this.Guid <- string this.Main.Guid
        this.IsReady <- this.Main.IsReady
        this.Magnification <- this.Params.Magnification.ToString()
        this.Real <- this.Params.Location.Real.ToString()
        this.Imag <- this.Params.Location.Imag.ToString()
        this.EscapeRadius <- this.Params.EscapeRadius.ToString()
        this.MaxIterations <- this.Params.MaxIterations.ToString()

    [<JavaScript false>]
    override this.LoadStateNames() =
        this.RenderMap <-  {
            Empty = ImageContext.RenderMap.Empty.Name
            Fractal = ImageContext.RenderMap.Fractal.Name
            InnerColors = ImageContext.RenderMap.InnerColors.Name
            OuterColors = ImageContext.RenderMap.OuterColors.Name
            Image = ImageContext.RenderMap.Image.Name
            Ready = ImageContext.RenderMap.Ready.Name
        }