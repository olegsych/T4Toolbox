# T4 Toolbox

T4 Toolbox extends the code generation functionality of the [T4](https://docs.microsoft.com/en-us/visualstudio/modeling/code-generation-and-t4-text-templates) 
text templates in Visual Studio and allows you to:
- Generate multiple output files from a single text template 
- Automatically add output files to one or more projects and folders 
- Automatically add or check-out generated files from source control 
- Edit text templates in Visual Studio 2015 with syntax colorization, outlining, QuickInfo tooltips, 
error reporting and statement completion 

You can install the latest stable version of T4 Toolbox using the Extension Manager in Visual Studio or download 
it from the [marketplace](https://marketplace.visualstudio.com/search?term=T4Toolbox&target=VS).
To learn more, visit the [Getting Started](http://olegsych.github.io/T4Toolbox/getting-started.html) wiki page.

## Building T4 Toolbox 

[![Build status](https://ci.appveyor.com/api/projects/status/github/olegsych/t4toolbox?branch=master&retina=true)](https://ci.appveyor.com/project/olegsych/t4toolbox/branch/master)

Pre-requisites
- [Visual Studio 2017](https://www.visualstudio.com/downloads), 
  - Edition: Community
  - Toolsets:
    - .NET desktop development toolset
    - Visual Studio extension development
- .NET 3.5 (for Lex and Yacc tools)
- [Visual Studio Color Theme Editor](https://marketplace.visualstudio.com/search?term=%22Color%20Theme%20Editor%22&target=VS) 
  (optional, for editing .vstheme and .pkgdef files)

Open `T4Toolbox.sln` in Visual Studio and build it. The NuGet packages the solution depends on should be restored
[automatically](http://docs.nuget.org/Consume/Package-Restore) by Visual Studio. If you have disabled the automatic
NuGet package restore in Visual Studio options, you can restore them manually by launching the NuGet Package Manager 
from the Solution Explorer.

The `Debug` configuration of the solution builds fast. It builds in seconds and you'll want to use it most of the 
time to stay productive.

The `Release` configuration of the solution builds slow because it runs FxCop and StyleCop checks. It can take a minute 
or more to build and you'll want to avoid it while working with tests, because Test Explorer builds the entire solution 
for every test run. 

Build the `Release` configuration before submitting pull requests.

## Running Tests

The unit test projects (*.Tests.csproj) contain close to a thousand tests. They are fast, but you'll want to run them 
separately from the integration tests to keep the test runs under 10 seconds. For that:
- Enter the following filter in the Test Explorer: `-Project:Integration` (the dash is important).
- Ensure that _no_ test settings file is selected in the `Test` / `Test Settings` menu of Visual Studio.

The integration test projects (*.IntegrationTests.csproj) contain close to a hundred tests. They have to be executed by 
a special test host inside of the Experimental instance of Visual Studio. The test host is slow, and you'll 
want to exclude the unit tests to keep the integration test runs under a minute. For that:
- Enter the following filter in the Test Explorer: `Project:Integration` (there should be no dash).
- Select `LocalTestRun.testsettings` in the `Test` / `Test Settings` menu of Visual Studio.
- At the start of a new test run, you may have to click through the Visual Studio sign-in and initial setup pages.

## Debugging

You can debug unit and integration tests by selecting the Debug commands in Test Explorer. 

Alternatively, you can debug both generator and editor code by running it in Visual Studio. For that, set the 
`T4Toolbox.vsix` project as the startup project in Solution Explorer and specify the following options on the 
Debug page of the project properties.
- Start Action / Start external program: ```C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe```
- Start Options / Command line arguments: ```/rootSuffix Exp```

With these settings in place, building the solution will automatically install the extension in the 
experimental hive of Visual Studio and starting the debugging will automatically launch an experimental instance of Visual 
Studio and attach your debugger to it.
