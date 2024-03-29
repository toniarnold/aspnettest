# ASP.NET Blazor Server

* [Summary](#summary)
* [Quickstart](#quickstart)
* [Screen recordings](#screen-recordings)
* [Scaffolding of `minimal.blazor`](#scaffolding-of-minimalblazor)
  * [Using the static `TestFocus` accessor](#using-the-static-testfocus-accessor)
    * [Automatic synchronization with `TestFocus`](#automatic-synchronization-with-testfocus)
    * [@ref Component references](#ref-component-references)
  * [Persistence of the main state object](#Persistence-of-the-main-state-object)
    * [Scope correlation setup](#scope-correlation-setup)
    * [Database persistence sequence diagram](#database-persistence-sequence-diagram)
    * [Browser storage persistence sequence diagram](#browser-storage-persistence-sequence-diagram)
  * [Using the test project](#using-the-test-project)
* [`asp.blazor` with the SMC](#aspblazor-with-the-smc)
  * [The RenderMain override](#the-rendermain-override)
* [`EditForm` handling](#editform-handling)
  * [`select` and `InputSelect` inputs](#select-and-inputselect-inputs)
* [Multiple async renders](#multiple-async-renders)
* [Comparison with bUnit](#comparison-with-bunit)
  * [Semantic HTML comparison in bUnit](#semantic-html-comparison-in-bunit)
  * [The `BUnitTestContext`](#the-bunittestcontext)
* [Double the test speed with whitebox tests](#double-the-test-speed-with-whitebox-tests)
* [The Text Explorer test runner](#the-test-explorer-test-runner)

## Summary

[WebSharper](websharper.md) was made somewhat obsolete with the arrival of
Microsoft Blazor Server. The earlier Blazor WebAssembly was not considered
here, as only the server variant brings back the productivity that once was
possible with [ASP.NET WebForms](webforms-core.md) by abstracting away the
client-server communitacion and thus eliminates the need to program a separate
backend API.

From the testing point of view, Blazor Server is more similar to the
[WebSharper](websharper.md) experience, as it behaves as a JavaScript SPA on
the client side. Except for the initial HTTP request which initiates the
SignalR two-way connection used in later interactions, there are no subsequent
HTTP requests, thus there is no HttpContext available.

Traditional session persistence is not possible for that reason: The SignalR
communication doesn't allow to set session cookies. But database cookies can be
negotiated on the initial request, and further serialization of the stateful
main object doesn't require altering the cookie no more. The
`WithStorage.razor` template implements examples of these distinct
persistence mechanisms:

* Blazor - native persistence resembling ASP.NET WebForm's ViewState
* Database - implemented as in the other frameworks using a cookie
* Window.sessionStorage - JavaScript Session Storage provided by Blazor
* Window.localStorage - JavaScript local storage provided by Blazor
* UrlQuery - URL query string

Unlike for WebSharper SPAs (and ultimately *all* modern SPA frameworks as
React and Anguler), there is no need to wait and poll for changes to happen
(`AssertPoll`), which turns out to be very brittle.

Instead, the `ITestFocus` static class in `asplib.blazor` provides an
`EndRender` resp. `EndRenderAsync` extension method to synchronize the tests.

Read about the [SpecFlow integration](specflow.md) in its own documentation.

## Quickstart

With the `aspnettest.template.blazor` NuGet package, a `dotnet new` template is
available to generate a Blazor Server App already wired up with bUnit and
aspnettest Selenium:

```sh
dotnet new install aspnettest.template.blazor
dotnet new aspnettest-blazor -o MyBlazorApp
```

If you haven't already, enable the "Microsoft WebDriver" in "Apps and Features"
in the Windows Settings. Then open the generated MyBlazorApp.sln with Visual
Studio. Due to the circular dependency (by design) between the app and its
Selenium tests, it is mandatory to build the solution *twice*, the second time
enforced as "Rebuild Solution". From the Test-Explorer window, open the
generated MyBlazorApp.playlist. It contains two test projects:
`MyBlazorAppBunitTest` (which runs fast) and `MyBlazorAppSeleniumTestRunner`
which starts the web server, an Edge browser instance and runs the tests in
`MyBlazorAppSeleniumTest` (excluded from the playlist) by pushing the test
button in the browser.


## Screen recordings

Running the `BlazorApp1.playlist` from `aspnettest.template.blazor`:

![aspnettest.template.blazor/BlazorApp1.playlist running](img/template.blazor-running.gif)

Unlike Edge, FireFox doesn't block mouse click events when run by the Selenium
WebDriver. The additional counter increment causes the test to fail:

![aspnettest.template.blazor interruption with FireFox](img/template.blazor-running-firefox.gif)



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


### Using the static `TestFocus` accessor

To be able to statically access the test component in focus with a contained
main state object from within tests, inherit from the generic
`StaticComponentBase` as this in the Razor page:
```
@using asplib.Components
@inherits StaticOwningComponentBase<Models.Main>
```
This base class injects that central state object as a property `Main` into the
containing Component. In NUnit tests run from within the browser and thus the
ASP.NET core process, the state object can be accessed statically as
`TestFocus.Component.Main`. When inheriting from
`StaticOwningComponentTest<TWebDriver, TComponent, TMain>`, the instance is
directly accessible via the generic `Main` property.

#### Automatic synchronization with `TestFocus`

Blazor Components to be synchronized should call the `EndRender` resp.
`EndRenderAsync` extension method (defined in the
`ITestFocus`/`TestFocusExtension` static class in `asplib.blazor`) at the *end* of
its own `OnAfterRender` resp. `OnAfterRenderAsync` override (or simply inherit
from `StaticComponentBase`). This will set the `TestFocus.Event`
`AutoResetEvent` to allow the waiting test method to continue and assign itself
(the Blazor component instance) to `TestFocus.Component` if and only if its type
was brought into focus by `SetFocus`. This diagram shows the sequence of a page
request and a subsequent button click issued from an NUnit test fixture and its
synchronization with `TestFocus.Event`:

```csharp
[SetUp]
public void SetFocus()
{
    TestFocus.SetFocus(typeof(TComponent));
}

[Test]
public void ClickSubmitTest()
{
    Navigate("/Withstatic");
    Click(Component.submitButton);
}

```

![Blazor synchronization sequence diagram](blazor-synchronization.png)

The cause of the need for synchronization is Blazor's "Render" client-side
JavaScript: Selenium returns from `Navigate` requests resp. `Click` calls
*before* the page is rendered. To wait, the test thread blocks at `WaitOne` on
the `AutoResetEvent` of `TestFocus`, as can be seen in the overlapping within
Blazor's rendering process, here simply abbreviated as "Render". After having
finished, this process will cause a call to Blazor's virtual `OnAfterRender`
method on the server which calls our `EndRender` method outlined above. This
will call Expose() for the Component, which adds the static C# reference
`TestFocus.Component`.

#### @ref Component references

Therefore the statically referenced component under test is still instantiated
after the first Navigate() block (on the left and the second Click() block can
obtain the Id attribute through the `IdAttr()` extension method which in turn
accesses the ElementReference.Id instance property.

Blazor automatically generates a reference Guid when a Component or HTML element
reference is added through the [@ref
attribute](https://docs.microsoft.com/en-us/dotnet/architecture/blazor-for-web-forms-developers/components#capture-component-references).
This empty HTML attribute with the undocumented(?) `_bl_` prefix for the Guid
looks like this in Blazor-generated HTML (also for some built-in input
components except `InputRadio<T>` for which an implementation has already been
[committed in git](https://github.com/dotnet/aspnetcore/pull/40828), is planned
for the Blazor 7 release):

```html
<button type="submit" _bl_6a88286a-1854-4d65-b9b4-140850e5ae7e="">Submit</button>
```


### Persistence of the main state object

To make the main state object persistent, inherit from the
`PersistentComponentBase` instead:

```csharp
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
ProtectedStorage mechanism) encrypted by the server-side secret
`"EncryptViewStateKey"` and thus not manipulable by the client. This should
be enough to justify re-enabling the otherwise newly by .NET per default as
"unsafe" declared binary serialization by setting
`EnableUnsafeBinaryFormatterSerialization` to `true` in the .csproj
file.


#### Scope correlation setup

This is only required for database persistence based on cookies. A request to a
Blazor Server application effectively consists of two requests: First comes the
initial full HTTP request/response with a valid `HttpContext` which can handle
cookies as usual. Then the Blazor App issues a second request (the one with the
`id` query string) to set up the SignalR connection. This second request
creates a second scope in which the Blazor App will run for the SignalR
connection lifetime. To retrieve persistent objects instantiated in the first
scope in the second scope from a cache, these scopes must be correlated for a
given client request.

To pass a correlation Guid from the first scope to the second one, the docs
recommend this pattern to "Pass tokens to a Blazor Server app" according to
[https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-6.0#pass-tokens-to-a-blazor-server-app](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-6.0#pass-tokens-to-a-blazor-server-app)

To hook into the scope correlation mechanism, two small additions are made:

##### ./Pages/_Host.cshtml

Add the `param-CorrelationGuid="@Guid.NewGuid()"` to the app component:

```
@page "/"
(�)
<component type="typeof(App)" param-CorrelationGuid="@Guid.NewGuid()" render-mode="ServerPrerendered" />
```

##### ./App.razor

On top, add `@inject ScopeCorrelationProvider provider` and the following
`@code` block transferring the correlation Guid from the parameter set by
`_Host.cshtml` to the SignalR-scoped service:

```
@inject ScopeCorrelationProvider provider
(�)
@code {
    [Parameter] // received once from _Host.cshtml
    public Guid CorrelationGuid { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        provider.SetScope(CorrelationGuid);
        return base.OnInitializedAsync();
    }
}
```

#### Database persistence sequence diagram

This sequence diagram shows the call sequence for database persistence over two
distinct browser sessions. That the `<<create>>` call to instantiate the Main
FSM is done by `ActivatorUtilities.CreateInstance` in the DI factory has been
omitted for brevity.

![Database persistence sequence diagram](blazor-database.png)

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

Drawback of this pattern: Unlike in ASP.NET WebForms, the component must be
initially visible on the GET request to trigger the Database persistence
mechanism. But storing the serialized object server-side can be considered as a
legacy technique that is replaced with Browser LocalStorage which did not exist
when designing the database persistence mechanism.

On the other hand, Browser storage may cause significant network traffic from
the client on each state change (just the same as WebForms' ViewState PostBack)
if the serialized state object grows big. With database storage, the serialized
object data is only moved from the web server to the database server unless the
page gets reinitialized on the client (e.g. after a SignalR disconnect).


#### Browser storage persistence sequence diagram

This diagram exemplifies the usage of Blazor's `ProtectedLocalStorage` in the
Browser, but the call sequence is exactly the same for
`ProtectedSessionStorage`. The `ActivatorUtilities.CreateInstance` for
`<<create>>` in the DI factory has also been omitted here.

![Browser storage persistence sequence diagram](blazor-browserstorage.png)

Unlike for the Database persistence diagram, there are no two separate blocks
(although `ProtectedLocalStorage` survives Browser restarts). As DI must
guarantee an instance right away (before it could be loaded from the Browser),
the Main FSM gets instantiated twice: The first default instance is an
initialization throwaway which gets overwritten with a stored instance.


### Using the test project

Adding an NUnit test button from the `iselennium.blazor` project is as simple as
adding

```html
@using iselenium.Components
<TestButton testproject="minimaltest.blazor" />
```

The `minimaltest.blazor` test project can't directly be referenced by the
Blazor application, as this would create a circular reference. The pattern used
in the solution is to add this post-build event which will copy the DLL to the
bin directory:

```
diff --binary $(TargetPath) $(SolutionDir)\src\bin
if errorlevel 1 xcopy /d /f /y $(TargetDir)\$(TargetName).* $(SolutionDir)\src\bin
```

and then reference the DLL created directly in the Blazor application under
test. Set "Copy Local" to true, as the TestRunnerBase loads the DLL from there.

The TestRunnerBase will load the DLL from the `bin` path parent to the
`Environment.ContentRootPath` which only works when run from VisualStudio
itself, but not from a release directory. However, the various Selenium Driver
.exe are not in the path anyway when running a published Blazor application via
run.bat - which, unlike .NET Core MVC, is required in Blazor for the `_content`
directoy from `asplib.blazor`.


## `asp.blazor` with the SMC

Unlike ASP.NET Core MVC or WebSharper, there is no additional ViewModel-like
wrapper necessary for the SMC class. The `Calculator.razor` example component
directly inherits persistence generically with the type of the SMC model class:
```
@inherits SmcComponentBase<Calculator, CalculatorContext, CalculatorContext.CalculatorState>
```

### The RenderMain override

The abstract `SmcComponentBase` scaffolds the setup of the SMC state machine
with its `HydrateMain` override that adds an event handler for state changes
which in turn will call the virtual `RenderMain` method. It is the
responsibility of the concrete component (`CalculatorComponent` in the example)
to dynamically display the parts according to the SMC state by using the
[DynamicComponent](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/dynamiccomponent?view=aspnetcore-6.0)
which displays a sub-component according to its type:

```csharp
protected override void RenderMain()
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

The `SmcComponentBase` also conveniently provides generic accessors for the
`Fsm` and its `State`.

## `EditForm` handling

A Blazor component containing a validating `EditForm` will not trigger
`OnAfterRender()` when submitting with a validation failure by itself. But when
derived from `StaticOwningComponentBase<T>`, `AddEditContextTestFocus()`
automatically adds a `OnValidationStateChanged` handler which will re-render the
component also on validation failure, which in turn triggers the usual
`TestFocus` synchronization. Wire it up immediately after the the `editContext`
instantiation:

```csharp
protected override void OnInitialized()
{
    editContext = new(Main);
    this.AddEditContextTestFocus(editContext);
}
```

### `select` and `InputSelect` inputs

The former is as direct HTML `<select>` element, the latter a Blazor Input
component which is rendered as `<select multiple="">` HTML element. They
currently cannot be selected by its instance, but individual options clicked on
by CSS selectors like this multi select example:

```csharp
Click(By.CssSelector, "#saladSelection > option[value=Corn]",  expectRenders: 0);
Click(By.CssSelector, "#saladSelection > option[value=Lentils]",  expectRenders: 0);
```

The non-default `expectRenders: 0` has to be added to prevent triggering
`TestFocus` synchronization when there is no server action.


## Multiple async renders

The optional `expectRenders` parameter on the `Click` method default to 1 and
replaces the original expectRender bool which just discriminated whether a
rendering is expected to happen (usually) or not (e.g. when selecting an
option).

But when awaiting another async method in an async event handler, Blazor will
return to the caller method from the framework, allowing it to re-render the
component. The `AutoResetEvent` from `TestFocus` needs to be awaited as many
times as a re-rendering is triggered, otherwise it will continue too early.

The `Async.razor` example page specifically exposes that property of Blazor. The
core is the following button click event handler:

```
public static int Iterations = 100;
public CountModel model = new(Iterations);

public async Task Start()
{
    while (model.Counter > 0)
    {
        model.Counter--;
        await Task.Delay(1);
        if (model.Counter % 2 == 0) // only render each 2nd time
        {
            StateHasChanged();
        }
    }
}
```

The corresponding test methods with a given iteration count either need to wait
for the rendering cascade to finish (`NonSynchronized`) - or to specify the
exact expected number of renderings in advance (`Synchronized`):

```
[Test]
public void NonSynchronized()
{
    Navigate("/async");
    Assert.That(Component.countNumber.Value, Is.EqualTo(Async.Iterations));
    Click(Component.startButton);
    this.AssertPoll(() => Component.countNumber.Value, () => Is.EqualTo(0));
}

[Test]
public void Synchronized()
{
    Navigate("/async");
    Assert.That(Component.countNumber.Value, Is.EqualTo(Async.Iterations));
    // Will always render once plus additionally half of the iterations
    cut.Click(cut.Instance.startButton, expectRenders: (Async.Iterations / 2) + 1);
    Assert.That(Component.countNumber.Value, Is.EqualTo(0));
}
```


## Comparison with bUnit

Tests based on the new [bUnit](https://bunit.dev/) library draw upon ordinary
unit tests running in the Test-Explorer of Visual Studio. As such, there is no
web server and no Selenium invoved: A tests directly instantiates a component by
calling the static `RenderComponent<T>()` factory which yields an
`IRenderedComponent<T>` instance.

This obtained object provides access to the `IRenderedFragment` produced by the
Blazor component and also directly to its instance itself. There is no static
`TestFocus` instance accessor and no synchronization necessary as with the
Selenium tests, as instantiation and rendering happen synchronously within the
test method. Assertions can directly read the global monolithic state object
in both testing paradigms alike.

The generated id HTML attributes for finding e.g. clickable buttons are as
undocumented in bUnit as the ones provided by Blazor itself and differ slightly:

**Blazor Server**
```html
<button _bl_75ef2312-3b13-4dfe-925b-b29a10026a50="">Enter</button>
```

**bUnit**
```html
<button blazor:onclick="2" blazor:elementReference="d3e0716b-28b0-48d8-ac2e-33a3aa6cab47">Enter</button>
```

The reason for the difference lies in the fact that bUnit's internal `Htmlizer` is
a modified copy of Blazor's internal
[`HtmlRenderer`](https://source.dot.net/#Microsoft.AspNetCore.Mvc.ViewFeatures/RazorComponents/HtmlRenderer.cs)
according to the source comments.

In the bUnit documentation, these unambiguous instance reference id attributes
are not used for finding HTML elements (they also turned out to be left empty in
some cases, e.g. the enter button in the footer part of the
`CalculatorComponent` after state changes in the example). Instead, these are
located by arbitrary CSS selectors in the RenderFragment.

**WebForms**
```html
<input type="submit" name="ctl00$ContentPlaceHolder1$calculator$footer$enterButton" value="Enter" id="ContentPlaceHolder1_calculator_footer_enterButton">
```

Accessing elements by id is an inheritance from the original ASP.NET WebForms
implementation of aspnettest. In WebForms, the publicly documented
[ClientID](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.control.clientid)
property is *always* generated for web controls, while in Blazor it is only
generated implicitly when there is an explicit `@ref` reference to the element.
One could argue that adding `@ref` component references just for accessing
elements within tests clutters the application with otherwise unnecessary ids.

### Semantic HTML comparison in bUnit

It is encouraged to write tests as .razor classes in bUnit. These obviate the
need for quoting the HTML to structurally compare expected and actual HTML:

```csharp
var title = cut.Find("h2");
title.MarkupMatches(@<h2>RPN calculator</h2>);
```

However, IntelliSense seems to be less intelligent in .razor files and the
"pythonic" [raw string
literals](https://github.com/dotnet/csharplang/blob/main/proposals/raw-string-literal.md)
in C# 11 provide another way to prevent having to quote quotation marks in HTML
attributes.

The [semantic HTML comparer from
bUnit](https://bunit.dev/docs/verification/verify-markup.html) is also available
in `iselenium` (including [bUnit-style
Find()](https://bunit.dev/docs/verification/verify-markup.html#finding-nodes-with-the-find-and-findall-methods)
methods retrieving a Selenium
[`IWebElement`](https://www.selenium.dev/selenium/docs/api/dotnet/html/T_OpenQA_Selenium_IWebElement.htm)
instead of an AngleSharp `INode` from bUnit). Here's an example from
`WithStaticTest`:

```csharp
Find("ul").MarkupMatches(
    @"<ul>
        <li>a first content line</li>
        <li>a second content line</li>
      </ul>");
```

### The `BUnitTestContext`

Without a browser, session persistence is not possible in the context of bUnit
tests - but if the component under test is configured as persistent, it uses the
corresponding services. Therefore the `BUnitTestContext` registers these
formallly. An `IWebHostEnvironment` mock is created by
[Moq](https://github.com/moq/moq).

That specialized `Bunit.TestContext` also provides some helpers around bUnit's
`blazor:elementReference` to be able to find HTML elements  in the "component
under test" (`cut`) by its Blazor Component `Instance` accessor like this (from
the `asptest.blazor.bunit` example):

```csharp
var cut = RenderComponent<CalculatorComponent>();
cut.Find(Dynamic<Enter>(cut.Instance.calculatorPart).operand).Change("3.141");
cut.Find(cut.Instance.footer.enterButton).Click();
```

Compared to the aspnettest/iselenium idiom with the static `Component` accessor in
the test fixture class itself:

```csharp
Navigate("/");
this.Write(Dynamic<Enter>(Component.calculatorPart).operand, "3.141");
Click(Component.footer.enterButton);
```


## Double the test speed with whitebox tests

Assertions with bUnit and Selenium can be performed on two levels: either as
usual on the rendered HTML or directly as assertions on the underlying model.
The first one shall be called "blackbox tests", the second one "whitebox tests".

The `BlazorApp1BunitTest` and `BlazorApp1SeleniumTest` projects in the
`aspnettest.template.blazor` package contain both variants for the Count button.
The difference is both in bUnit and Selenium whether the assertion happens in

- Blackbox: `Find("#countP").MarkupMatches($...`
- Whitebox: `Main.CurrentCount`

The performance measures in tabular form show, that the performance can roughly
be doubled by omitting the 2nd browser/component round trip for the assertion
immediately after the click (1st round trip):

|              | bUnit (5000) | Selenium (100) |
|--------------|--------------|----------------|
| Blackbox     |   **2.8 s**  |   **6.1 s**    |
| Whitebox     |   **1.4 s**  |   **3.6 s**    |

<table>

<tr><th>BlazorApp1BunitTest</th><th>BlazorApp1SeleniumTest</th></tr>

<tr><td>

```csharp
private const int COUNT_NUMBER = 5000;

[Test]
public void CountBlackboxTest()     // 2.8 Sek.
{
    var cut = RenderComponent<Counter>();
    cut.Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

    // Click multiple times and verify that the HTML contains the current i
    for (int i = 1; i <= COUNT_NUMBER; i++)
    {
        cut.Find("#incrementButton").Click();
        cut.Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>Current count: {i}</p>");
    }
}

[Test]
public void CountWhiteboxTest()     // 1.4 Sek.
{
    var cut = RenderComponent<Counter>();
    Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(0));

    // Click multiple times and verify that the model object contains the current i
    for (int i = 1; i <= COUNT_NUMBER; i++)
    {
        cut.Find("#incrementButton").Click();
        Assert.That(cut.Instance.Main.CurrentCount, Is.EqualTo(i));
    }
}
```

</td><td>

```csharp
private const int COUNT_NUMBER = 100;

[Test]
public void CountBlackboxTest() // duration="6.112222"
{
    Assert.That(Main.CurrentCount, Is.EqualTo(0));
    Find("#countP").MarkupMatches("<p diff:ignoreAttributes>Current count: 0</p>");

    // Click multiple times and verify that the HTML contains the current i
    for (int i = 1; i <= 100; i++)
    {
        Click(Cut.incrementButton);
        Find("#countP").MarkupMatches($"<p diff:ignoreAttributes>Current count: {i}</p>");
    }
}

[Test]
public void CountWhiteboxTest() // duration="3.602152"
{
    Assert.That(Main.CurrentCount, Is.EqualTo(0));

    // Click multiple times and verify that model object contains the current i
    for (int i = 1; i <= 100; i++)
    {
        Click(Cut.incrementButton);
        Assert.That(Main.CurrentCount, Is.EqualTo(i));
    }
}
```

</td></tr>

</table>

While pure whitebox test assertions are significantly faster, they also make the
timing brittle due to a race condition: The `OnAfterRenderAsync` synchronization
event may fire a slightly out of sync, causing the assertion to fail. A 2nd
browser round trip for the assertion (with an implicit Selenium WebDriverWait)
will ensure proper synchronization.

This screencast of `aspnettest.template.blazor` illustrates the Count button
speedup (ca. sec 12/18):

![Blazor Template BlazorApp1.playlist](img/template.blazor-running.gif)


## The Test Explorer test runner

The example runner `minimaltestrunner.blazor` runs as ordinary unit test in the
Visual Studio Text Explorer, but contains besides a `Setup()/TearDown()` pair
only one test method `RunTest()`:

```csharp
[TestFixture]
[Category("ITestServer")]
public class Runner : SeleniumTest<EdgeDriver>, ITestServer
{
    public List<Process> ServerProcesses { get; set; }

    [SetUp]
    public void SetUp()
    {
        this.StartServer();
    }

    [TearDown]
    public void TearDown()
    {
        this.StopServer();
    }

    [Test]
    public void RunTests()
    {
        this.Navigate("/", pause: 200); // allow the testButton time to render
        this.ClickID("testButton");
        this.AssertTestsOK();
    }
}
```

...that is run run with this configuration:

```js
{
  "ServerProject": "minimal.blazor.csproj",
  "Root": "..\\..\\..\\..\\minimal.blazor",
  "Port": "5000",
  "RequestTimeout": "100",
  "ServerStartTimeout": "10"
}
```

The extension methods of `ITestServer` provide a `StartServer()`/`StopServer()`
pair which start a Kestrel server process in the project directory ("`Root`").
The base class `SeleniumTest<EdgeDriver>` runs the chosen Browser and clicks the
`testButton`. This button starts the nested in-process test within the web
server process, which in turn starts another browser instance with Selenium.

In the current implementation, the test can only succeed or fail as a whole. It
doesn't seem to be possible to reload the tests actually running in-process
kinda dynamically in the Test Adapter for the Test Explorer - therefore it only
shows the XML test result that's also shown in the Browser itself (where no Test
Explorer is available at all).

The outer Selenium test (the test runner) is effectively classic (running out-of
process), therefore it has to pause some time span after `Navigate()` to allow
Blazor's client side rendering to complete and then to click the test button by
its HTML DOM `id`, not by instance.

The inner Selenium test on the other side runs in-process and therefore has
access to the element instances and the `AutoResetEvent` synchronization
mechanism presented here. This diagram shows the process boundaries:

![In- and out-of-process Selenium tests](blazor-inoutprocess.png)

It would be preferable to run the Kestrel server in-process from within the
Visual Studio Test adapter process, but Blazor Server applications can't even be
run from the ./bin/../app.exe executable (resources from a Blazor component
library are not found), it requires project-level `dotnet run` for a working
environment.