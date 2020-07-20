namespace mandelbrot.frontend

open mandelbrot.image
open Microsoft.Extensions.Caching.Memory
open statemap
open System
open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection

/// <summary>
/// Service Container for tthe mandelbrot.renderer client the caches
/// </summary>
type ImageService(imageCache: ImageCache, indexCache: IMemoryCache) =
    let ImageCache = imageCache
    let IndexCache = indexCache

    // MutEx locks
    static let create = new obj()
    static let replace = new obj()

    interface IDisposable with
        member this.Dispose() =
            ImageCache.Dispose()
            IndexCache.Dispose()

    /// <summary>
    /// Also delete the image from the index cache when it is removed from the ImageCache
    /// </summary>
    member this.PostEviction(key: obj, value: obj, reason: EvictionReason, state: obj) =
        let guid = (value :?> Image).Guid
        indexCache.Remove(guid)

    /// <summary>
    /// Replace the image in progress with its new state in the cache
    /// </summary>
    member this.ReplaceInCache(image: Image) =
        lock replace (fun () ->
            imageCache.Remove(image.Coordinates.Key)
            imageCache.GetOrCreate(image.Coordinates.Key, fun entry ->
                entry.Size <- image.Size() // new size with the rendered image
                image
            ) |> ignore
            indexCache.Set(image.Guid, image) |> ignore // was also removed in PostEviction
        )

    /// <summary>
    /// Get the image from the ImageCache described by key of its Coordinates.
    /// Returns a new instance with ongoing rendering if it does not yet exist,
    /// or the image either in Ready state or still rendering if it was already
    /// requested before.
    /// </summary>
    /// <param name="key">Image.Specification</param>
    member this.Get(coord: Coordinates) =
        let image = lock create (fun () ->
            ImageCache.GetOrCreate(coord.Key, fun entry ->
                // Not in cache -> create a new Image in empty state
                let newimage = new Image(coord)
                entry.Size <- newimage.Size() // initial size without the image
                // Can't cache yet by our Guid, as it comes from the render
                entry.RegisterPostEvictionCallback(fun key value reason state ->
                    this.PostEviction(key, value, reason, state)) |> ignore
                newimage // and immediately return empty image
            )
        )
        image // either newimage or image from the cache

    /// <summary>
    /// Get the existing image by Guid from ImageCache
    /// </summary>
    /// <param name="key">Image.Key</param>
    member this.Get(guid: Guid) =
        IndexCache.Get(guid) :?> Image

[<Extension>]
type ImageServiceExtension =
    /// <summary>
    /// Adds a Service Container for the Image SMC state machine
    /// </summary>
    [<Extension>]
    static member inline AddImageService(xs: IServiceCollection) =
        xs.AddSingleton<ImageService>()