# ASP.NET Blazor Server

* [Summary](#summary)
* [Scaffolding of ```minimal.blazor```](#scaffolding-of-minimal.blazor)

## Summary

 [WebSharper](websharper.md) was made somewhat obsolete with the arrival of
 Microsoft Blazor Server. The earlier Blazor WebAssembly was not considered
 here, as only the server variant brings back the productivity that once was
 possible with [ASP.NET WebForms](webforms-core.md) by abstracting away the client-server
 communitacion and thus eliminates the need to program a separate backend API.

 From the testing point of view, Blazor Server is more similar to the
 [WebSharper](websharper.md) experience, as it behaves as a JavaScript SPA on
 the client side. Except for the initial HTTP request which initiates the
 SignalR two-way connection used in later interactions, there are no subsequent
 HTTP requests, thus there is no HttpContexxt available. For that reason, the
 ```this.AssertPoll``` extension methods are almost always required in NUnit
 tests.

Traditional session persistence is not possible for the same reason: The SignalR
communication doesn't allow to set session cookies. But database cookies can be
negotiated on the initial request, and further serialization of the stateful
main object doesn't require altering the cookie no more. The
```WithStorage.razor``` template implements examples of these distinct
persistence mechanisms:

* Blazor - native persistence resembling ASP.NET WebForm's ViewState
* Database - implemented as in the other frameworks using a cookie
* Window.sessionStorage - JavaScript Session Storage provided by Blazor
* Window.localStorage - JavaScript local storage provided by Blazor


## Scaffolding of ```minimal.blazor```

The initial setup  in ```Program.cs``` adds the following required services to
the raw template provided bv VS2022:

```
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
ASP_DBEntities.ConnectionString = builder.Configuration["ASP_DBEntities"];
builder.Services.AddPersistent<Main>();
app.UseMiddleware<ISeleniumMiddleware>(); 
```

The first two lines set up the database connection string for database
persistence. The static ```ASP_DBEntities.ConnectionString``` is only required
for the Selenium NUnit tests (to clean up after the tests were run), as the the
.NET Core Dependency Injection is not available for the TestFixture class
instantiation by the NUnit framework. Storing the connection string in a static
member was common practice in the .NET Framework days, before DI.

```AddPersistent<Main>()``` sets up the ```PersistentMainFactoryExtension```
which serializes/deserializes the central ```Main``` state object.



