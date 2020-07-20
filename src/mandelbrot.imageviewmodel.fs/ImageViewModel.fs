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

    let mutable _coordinates = coordinates
    let mutable _resolution = resolution
    let mutable _params = Params()
    [<DefaultValue>] val mutable RenderMap : RenderMap
    let mutable _guid = "00000000-0000-0000-0000-000000000000"
    let mutable _isReady = false

    member this.Coordinates
        with get() = _coordinates
        and set(value) = _coordinates <- value
    member this.Resolution
        with get() = _resolution
        and set(value) = _resolution <- value
    member this.Params
        with get() = _params
        and set(value) = _params <- value
    member this.Guid
        with get() = _guid
        and set(value) = _guid <- value
    member this.IsReady
        with get() = _isReady
        and set(value) = _isReady <- value
    member this.ImgSrc = "/Img/" + this.Guid

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
    /// Serialize to JSON in the front end JS
    /// </summary>
    member this.ToJson() =
        JSON.Stringify(this)

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
        this.Main.Coordinates <- _coordinates // set on the JS client

    [<JavaScript false>]
    override this.LoadMembers() =
        base.LoadMembers()
        if (box this.Main.Coordinates) <> null then
             _coordinates <- this.Main.Coordinates
        if (box this.Main.Specification) <> null then
            _params <- this.Main.Specification.Params
            _resolution <- this.Main.Specification.Resolution
        _guid <- string this.Main.Guid
        _isReady <- this.Main.IsReady

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