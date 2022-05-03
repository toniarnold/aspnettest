# ASP.NET Blazor Server

* [Summary](#summary)
* [Scaffolding of `minimal.blazor`](#scaffolding-of-minimalblazor)
  * [Using the static main accessor](#using-the-static-main-accessor)
  * [Persistence of the main state object](#Persistence-of-the-main-state-object)
    *  [The `DatabaseKeyAttribute` and `DatabaseSessionAttribute`](#the-databasekeyattribute-and-databasesessionattribute)
  * [Using the test project](#using-the-test-project)
* [`asp.blazor` with the SMC](#aspblazor-with-the-smc)
  *  [The ReRender override](#the-rerender-override)
  * [Database persistence sequence diagram](#database-persistence-sequence-diagram)

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
 `this.AssertPoll` extension methods are almost always required in NUnit
 tests.

Traditional session persistence is not possible for the same reason: The SignalR
communication doesn't allow to set session cookies. But database cookies can be
negotiated on the initial request, and further serialization of the stateful
main object doesn't require altering the cookie no more. The
`WithStorage.razor` template implements examples of these distinct
persistence mechanisms:

* Blazor - native persistence resembling ASP.NET WebForm's ViewState
* Database - implemented as in the other frameworks using a cookie
* Window.sessionStorage - JavaScript Session Storage provided by Blazor
* Window.localStorage - JavaScript local storage provided by Blazor


## Scaffolding of `minimal.blazor`

The initial setup  in `Program.cs` adds the following required services to
the raw template provided bv VS2022:

```
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
ASP_DBEntities.ConnectionString = builder.Configuration["ASP_DBEntities"];
builder.Services.AddPersistent<Main>();
app.UseMiddleware<ISeleniumMiddleware>(); 
```

The first two lines set up the database connection string for database
persistence. The static `ASP_DBEntities.ConnectionString` is only required
for the Selenium NUnit tests (to clean up after the tests were run), as the the
.NET Core Dependency Injection is not available for the TestFixture class
instantiation by the NUnit framework. Storing the connection string in a static
member was common practice in the .NET Framework days, before DI.

`AddPersistent<Main>()` sets up the `PersistentMainFactoryExtension`
which serializes/deserializes the central `Main` state object.


### Using the static main accessor

To be able to statically access the main state object, inherit from the generic
`StaticComponentBase` as this in the Razor page:
```
@using asplib.Components
@inherits StaticComponentBase<Models.Main>
```
This base class injects that central state object as a property `Main`. In
NUnit tests run from within the browser and thus the ASP.NET core process, the
state object can be accessed as `MainAccessor<Main>.Instance`.


### Persistence of the main state object

To make the main state object persistent, inherit from the
`PersistentComponentBase` instead:
```
@using asplib.Components
@inherits PersistentComponentBase<Models.Main>
```
This also injects a property `Main`, but makes it persistent according to
the storage type configured in the calue `"SessionStorage"` in
`aspsettongs.json`. The value is parsed as the global `Storage` enum
from the global `asplib` .NET Standard project shared by all frameworks.

Valid values in ASP.NET Blazor Server are (the last two are exclusive for Blazor
and not availiable in .NET Core MVC ore WebForms):

 * `ViewState`
   - Disables persistence, state will not survive a SignalR reconnection.
 * `Database`
   - Serializes the main state into the database which makes a
     `"ASP_DBEntities"` connection string in `aspsettongs.json`
     mandatory. State will be available as many days as configured in
     `"DatabaseStorageExpires"`.
 * `SessionStorage`
   - Serializes the main state into the browser JavaScript Window.sessionStorage
     via ProtectedSessionStorage. This makes the state local to the browser tab,
     and it will survive SignalR reconnections.
 * `LocalStorage`
   - Serializes the main state into the browser JavaScript Window.localStorage
     via ProtectedLocalStorage. It is up to the browser how long it will
     retain the data.

The binary serialization in the browser is (additionally to the built-in
Protected*Storage mechanism) encrypted by the server-side secret
`"EncryptViewStateKey"` and thus not manipulable by the client. This should
be enough to justify re-enabling the otherwise newly by .NET per default as
"unsafe" declared binary serialization by setting
`EnableUnsafeBinaryFormatterSerialization` to `true` in the .csproj
file.

#### The `DatabaseKeyAttribute` and `DatabaseSessionAttribute`

In Blazor, the `HttpContext` where the database `Main.session` value and the
optional encryption key are storedin permament Cookies is only available upon
the initial HTTP request, but no more during the SignalR connection afterwards.
Using these dynamically added attributes on the `Main` instance  is a
transparent workaround to keep them after the initial load for subsequent
storage into the database.


### Using the test project

Adding an NUnit test button from the `asplib.core` project is as simple as
adding
```
@using asplib.Components
<TestButton testproject="minimaltest.blazor" tabindex="1000" />
```

The `minimaltest.blazor` test project can't directly be referenced by the
Blazor application, as this would create a circular reference. The pattern used
in the solution is to add this post-build event which will copy the DLL to the
bin directory:
```
diff --binary $(TargetPath) $(SolutionDir)\src\bin
if errorlevel 1 xcopy /d /f /y $(TargetDir)\$(TargetName).* $(SolutionDir)\src\bin
```
and then reference the DLL created directly in the Blazor application under test.



## `asp.blazor` with the SMC

Unlike ASP.NET Core MVC or WebSharper, there is no additional ViewModel-like
wrapper necessary for the SMC class. The `Calculator.razor` example component
directly inherits persistence generically with the type of the SMC model class:
```
@inherits SmcComponentBase<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
```

### The ReRender override

The abstract `SmcComponentBase` scaffolds the setup of the SMC state machine and
adds an event handler for state changes which will call the virtual ReRender
method. It is the responsibility of the concrete component
(`CalculatorComponent` in the example) to dynamically display the parts
according to the SMC state by using the
[DynamicComponent](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/dynamiccomponent?view=aspnetcore-6.0)
which displays a sub-component according to type:

```
protected override void ReRender()
{
    switch (Main.State)
    {
        case var s when s == CalculatorContext.Map1.Splash:
            pageType = typeof(Splash);
            break;

        case var s when s == CalculatorContext.Map1.Calculate:
            pageType = typeof(Calculate);
            break;
        (...)
    }
}
```


### Database persistence sequence diagram

This UML sequence diagram is not formally correct, but rather illustrative.
Although the "HttpContext" is also a class, it doesn't call the Direct Injection
transient factory registered in the static `PersistentMainFactoryExtension`
class directly. The crucial fact to illustrate is the lifetime of the
HttpContext: It is only available during the initial GET request. Afterwards, it
will upgrade the HTTP connection to a SignalR WebSocket and dispose the
HttpContext instance.

The "Blazor" pseudo-class represents the whole Blazor Server Framework including
Direct Injection (DI), the SignalR connection to the browser and the component
lifecycle events, particularly the `OnAfterRenderAsync` override which serves
the purpose of hte `OnPreRender` event in the ASP.NET WebForms implementation
of the SMC pattern.

Also the `TypeDescriptor.AddAttributes` static method is not called on the "Main
SMC" object itself, but manipulates it by adding the  `DatabaseKeyAttribute` and
`DatabaseSessionAttribute`. This is required, as the Browser Cookie storing
these values is not available no more after the HttpContext has been disposed -
these attributes serve as a workaround.

![Blazor database persistence sequence diagram](blazor-database.png)

Drawback of this pattern: Unlike in ASP.NET WebForms, the component must be
initially visible on the GET request to trigger the Database persistence
mechanism. But storing the serialized object server-side probably is a legacy
technique that is replaced with Browser LocalStorage which did not exist when
designing the database persistency mechanism.