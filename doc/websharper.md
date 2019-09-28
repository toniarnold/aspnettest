# WebSharper with SMC

"Give me back the ViewState from ASP.NET WebForms for modern SPA applications"
was the main intention for the WebSharper project. It turned out that this was
indeed possible, but the testing infrastructure with Internet Explorer
specifically can't handle WebSharper's DOM manipulations.

The compositional nature of WebSharper matches user control composition in
ASP.NET WebForms, therefore it was also possible to mirror the `tryptich.aspx`
page in `asp.websharper.spa.Client.TriptychDoc`. This seems not to be possible
with Razor Pages in a strictly compositional way.
