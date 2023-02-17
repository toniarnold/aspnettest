# SpecFlow integration

The Blazor template and asp.blazor contaion a [SpecFlow](https://specflow.org)
test suite performing some of the same tests as the direct Selenium tests suite.

The WebForms example (`asptest.webforms.specflow`) was originally created with
VS 2019 (the VS SpecFlow 2022 package creates C# with `global using` incompatible with
.NET Framework). It follows the same pattern as the Blazor example, it compiles,
but it currently does not run (although the required
`TechTalk.SpecFlow.NUnit.SpecFlowPlugin.dll` is present in the `bin` directory):

`Interface cannot be resolved: TechTalk.SpecFlow.UnitTestProvider.IUnitTestRuntimeProvider('nunit')`

The libraries (NuGet packages) themselves have no SpecFlow dependency, only the
applications and tests which are actually  using it.