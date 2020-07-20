namespace mandelbrot.renderer

open mandelbrot.image
open Microsoft.Extensions.Caching.Memory
open statemap
open System
open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection

/// <summary>
/// Service Container for the Image SMC state machine and the caches
/// </summary>
type ImageService(imageCache: ImageCache, indexCache: IMemoryCache) =

    // MutEx locks
    static let create = new obj()
    static let replace = new obj()

    interface IDisposable with
        member this.Dispose() =
            imageCache.Dispose()
            indexCache.Dispose()

    /// <summary>
    /// Replaces the image including the Bytes bitmap in the cache if it has become ready
    /// with the same image, but its full size
    /// </summary>
    member this.StateChanged(sender: obj, args: StateChangeEventArgs) =
        if args.NewState() = (ImageContext.RenderMap.Ready :> State) then
            let image = (sender :?> ImageContext).Owner
            lock replace (fun () ->
                imageCache.Remove(image.Specification.Key)
                imageCache.GetOrCreate(image.Specification.Key, fun entry ->
                    entry.Size <- image.Size() // new size with the rendered image
                    image
                ) |> ignore
                indexCache.Set(image.Guid, image) |> ignore // was also removed in PostEviction
            )

    /// <summary>
    /// Also delete the image from the index cache when it is removed from the imageCache
    /// </summary>
    member this.PostEviction(key: obj, value: obj, reason: EvictionReason, state: obj) =
        let guid = (value :?> Image).Guid
        indexCache.Remove(guid)

    /// <summary>
    /// Get the image from the imageCache described by key of its Specification.
    /// Returns a new instance with ongoing rendering if it does not yet exist,
    /// or the image either in Ready state or still rendering if it was already
    /// requested before.
    /// </summary>
    /// <param name="key">Image.Specification</param>
    member this.Get(spec: Specification) =
        let image = lock create (fun () ->
            imageCache.GetOrCreate(spec.Key, fun entry ->
                // Not in cache -> create a new Image in Params state
                let image = new Image(spec.Params, spec.Resolution)
                image.AddStateChangedHandler(fun sender args ->
                    this.StateChanged(sender, args)
                    )
                entry.Size <- image.Size() // initial size without the image
                // Cache by Image.Gid
                indexCache.Set(image.Guid, image) |> ignore
                entry.RegisterPostEvictionCallback(fun key value reason state ->
                    this.PostEviction(key, value, reason, state)) |> ignore
                image.RenderAsync() |> ignore // start the background rendering task and don't wait
                image // and immediately return the image in progress
            )
        )
        image // either newimage or image from the cache

    /// <summary>
    /// Get the existing image by Guid from imageCache
    /// </summary>
    /// <param name="key">Image.Key</param>
    member this.Get(guid: Guid) =
        indexCache.Get(guid) :?> Image

[<Extension>]
type ImageServiceExtension =
    /// <summary>
    /// Adds a Service Container for the Image SMC state machine
    /// </summary>
    [<Extension>]
    static member inline AddImageService(xs: IServiceCollection) =
        xs.AddSingleton<ImageService>()