# Screen Recordings

* [Synopsis in the README](#synopsis-in-the-readme)
* [Core Dump on unhandled Exceptions](#core-dump-on-unhandled-exceptions)
* [Start a test in a pre-stored state (Fibonacci Sequence)](#start-a-test-in-a-pre-stored-state-fibonacci-sequence)


## Synopsis in the README

This is the test filter used to run this subset of the GUI tests:

```xml
<!-- For TestFilterBuilder.SelectWhere(), e.g. value="class == testie.asp.ExceptionDumpTest" -->
<add key="TestFilterWhere" value="class == testie.asp.calculator.CalculateTest ||
                                    class == testie.asp.calculator.SessionGridViewTest ||
                                    class == testie.asp.TriptychTest" />
```

![Tests running...](img/running.gif)


## Core Dump on unhandled Exceptions

```xml
<add key="TestFilterWhere" value="class == testie.asp.ExceptionDumpTest" />
```

![ExceptionDumpTest in action](img/ExceptionDumpTest.gif)


## Start a test in a pre-stored state (Fibonacci Sequence)

```xml
<add key="TestFilterWhere" value="class == testie.asp.calculator.FibonacciTest" />
```

![FibonacciTest in action](img/FibonacciTest.gif)