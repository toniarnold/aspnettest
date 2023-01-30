# Changelog

## [0.2.1]

* Blazor: Fix issue #5 race condition in the `PersistentMainFactory` due to the
  static ad hoc cache by setting up a request correlation guid in `_Host.cshtml`

* Blazor: Deprecate the `Component` accessor in favor of the apparently
  canonical `Cut` for "component under test"

* Blazor: Synchronization in `Navigate()` and `Refresh()` now with the same
  arguments as `Click()` with int `expectRenders` instead of bool `expectRender`
  (breaking change)

* Blazor: Support the `?clear=true` GET argument to clear the storage

* WebForms and Core examples: Internet Explorer has been forcibly disabled by
  Microsoft, thus remove it from old tests

* Set Selenium `RequestTimeout` to 1 sec if not configured

* Add Source Link to the NuGet packages


## [0.2.0] - 2022-12-23

* Replaced direct Internet Explorer COM Interop with Selenium (also with IE)

* Split library into asplib and iselenium

* Added Blazor Server as web framework with bUnit

* Updated to .NET Framework 4.6.2 (WebForms) and .NET 6.0 (Core, WebSharper,
  Blazor)
