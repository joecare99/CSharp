# RnzTrauer migration workspace

`RnzTrauer.slnx` contains the complete workspace, including legacy website
projects that require the .NET Framework MSBuild toolchain.

For the portable C# test suite, use `RnzTrauer.Tests.slnx`. It deliberately
contains only the SDK-style .NET test projects and their database abstraction
dependencies:

```powershell
dotnet restore .\RnzTrauer.Tests.slnx
dotnet test .\RnzTrauer.Tests.slnx --no-restore -p:TargetFramework=net8.0
```

The test solution does not connect to a live database. Provider tests must use
fakes or command/SQL assertions; live database tests require an explicit
environment-specific command and isolated test data.

The explicit `net8.0` target keeps the baseline deterministic while the
workspace contains optional higher-target-framework projects. The full
`RnzTrauer.slnx` must not be used for this command because it also contains
legacy ASP.NET website projects that require the .NET Framework MSBuild
toolchain.

The shared `Gen_FreeWin` projects currently emit `MSB3539` because
`GenFreeWin.props` changes `BaseIntermediateOutputPath` after the SDK has
initialized MSBuild. This is retained as a separate build-infrastructure
cleanup item; the renderer contract tests do not depend on that warning or on
any live database.
