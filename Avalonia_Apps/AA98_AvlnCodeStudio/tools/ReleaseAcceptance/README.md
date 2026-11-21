# AvlnCodeStudio release acceptance

This is the manual release gate for the integrated Workbench and its standalone
designer host. Run the test suite from the solution root; run desktop smokes in
an interactive session on each supported host. A passing automated smoke does
not replace keyboard, screen-reader, or other physical accessibility testing.

## Target framework support policy

As of November 11, 2026, Microsoft's support for .NET 8 and .NET 9 ended on
November 10, 2026. .NET 10 LTS is supported through November 14, 2028. The
net8.0/net9.0 historical results below are not evidence of current runtime
acceptance or security support. The project still targets all three; do not
remove older targets until the product's compatibility policy is explicitly
decided.

See the [official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
and [Microsoft lifecycle table](https://learn.microsoft.com/en-us/lifecycle/products/microsoft-net-and-net-core).

## Automated solution tests

```powershell
.\tools\build\dotnet-isolated.ps1 test .\AA98_AvlnCodeStudio.slnx --framework net10.0
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
    .\tools\build\dotnet-isolated.ps1 test $project --framework $framework --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw "Test failed: $project ($framework)" }
  }
}
```

For WSL/Linux, use the same wrapper and commands, for example:

```bash
bash tools/build/dotnet-isolated.sh test ./AA98_AvlnCodeStudio.slnx --framework net10.0
```

The wrappers set `HostArtifactRoot` and inject `tools/build/IsolatedArtifacts.props`
through `CustomAfterDirectoryBuildProps`, before MSBuild chooses its project
extensions path. This preserves normal directory and project imports, fills in
missing output paths for references that do not import the shared solution
props, and excludes existing project-local `obj` and `bin` from SDK default
items. Defaults are `artifacts/windows` and `artifacts/linux` beneath the
shared workspace parent. Pass `-p:HostArtifactRoot=<absolute-path>` to either
wrapper to select another root; use distinct roots for concurrent builds.
`CustomAfterDirectoryBuildProps` is reserved by the wrappers. A direct
`dotnet` invocation with only `HostArtifactRoot` does **not** isolate projects
that do not consume that property. Explicit custom intermediate/extensions
paths or projects that bypass `Microsoft.Common.props` require separate review.

To check the evaluated intermediate and early extension paths across the actual
solution graph, including referenced external projects, run:

```powershell
$root = Join-Path (Get-Location) 'artifacts\verify-isolation-windows'
$verify = Join-Path (Get-Location) 'tools\build\VerifyIsolation.targets'
.\tools\build\dotnet-isolated.ps1 msbuild .\AA98_AvlnCodeStudio.slnx -graphBuild -t:VerifyIsolation "-p:HostArtifactRoot=$root" "-p:CustomAfterMicrosoftCommonTargets=$verify" "-p:CustomAfterMicrosoftCommonCrossTargetingTargets=$verify"
```

```bash
root="$PWD/artifacts/verify-isolation-linux"
verify="$PWD/tools/build/VerifyIsolation.targets"
bash tools/build/dotnet-isolated.sh msbuild AA98_AvlnCodeStudio.slnx -graphBuild -t:VerifyIsolation "-p:HostArtifactRoot=$root" "-p:CustomAfterMicrosoftCommonTargets=$verify" "-p:CustomAfterMicrosoftCommonCrossTargetingTargets=$verify"
```

## Interactive desktop smokes

Run each applicable smoke independently and require its success message and a
zero process exit code:

```powershell
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchMvvmSmoke\WorkbenchMvvmSmoke.csproj
.\tools\build\dotnet-isolated.ps1 run --project tools\ProjectSettingsReviewSmoke\ProjectSettingsReviewSmoke.csproj
```

For the focused compact-layout check, use a unique artifact root:

```powershell
$root = Join-Path (Get-Location) 'artifacts\compact-workbench-smoke'
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj "-p:HostArtifactRoot=$root" -- --compact-only
```

The check exercises a 720 × 500 logical-pixel client area, the workspace
sidebar toggle, editor, properties, diagnostics and toolbar overflow. At this
size, hide the workspace sidebar using the View menu to give the editor more
space. This programmatic desktop check is not physical high-DPI, keyboard,
screen-reader or Linux acceptance.

These cover integrated designer and project-settings workflows using temporary
fixtures. The architecture smoke also opens the standalone designer host. See
the individual tool documentation for detailed coverage and platform setup.

The project-debug smoke additionally requires a trusted NetCoreDbg installation:

```powershell
$env:AVLN_NETCOREDBG_PATH = "C:\path\to\netcoredbg.exe"
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchDebugSmoke\WorkbenchDebugSmoke.csproj
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
- Earlier Windows net8.0/net9.0 runs reported 711 passed and 5 skipped per
  framework, but those results were not reproduced with the subsequently
  restored package graph. SDK 11 aborted during VSTest protocol negotiation;
  SDK 10 timed out. See `findings.md` for the commands and graph details.
  Current older-target runtime acceptance remains blocked.
- .NET 8 and .NET 9 are end-of-support. Confirm the supported-target policy
  before release; a successful compatibility run would not change that status.
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
