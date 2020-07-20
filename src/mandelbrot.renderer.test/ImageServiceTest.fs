namespace  mandelbrot.renderer.test

open NUnit.Framework
open NUnit.Framework.Constraints
open mandelbrot.image
open mandelbrot.renderer
open iselenium
open System

type ImageServiceTest() =
    let service = ServiceProvider.Get<ImageService>()

    interface IAssertPoll

    [<OneTimeTearDown>]
    member this.DisposeImageService() =
        // Free the objects hold in the dictionary to prevent locking the DLL in VS:
        (service :> IDisposable).Dispose()

    [<Test>]
    member this.CacheTest() =
        let frontendRequestImage = new Image(Coordinates(X = 0L, Y = 0L, Z = 0L))
        frontendRequestImage.Fsm.ComputeParams(Resolution(Width = 16, Height = 16)) // image template, the service returns a new one anyway
        Assert.That(frontendRequestImage.State, Is.EqualTo(ImageContext.RenderMap.Parameters))
        let key = frontendRequestImage.Specification;
        let rendernigImage = service.Get(key);
        Assert.That(rendernigImage.Coordinates, Is.Null) // cannot preserve coordinates from the query key
        Assert.That(rendernigImage.State, Is.EqualTo(ImageContext.RenderMap.Parameters)) // immediately after creation

        // Quickly poll the image being rendered in another thread instead of "blind" sleep.
        // Querying State instead of ReachedState throws StateUndefinedException
        this.AssertPoll(new ActualValueDelegate<_>(fun () -> rendernigImage.ReachedState),
                        new Func<_>(fun () ->
                            Is.EqualTo(ImageContext.RenderMap.Ready) :> IResolveConstraint),
                        timeout = new Nullable<int>(1));
        Assert.That(rendernigImage.State, Is.EqualTo(ImageContext.RenderMap.Ready))
        let imageBytes = rendernigImage.Bytes.Clone() :?> byte[]

        // Get the cached image
        let image2 = service.Get(key)
        Assert.That(image2.State, Is.EqualTo(ImageContext.RenderMap.Ready)) // immediately ready
        Assert.That(image2.Bytes, Is.EquivalentTo(imageBytes));    // the same rendered image

        // Get image via Guid from the index cache
        let image3 = service.Get(rendernigImage.Guid)
        Assert.That(image3.State, Is.EqualTo(ImageContext.RenderMap.Ready)) // immediately ready
        Assert.That(image3.Bytes, Is.EquivalentTo(imageBytes));    // the same rendered image