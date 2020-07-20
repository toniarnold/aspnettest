namespace mandelbrot.frontend.test

open System
open NUnit.Framework
open asplib.Services
open mandelbrot.image
open mandelbrot.frontend

type SiteImageTest() =
    let Server = ServiceProvider.CreateFrontendServer()
    let ClientFactory = TestServerClientFactory(Server)

    [<OneTimeTearDown>]
    member this. DisposeTestServer() =
        Server.Dispose()

    [<Test>]
    member this. GetImgTest() =
        use client =  ClientFactory.CreateClient("HTTP Client") // also creates the ImageService
        // 1. Create an empty image in the cache
        let service = StaticServiceProvider.GetSingleton<ImageService>()
        let coordinates = Coordinates(X = 1L, Y = 1L, Z = -1L)
        let image = service.Get(coordinates)
        let guid = image.Guid
        // 2. Get the image with the empty.png placeholder from the image endpoint
        let uri = String.Format("/Img/{0}", guid)
        let response = client.GetByteArrayAsync(uri).Result
        Assert.That(response.Length, Is.EqualTo(160)) // empty.png size

    [<Test>]
    member this.GetImgNotFoundTest() =
        use client =  ClientFactory.CreateClient("HTTP Client")
        let uri = String.Format("/Img/{0}", Guid.NewGuid())
        let response = client.GetByteArrayAsync(uri).Result
        Assert.That(response.Length, Is.EqualTo(160))