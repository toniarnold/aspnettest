namespace  mandelbrot.renderer.test

open Microsoft.Net.Http.Headers
open NUnit.Framework
open asplib.Services

type SiteIndexTest() =
    let Server = ServiceProvider.CreateTestServer()
    let ClientFactory = TestServerClientFactory(Server)

    let GetHttpClient() =
        let client = ClientFactory.CreateClient("HTTP Client")
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, ".NET HttpClient")
        client

    [<OneTimeTearDown>]
    member this.DisposeTestServer() =
        Server.Dispose()

    [<Test>]
    member this. HttpResponseTest() =
        use client = GetHttpClient()
        let response = client.GetStringAsync("/").Result
        Assert.That(response, Does.Contain("Web Service mandelbrot.renderer"))