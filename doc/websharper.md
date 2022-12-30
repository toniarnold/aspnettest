# WebSharper with SMC

* [Summary](#summary)
* [Controller persistence](#controller-persistence)
* [State driven document part visibility](#state-driven-document-part-visibility)

*IntellyFactory seems to have deprecated WebSharper in favor of Bolero (Blazor
Server in F#, see the [WebSharper Blog](https://forums.websharper.com/blogs)),
and asp.websharper.spa.fs now even crashes wsfsc.exe ([Bug #5](/../../issues/5)).
Anyway, they have finally released a version for .NET 6.0 in July 2022.*

## Summary

"Give me back the ViewState from ASP.NET WebForms for modern SPA applications"
was the main intention for the WebSharper project. It turned out that this was
indeed possible, but the testing infrastructure with Internet Explorer
specifically can't handle WebSharper's DOM manipulations.

The compositional nature of WebSharper matches user control composition in
ASP.NET WebForms, therefore it was also possible to mirror the `tryptich.aspx`
page in `asp.websharper.spa.Client.TriptychDoc`. This seems not to be possible
with Razor Pages in a strictly compositional way.


## Controller persistence

The `asp.websharper.spa` project shares the `asplib`/`asplib.core` dependency
and the services setup with the `asp.core` MVC project. The `Session` and
`Database` persistence mechanism don't require any collaboration with the View
and therefore are transparently available for WebSharper, too.

But `ViewState` persistence requires storage space on the client side. The
abstract `asplib.Model.ViewModel` C#/JavaScript class provides that space with
its `ViewState` member (transported forth and back via JS-Remoting) and its
C#/.NET `SerializeMain()`/`DeserializeMain()` methods (not transpiled to JS,
only callable on the server side). 


## State driven document part visibility

Unlike the pattern used in ASP.NET WebForms / ASP.NET MVC Core, there is no easy
place for a central `switch` statement which coverns document part visibility.

The descendants of above `asplib.Model.ViewModel` class provide access to the
strongly typed state names in the concrete `CalculatorViewModel` class.
Each document part (which is itself a document) gets the global application
state from its ViewModel and displays itself when the state matches, otherwise
it returns just a `WebSharper.UI.Doc.Empty`. Each state name string is made
accessible with a WebSharper `Map1` lambda expression.

The Error document is visible in three distinct error states with a different
message and otherwise terse, thus appropriate as an example. This C# code is
never executed in C# itself, but transpiled to JavaScript by the WebSharper
engine and executed by the browser:

```cscharp
public static object ErrorDoc(View<CalculatorViewModel> viewCalculator) =>
    V(viewCalculator.V.State).Map(state =>
    V(viewCalculator.V.Map1.ErrorEmpty).Map(empty =>
    V(viewCalculator.V.Map1.ErrorNumeric).Map(numeric =>
    V(viewCalculator.V.Map1.ErrorTuple).Map(tuple =>
    {
        return (
            state == empty ||
            state == numeric ||
            state == tuple
            ) ?
            new Template.Error.Main()
                .Msg(
                    (state == empty) ? "Need a value on the stack to compute." :
                    (state == numeric) ? "The input was not numeric." :
                    (state == tuple) ? "Need two values on the stack to compute." :
                    "")
                    .Doc()
            : WebSharper.UI.Doc.Empty;
    }))));
```