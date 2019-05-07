# Screen Recordings

**_[Bug #3](/../../../issues/3): Due to (probably) Win10 update 1809, Internet Explorer visibility is disabled in `SetUpIE()`,
as it cannot run no more in the foreground._**

* [Synopsis in the README](#synopsis-in-the-readme)
* [Core Dump on unhandled Exceptions](#core-dump-on-unhandled-exceptions)
* [Start a test in a pre-stored state (Fibonacci Sequence)](#start-a-test-in-a-pre-stored-state-fibonacci-sequence)


## Synopsis in the README

This is the test filter used to run this subset of the GUI tests:

```xml
<!-- For TestFilterBuilder.SelectWhere(), e.g. value="class == asptest.ExceptionDumpTest" -->
<add key="TestFilterWhere" value="class == asptest.calculator.CalculateTest ||
                                    class == asptest.calculator.SessionGridViewTest ||
                                    class == asptest.TriptychTest" />
```

![Tests running...](img/running.gif)


## Core Dump on unhandled Exceptions

```xml
<add key="TestFilterWhere" value="class == asptest.ExceptionDumpTest" />
```

![ExceptionDumpTest in action](img/ExceptionDumpTest.gif)


## Start a test in a pre-stored state (Fibonacci Sequence)

```xml
<add key="TestFilterWhere" value="class == asptest.calculator.FibonacciTest" />
```

![FibonacciTest in action](img/FibonacciTest.gif)