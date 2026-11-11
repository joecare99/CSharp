# AvlnCodeStudio release acceptance

This is the manual release gate for the integrated Workbench and its standalone
designer host. Run the test suite from the solution root; run desktop smokes in
an interactive session on each supported host. A passing automated smoke does
not replace keyboard, screen-reader, or other physical accessibility testing.

## Target framework support policy

As of November 11, 2026, Microsoft's support for .NET 8 and .NET 9 ended on
November 10, 2026. .NET 10 LTS is supported through November 14, 2028. The
net8.0/net9.0 results below are compatibility evidence only, not a claim that
those runtimes remain security-supported. The project still targets all three;
do not remove older targets until the product's compatibility policy is
explicitly decided.

See the [official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
and [Microsoft lifecycle table](https://learn.microsoft.com/en-us/lifecycle/products/microsoft-net-and-net-core).

## Automated solution tests

```powershell
dotnet test .\AA98_AvlnCodeStudio.slnx --framework net10.0
```

The first run restores packages. Do not add `--no-restore` unless the complete
solution has already been restored for the selected target framework. The
solution's Terminal host tests target net10.0 only. For the four multi-target
test projects, run net8.0/net9.0 explicitly:

```powershell
$testProjects = @(
  '.\AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj',
  '.\AA98_AvlnCodeStudio.Base.AI.Tests\AA98_AvlnCodeStudio.Base.AI.Tests.csproj',
  '.\AA98_AvlnCodeStudio.Editor.Tests\AA98_AvlnCodeStudio.Editor.Tests.csproj',
  '.\AA98_AvlnCodeStudio.Config.Secrets.Tests\AA98_AvlnCodeStudio.Config.Secrets.Tests.csproj'
)
foreach ($framework in @('net8.0', 'net9.0')) {
  foreach ($project in $testProjects) {
    dotnet test $project --framework $framework --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw "Test failed: $project ($framework)" }
  }
}
```

Keep Linux `bin`/`obj` outputs isolated from Windows outputs when both hosts use
the same checkout.
For a shared Windows/WSL checkout, pass an early-import isolation props file
and a separate `LinuxConfigArtifactsRoot`, as described in the Workbench MVVM
smoke README. Keep restore and test in one WSL invocation when the artifact
root is ephemeral.

## Interactive desktop smokes

Run each applicable smoke independently and require its success message and a
zero process exit code:

```powershell
dotnet run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj
dotnet run --project tools\WorkbenchMvvmSmoke\WorkbenchMvvmSmoke.csproj
dotnet run --project tools\ProjectSettingsReviewSmoke\ProjectSettingsReviewSmoke.csproj
```

These cover integrated designer and project-settings workflows using temporary
fixtures. The architecture smoke also opens the standalone designer host. See
the individual tool documentation for detailed coverage and platform setup.

The project-debug smoke additionally requires a trusted NetCoreDbg installation:

```powershell
$env:AVLN_NETCOREDBG_PATH = "C:\path\to\netcoredbg.exe"
dotnet run --project tools\WorkbenchDebugSmoke\WorkbenchDebugSmoke.csproj
```

On Linux, point `AVLN_NETCOREDBG_PATH` at the Linux adapter and run in a
supported desktop session. The smoke exercises project selection, MSBuild
inspection, build, runtime refresh, DAP launch, breakpoints, stack/locals,
stepping, continue, and stop. A missing adapter is a blocked platform result,
not a pass.

## Release evidence and open gates

- Record OS, SDK, target framework, adapter version (for DAP), command, exit
  status, and test counts for each run. Keep Windows and Linux results separate.
- The Windows .NET 10 solution run completed with 747 passed, 5 skipped, and
  0 failed tests across five test assemblies. A first `--no-restore` attempt
  could not find cached Avalonia 12.1.4 for `AA98.Terminal.Host.Tests`; the
  normal restore-and-test run succeeded.
- The four multi-target test projects completed on Windows net8.0 and net9.0
  with 711 passed, 5 skipped, and 0 failed tests per framework.
- .NET 8 and .NET 9 are end-of-support; their successful test runs demonstrate
  compatibility only. Confirm the supported-target policy before release.
- The isolated WSLg/Linux .NET 10 solution run completed with 745 passed,
  7 skipped, and 0 failed tests across the same five test assemblies. Its
  artifacts were written under a separate `/tmp` root.
- Linux net8.0/net9.0 remain unverified; this WSL installation currently has
  only .NET 10 runtimes.
- The Workbench architecture, MVVM, and project-settings desktop smokes exited
  successfully on Windows. All three flows also have prior successful
  WSLg/Linux acceptance records.
- The real project-debug flow has passed under WSLg/Linux with .NET 10 and
  NetCoreDbg 3.2.0-1. The Windows DAP run remains unverified until a trusted
  Windows adapter is available.
- Workbench and standalone host smokes use programmatic UI interaction. Perform
  and record physical keyboard and screen-reader acceptance separately.
- Launch profiles remain read-only and are not loaded or executed by these
  smokes. No claim is made about profile argument/environment handling, secrets,
  or execution of a real user project.
- The solution run reports `NU1701` for legacy .NET Framework packages
  (`Microsoft.Build` and `Microsoft.IO.Redist`) and `MSTEST0044` for a
  deprecated `DataTestMethod`. Linux output also includes inherited compiler
  and analyzer warnings from referenced builder projects. Track these
  separately; do not treat warning-free compilation as a substitute for the
  behavioral gates above.
