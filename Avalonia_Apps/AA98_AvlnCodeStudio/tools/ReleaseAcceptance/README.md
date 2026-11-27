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

For the external `Workbench.Builder.Core` reference, its directory props append
`Configuration` before MSBuild supplies the default. Solution restore and
build can therefore evaluate different extensions paths. The isolation props
give that specific project a configuration-independent base intermediate path;
the SDK still separates configuration-specific intermediates. The isolated
Workbench parent retains its configuration for references outside the solution,
and the external project's default output path includes Debug when configuration
was not yet set. These adjustments apply only when the isolation props are
injected. Normal builds without them are unaffected.
The guard uses the effective `HostArtifactRoot`, which some directory props
supply by default even if it was not passed on the command line.

To verify a fresh solution restore **and** build through this actual reference
graph on Windows, choose a root that does not exist and run:

```powershell
$root = Join-Path (Get-Location) 'artifacts\verify-workbench-core-windows'
.\tools\build\verify-workbench-core-isolation.ps1 -ArtifactRoot $root
```

The check requires both the restore and net10.0 build verification messages
and a successful solution build. It leaves the root for inspection; remove
only that root after use. The injected verification targets assert the same
assets path and its existence rather than relying solely on evaluated paths.

For configuration regression checks, run solution restore and a net10.0 build
with `-p:Configuration=Debug` and then `Release`, using a fresh, distinct
`HostArtifactRoot` for each run. Inject `VerifyWorkbenchCoreIsolation.targets`
using **both** `CustomAfterMicrosoftCommonTargets` and
`CustomAfterMicrosoftCommonCrossTargetingTargets`; require the restore and
build verification messages as well as successful exit codes. Require a
message for the requested configuration. A solution build can additionally
reach the reference through a differently configured project; check that
those outputs are separated too. For a dedicated project build,
`-p:VerifyWorkbenchCoreConfiguration=Debug` or `Release` asserts the expected
configuration for every invocation. The build
target checks `ProjectAssetsFile` immediately before `ResolvePackageAssets`.
Also inspect the resulting `obj.net/Workbench.Builder.Core/<configuration>/net10.0`
and `bin/Workbench.Builder.Core/<configuration>/net10.0` DLLs. A missing
verification message is a failure, even if the build succeeds.

The Linux wrapper resolves its own directory physically before injecting the
props. This matters when the solution is opened through a symlinked workspace:
MSBuild may resolve project references to physical paths, so an aliased props
path can otherwise miss the exact Workbench project match. Direct injection
must likewise use a physical path to `IsolatedArtifacts.props`. The wrapper
does not canonicalize an explicitly supplied artifact root; use one spelling
consistently. Use distinct roots for simultaneous builds: sharing a restore
assets file between concurrent configurations is not covered.

The normal, non-injected configuration can be inspected with `dotnet msbuild
<project> -getProperty:BaseIntermediateOutputPath,MSBuildProjectExtensionsPath`
without performing a restore or build into someone else's artifacts. A
same-named project in another directory is not the external reference and
should receive only generic isolation. An explicit extensions path opts out of
the Workbench-specific adjustment and requires its own consistency review.
These checks do not prove every custom MSBuild import or third-party project is
isolated, and they do not replace runtime or UI acceptance.

On November 27, 2026, solution restores and net10.0 builds were checked with
separate new roots for Debug and Release on Windows (SDK 11.0.100) and WSL
Ubuntu 26.04 (SDK 10.0.404). Both restore and build verification messages were
present, and the actual assets JSON identified the external project and the
selected root. Compiled DLLs existed in configuration-specific `obj.net` and
`bin` paths; the Release solution graph also produced separate Debug DLLs.
An explicitly empty root and a same-named fixture elsewhere were evaluated
without non-isolated builds; a dedicated Windows project build also exercised
both configurations in one root sequentially. The Linux run used the `/mnt/c`
workspace alias, whose physical location was `/mnt/d`.

The Windows builds finished with 27 Debug and 21 Release warnings; Linux
finished with 27 warnings for each. Warnings included NU1701, MSTEST0044,
nullable/compiler, platform and Avalonia warnings. A strict Release assertion
initially failed because the solution graph also built a Debug reference; the
successful retry checked both outputs rather than asserting all invocations
were Release. Linux Debug also required a retry after MSB4166 (child node
exited prematurely); retries disabled node reuse and used one MSBuild node.
The first Linux alias attempt before the physical-path wrapper fix missed the
Workbench verification and was not a pass. These are build-path checks, not
test execution or proof of the older target frameworks.

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

- The counts and smoke results below are historical evidence from earlier
  revisions of the working copy. Subsequent changes were tested separately;
  rerun the complete applicable matrix on the release candidate before
  treating any of these results as release acceptance.
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
