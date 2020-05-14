# Migrate from IIE to ISelenium

* [Add Selenium NuGet packages](#add-selenium-nuget-packages)
   *  [For a specific browser ...](#for-a-specific-browser)
   *  [... or Edge](#or-edge)
      *  [Abandoning ```Interop.SHDocVw```](#abandoning-interop.shdocvw)
   *  [Selenium.WebDriver for ASP.NET Framework tests](#selenium.webdriver-for-asp.net-framework-tests)
* [Exchange project references](#exchange-project-references)
   * [Replace using directives](#exchange-using-directives)
* [Generic test classes](#generic-test-classes)
* [Fixed compatibility caveats](#fixed-compatibility-caveats)


## Add Selenium NuGet packages

### For a specific browser ...

While for IIE, the COM references to ```Interop.MSHTML``` and
```Interop.SHDocVw``` were only required in the library itself, some NuGet
packages must be added to the web/test projects, too (similar to NUnit itself).

The ISelenium library only references the generic ```Selenium.WebDriver``` to
communicate with the browser. Therefore, thhe concrete WebDriver NuGet package
is required for the web project, either
```Selenium.InternetExplorer.WebDriver``` (32 bit),
```Selenium.Firefox.WebDriver``` or ```Selenium.Chrome.WebDriver```. 

### ... or Edge

The case for Edge is different, as the WebDriver is integrated into the win10
operating system as an official Windows Feature on Demand. Go to Apps in the
win10 Settings, click on Optional Features and Add Feature, then look for the
"Microsoft WebDriver" and install it.

Additionally, Developer Mode must be enabled. Go to Settings > Update and
Security > For Developer and then select "Developer mode".

Even more, in Edge itself, access to localhost must be enabled. Type
```about:flags``` in the URL bar and enable the "Localhost-Loopback".

#### Abandoning ```Interop.SHDocVw```

However, this will not enable the now blocked http://127.0.0.1 (to ptotect local
webservices from malicious CORS requests from a remote web site, see e.g.
[Hacker News - I can see your local web
servers](https://news.ycombinator.com/item?id=20028108)). No http://127.0.0.1 no
more is the main reason why IIE with direct COM communication is now abandoned,
although it was by far the fastest test driver. No more [tweak the
solution](setup.md#clone-and-tweak-the-solution) because "localhost" seems to
never reach ```OnDocumentComplete``` (in ```iie.IEExtension```). Its inability
to communicate with SPA pages like the minimal.websharper.spa was another big
reason.

### Selenium.WebDriver for ASP.NET Framework tests

Unlike for .NET Core, the NUnit test project for ASP.NET Framework also
requires the generic ```Selenium.WebDriver``` to be installed in the project
itself, the indirect reference via ```iselenium.webforms``` is not enough. 

## Exchange project references

Delete the ```iie.webforms``` resp. ```iie.core``` and the parent ```iie```
project references and replace them with ```iselenium.webforms``` resp.
```iselenium.core``` in both the web project and its corresponding NUnit test
project. For .NET core, the parent ```iselenium``` reference is only needed in
the web project itself.


### Replace using directives

After the ```iie``` references have been removed, Visual Studio will
automatically complain - it should suffice to replace

```chsarp
using iie;
```

with 

```chsarp
using iselenium;
```

in all test sources and, in case of .NET Core, in the Controller class for the
page containing the NUnit button to start the test.

For ASP.NET Framework, the ```Register Assembly``` directive in the .ascx file
containing the NUnit button must be adjusted as well. Replace

```html
<%@ Register Assembly="iie.webforms" Namespace="iie" TagPrefix="iie" %>
```

with

```html
<%@ Register Assembly="iselenium.webforms" Namespace="iselenium" TagPrefix="iie" %>
```

The ```TagPrefix="iie"``` can remain as is.



## Generic test classes

If you don’t want to use the obsolete non-generic ```IETest``` base classes for
Internet Explorer, the concrete browser to perform tests with needs to be
declared in the TestFixture. Multiple declarations are allowed, which will
execute all tests with each of the given browsers:

```csharp
    [TestFixture(typeof(EdgeDriver))]
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    [TestFixture(typeof(InternetExplorerDriver))]
```

Make sure to add the corresponding [NuGet packages](#add-nuget-packages) for the browser-specific WebDriver.


# Fixed compatibility caveats

__getElementsByName__

The ```getElementsByName()``` method in an ```IHTMLDocument3``` instance from
```Interop.MSHTML``` implicitly returns an HTMLElement with that id instead of
the given name if the latter is not founc. This behavior has been replicated in
SeleniumExtensionBase.GetHTMLElementByName() (which by itself ignores the id) to
retain compatibility.

__Line Breaks__

Line breaks in a ```MSHTML.IHTMLDocument2.body.outerHTML``` are only LF (\n),
while line breaks returned by Seleniums ```GetAttribute("outerHTML")``` are
Windows-style CR LF (\r\n). To retain compatiblity with hackish test assertions
like ```Assert.That(this.Html(), Does.Contain(" 5\n")```, this has been
mitigated by returning only LF (\n), too. 
