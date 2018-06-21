# Architecture

1. [Components](#components)
2. [Basic Internet Explorer Interaction](#Basic-Internet-Explorer-Interaction)
3. [IIE/IEExtension in Detail](#IIE/IEExtension-in-Detail)
   * [Component Environment of IIE](#Component-Environment-of-IIE)
   * [Using IIE in tests](#Using-IIE-in-tests)
   * [ASP.NET pages with IIE](#ASP.NET-pages-with-IIE)



## 1. Components

The various projects are distributed over the components in the dependency tree below. 
The dotted box on the left hand side confines the NUnit address space when the tests are run
within Visual Studio, the box on the right hand side confines the ASP.NET address space
when the functional tests are started from the web application itself.

Both test variants verify the same web application with its libraries, therefore that
component appears in both boxes - but of course not at the same time.
 
Note the cyclic dependency the ```testie``` assembly takes part in:
The requirement is that all components live in the same ASP.NET address space,
thus ```asp``` needs to start (i.e. depends on) ```testie```, but ```testie```
of course also depends on ```asp``` as the application under test.

The cycle is broken up by providing the ```ITestRunner``` (started by ```asp```)
only with the late-bound physical path to the ```testie.dll``` file to load the test
assembly ultimately with a call to ```Assembly.Load()```.

![Components](components.png)


## 2. Basic Internet Explorer Interaction

Opening an URL in Internet Explorer works the same way for all depth levels
of interaction between the various components. This is the basic call sequence:

![iInternet Explorer Sequence](internet-explorer.png)


## 3. IIE/IEExtension in Detail

This is a slightly abbreviated description of the static structure
for all depth levels of interaction between test engine, Internet Explorer
and the ASP.NET web application under test. 


### Component Environment of IIE

![IIE/IEExtension Components](iie-component.png)

The SMC project dependency is of course only required for the ```Smc``` classes and interfaces.
Including SMC as a source project instead of a pre-built binary of the ```FSMContext```
is only due to flexibility in architecture and self-documentation, the sources are unmodified 
except some code analysis warning suppression.



### Using IIE in tests

The test specialized engine itself is implemented in the ```IEExtension``` class.
The pure marker interface ```IIE```  is used to pull in the extension methods and  static fields 
of the ```IEExtension``` into the respective test fixture.

The four concrete test fixture classes on the left hand side are the same as in the
four beginning sections of the [Writing GUI Tests](writing.md) document, in ascending order
of specificity and extent of functionality provided. A deeper level requires everything 
from the upper level and adds more to it.


![IIE/IEExtension Tests](iie-test.png)



### ASP.NET pages with IIE

Again, the four concrete ```System.Web.UI``` classes on the right hand side reflect
the four beginning sections of the [Writing GUI Tests](writing.md) document, parallel
to above test fixtures.

![IIE/IEExtension Pages](iie-page.png)
