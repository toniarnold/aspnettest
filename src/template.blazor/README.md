# Solution template for aspnettest Blazor Server App

The template to generate a Blazor Server App already wired up with bUnit and
aspnettest Selenium (NuGet packages aspnettest.asplib.blazor and
aspnettest.iselenium.blazor) can be instantiated with:
```sh
dotnet new install aspnettest.template.blazor
dotnet new aspnettest-blazor -o MyBlazorApp
```
If you haven’t already, enable the „Microsoft WebDriver“ in „Apps and Features“
in the Windows Settings. Then open the generated MyBlazorApp.sln with Visual
Studio. Due to the circular dependency (by design) between the app and its
Selenium tests, it is mandatory to build the solution *twice*, the second time
enforced as „Rebuild Solution“. From the Test-Explorer window, open the
generated MyBlazorApp.playlist. It contains two test projects:
`MyBlazorAppBunitTest` (which runs fast) and `MyBlazorAppSeleniumTestRunner`
which starts the web server, an Edge browser instance and runs the tests in
`MyBlazorAppSeleniumTest` (excluded from the playlist) by pushing the test
button in the browser.

Running the `BlazorApp1.playlist`:

![BlazorApp1.playlist running](https://raw.github.com/toniarnold/aspnettest/master/doc/img/template.blazor-running.gif)

Unlike Edge, FireFox doesn't block mouse click events when run by the Selenium
WebDriver. The additional counter increment causes the test to fail:

![Interruption with FireFox](https://raw.github.com/toniarnold/aspnettest/master/doc/img/template.blazor-running-firefox.gif)
