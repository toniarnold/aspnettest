namespace mandelbrot.renderer

open System
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.Configuration
open System.Globalization
open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection

/// <summary>
/// Size-Limited memory Cache with configuration "ImageCacheSize" in bytes
/// </summary>
type ImageCache(config: IConfiguration) =
    inherit MemoryCache(
        new MemoryCacheOptions(
            SizeLimit = new Nullable<Int64>(
                Int64.Parse(config.GetValue("ImageCacheSize"), NumberStyles.Float))
        )
    )

[<Extension>]
type ImageCacheExtension =
    /// <summary>
    /// Adds a size-Limited memory Cache with configuration "ImageCacheSize" in bytes
    /// </summary>
    [<Extension>]
    static member inline AddImageCache(xs: IServiceCollection) =
        xs.AddSingleton<ImageCache>()