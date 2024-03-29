# Initial Setup

The Visual Studio solution requires some initial tweaking to get it up and running:

* [Clone and tweak the solution](#clone-and-tweak-the-solution)
* [Optional rebuild dependencies](#optional-rebuild-dependencies)
* [NuGet package restore](#nuget-package-restore)
  * [NuGet-only binary distribution](#nuget-only-binary-distribution)
* [SQL database creation](#sql-database-creation)
* [Running the unit tests](#running-the-unit-tests)
* [Running the web application self tests](#running-the-web-application-self-tests)
* [Let it rattle](#let-it-rattle)


## Clone and tweak the solution

 ```git clone git@github.com:toniarnold/aspnettest.git```

Open ```aspnettest.sln``` (the full solution) with Visual Studio 2017. This
step will create the hidden ```\.vs\config``` directory within the
```aspnettest``` root folder containing the ```applicationhost.config```
configuration file where the virtual directories for the two ASP.NET WebForms
web site projects are auto-configured with their respective ports. Open it with
a text editor and find for the relevant ```<bindings>``` by searching for
```:localhost```, as this is exactly the culprit:

```xml
<bindings>
    <binding protocol="http" bindingInformation="*:51333:localhost" />
</bindings>
```

Replace ```localhost``` with its IP address ```127.0.0.1``` in both websites,
otherwise Internet Explorer will not work with your site:

```xml
<bindings>
    <binding protocol="http" bindingInformation="*:51333:127.0.0.1" />
</bindings>
```

In case something went wrong with the configuration and e.g. IIS Express
doesn't start no more, the canonical solution seems to be to manually prune the
whole  ```\.vs\``` directory.

If not already present, Visual Studio will download and install the components
required for Visual C++ Makefile projects, as I considered ```nmake``` as the
simplest tool for handling SMC state machine generation - there is no C++
involved.

If you don't *re*-build the solution, the generated code files are already
present and ```nmake``` will not run.


## Optional rebuild dependencies

For a complete rebuild from source, the Makefiles in the solution expect these additional binaries
present in ```%PATH%```:

* [```java.exe```](https://www.oracle.com/java/index.html) Java runtime for executing ./SMC/Smc.jar
  in order to generate .cs and .dot files from the portable .sm state machine source.
* [```dot.exe```](https://www.graphviz.org/download/) Graphviz to generate the
  splash images from .dot files.
* [```dia.exe```](http://dia-installer.de) GNU Dia for generating .png images from
  the .dia diagrams in ./doc


## NuGet package restore

The whole project heavily depends on NUnit 3, and the database persistency
layer is built on Entity Framework 6 (EF6, with the "Database First" paradigm),
so you'll need to perform a NuGet Package Restore.

Now you should be able to build the solution with the Any CPU Debug
configuration.


### NuGet-only binary distribution

There are four rather experimental binary packages available on NuGet, one web
application base / test base project for each Framework:

* [aspnetsmc.webforms](https://www.nuget.org/packages/aspnetsmc.webforms/)
* [aspnettest.webforms](https://www.nuget.org/packages/aspnettest.webforms/)
* [aspnetsmc.core](https://www.nuget.org/packages/aspnetsmc.core/)
* [aspnettest.core](https://www.nuget.org/packages/aspnettest.core/)

If these are installed into the source solutions, there will be DLL hell. But
all four projects make little sense outside of a complete web application,
therefore there are two solutions with the minimal application (without SMC)
for each framework referencing none of the source packages:

* ```minimal.nuget.webforms.sln```
* ```minimal.nuget.core.sln```

To use these, clone the ```aspnettest``` source tree into a separate folder and
never open and compile one of the source solutions there such that the NuGet
packages retain their monopoly to provide the respective DLLs. These two
minimal solutions contain only the web application and the test DLLs, but no
library projects:

![Minimal WebForms NuGet Solution](img/minimal.nuget.webforms.png)
![Minimal MVC Core NuGet Solution](img/minimal.nuget.core.png)


#### COM Dependencies

As COM dependencies of NuGet packages are not transitively inherited by the
target projects, they have been added manually to the respective projects (for
.NET Core, there is not even VS GUI support to create new COM dependencies).
But for the WebForms project, these DLLs additionally need to be copied
manually to the ```.\minimal.webforms\bin``` directory - e.g. from *another*
checkout built from source:

```bash
cp Interop.MSHTML.dll .\minimal.webforms\bin
cp Interop.SHDocVw.dll .\minimal.webforms\bin
```


#### Missing EF6 sources
You can directly copy the missing EF6 sources for the concrete tables from the root directory
without modifications into the ```minimaltest.webforms``` project:

```bash
cp .\asplib.webforms\Model\Db.cs  .\minimaltest.webforms\Model\
cp .\asplib.webforms\Model\Db.Context.cs .\minimaltest.webforms\Model\
```



## SQL database creation

There is an SQL Server Management Studio 17 solution ```db.ssmssln```
with the scripts, the database name is ```[ASP_DB]```. Execute these:

1. ```Main.sql``` -- DDL that creates the table for session storage
2. ```asptest.calculator.FibonacciTest.sql``` -- DML for that test case with
   a sample session dump

Adjust this connection string (the user iis/pass on my development machine is
no big secret):

```xml
<connectionStrings>
<add name="ASP_DBEntities" connectionString="metadata=res://*/Model.Db.csdl|res://*/Model.Db.ssdl|res://*/Model.Db.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(local);initial catalog=ASP_DB;persist security info=True;user id=iis;password=pass;multipleactiveresultsets=True;application name=EntityFramework&quot;" providerName="System.Data.EntityClient" />
</connectionStrings>
```
...in these three .config files:

* ```.\asp\Web.config``` The full web application requiring SMC
* ```.\minimal\Web.config``` The minimized web application
* ```.\test\App.config``` The unit test suite outside the web applications


## Running the unit tests

"A unit test talking to a database is not an unit test!" - well, then call it
shallow integration tests or whatever, the point is that the NUnit test suite
in the ```test``` project is intended to run within Visual Studio, while the
two functional test suites in the ```asptest``` resp. ```minimaltest``` projects
can only succeed when called from the respective web application itself.

Therefore open  ```test.playlist``` within the Test Explorer, which contains
all the tests in only the ```test``` project. They should all succeed, and
don't mind the empty Internet Explorer window popping up just for the blink of
an eye...


## Running the web application self tests

It seems that in Windows 10 there is no simple way to grant the web application
pool identity the right to open Internet Explorer (on Windows 7 it could at
least use it, but it had no right to open its GUI), so the web application self
tests need to be run in a Visual Studio debugging session.

As non-excluded exceptions interrupt the test procedure, I recommend to first
debug the ```minimal``` web application project. It will open a sparse web site
with a big green test button in the upper right corner:

![minimal main page](./img/minimal.png)

Clicking on it will run the ```minimaltest``` suite, but it soon will interrupt
the debugger first with a ```InvalidOperationException```, then with a
```TestException```. Configure the Visual Studio debugger to not interrupt
execution at least for this specific types of exception from the respective
assemblies:

![interrupting break](./img/break.png)

While at it, you can open the exception settings and exclude asp.dll from
interrupting on ```TestException```, too.

Now the tests should pass trough. In case of a failure, the default page (from
which the tests were started) will display a TestResult.xml with the details of
the problem:

![test failure](./img/failure.png)


## Let it rattle

Once the minimalist test setup passes without break, chances are high that the
```asptest``` project (run from the ```asp``` startup project) will pass, too.
Expect 1-2 minutes for all tests. 

Take care not to move the mouse pointer over the flashing Internet Explorer
instance, as it will interfere with the ASP.NET client side JavaScript and
cause some tests to fail. Just placing it in a side corner of the screen while
waiting works best. Also continue with F5 instead of mouse clicks when the
debugger stops at an exception.
