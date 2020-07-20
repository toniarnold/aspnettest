namespace  mandelbrot.renderer.test

open System
open NUnit.Framework
open asplib.Services
open mandelbrot.image
open mandelbrot.renderer

type SiteImageTest() =
    let Server = ServiceProvider.CreateTestServer()
    let ClientFactory = TestServerClientFactory(Server)

    [<OneTimeTearDown>]
    member this. DisposeTestServer() =
        Server.Dispose()

    [<Test>]
    member this. GetImgTest() =
        use client =  ClientFactory.CreateClient("HTTP Client") // also creates the ImageService
        // 1. Create the image in the cache, starts rendering
        let service = StaticServiceProvider.GetSingleton<ImageService>()
        let key = Specification(
                    Params = new Params(1.0, new Complex(Real = -0.712, Imag =  0.0), 4.0),
                    Resolution = new Resolution(Width = 16, Height = 16)
                    )
        let image = service.Get(key)
        let guid = image.Guid
        // 2. Get the image from the ImageEndpoint
        let uri = String.Format("/Img/{0}", guid)
        let response = client.GetByteArrayAsync(uri).Result
        Assert.That(response.Length, Is.GreaterThanOrEqualTo(160)) // empty.png size

    [<Test>]
    member this.GetImgNotFoundTest() =
        use client =  ClientFactory.CreateClient("HTTP Client")
        let uri = String.Format("/Img/{0}", Guid.NewGuid())
        let response = client.GetByteArrayAsync(uri).Result
        Assert.That(response.Length, Is.EqualTo(160))