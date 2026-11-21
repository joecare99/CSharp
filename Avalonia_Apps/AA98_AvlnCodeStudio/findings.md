# Development review findings

Review scope: Windows/.NET 10 build and test run, documented Workbench desktop smoke flows, launch/screenshot check, static review of application and shared project setup, with emphasis on Windows and Linux/WSL portability. Findings are ordered by impact. No source changes were made during this review.

## High priority

### Planning document writes can escape the selected repository

**Location:** `AA98_AvlnCodeStudio.Planning.Local/Services/LocalPlanningProvider.cs:88-105`, `:142-156`

`item.SourcePath` is accepted as-is and combined with the repository root. Rooted paths or traversal components such as `..` can resolve outside the selected repository. The resulting directory can be created and `File.WriteAllTextAsync` can overwrite a file at that external location. The generated fallback path also relies on input values rather than verifying the final canonical path remains within the intended planning root. A symlink inside the root may also redirect the write outside it.

**Recommendation:** Canonicalize the repository root and every target; reject rooted paths and paths outside the allowed root using a separator-aware containment check. Resolve/check symlinks (or use a write strategy that prevents following them) before creating directories or writing. Add tests for rooted paths, traversal, and symlink escape attempts, asserting no external file is created or changed.

**Status after follow-up:** Addressed in `LocalPlanningProvider.WriteAsync`. Rooted and parent-traversal paths are rejected, planning and generated fallback paths are constrained to the selected planning root, and existing symlink targets are resolved and checked before directory creation and writing. Regressions cover rooted paths, POSIX and Windows-style traversal, generated fallback paths, planning-root symlinks, and document-directory symlinks. `LocalPlanningProviderTests` passes 9/9 on Windows/net10 and isolated WSL Ubuntu 26.04/net10.

## Medium priority

### Workbench syntax colors are unreadable on dark backgrounds

The AvaloniaEdit C# definition used dark fixed syntax colors on the Workbench's
dark background. The light variant also inherited fixed dark Workbench surfaces,
making its Fluent foreground text unreadable.

**Status after follow-up:** The editor loads independent light/dark versions of
the existing C# and XML-documentation highlighting definitions with contrasting
foreground colors. Workbench, menu and status surfaces now follow the theme.
Programmatic Windows and WSLg/net10 theme smokes check contrast for representative
highlighted text, menu, editor and status, theme switching and editor undo/redo;
WSLg screenshots were visually inspected. The View menu now exposes a localized
light-theme toggle; dark remains the startup default and the selection is
session-only. The menu updates the theme through both automation invoke and
toggle and reflects application theme changes. These programmatic checks do not
constitute a full accessibility, high-DPI or physical-display assessment.

### Windows and Linux can collide in shared build artifacts

**Location:** `AA98_AvlnCodeStudio.props:1-12`, `Solution.props:3-5`, `tools/ReleaseAcceptance/README.md:46-51`, `tools/WorkbenchMvvmSmoke/README.md:9-13`

Build output and intermediate paths are shared outside the checkout. The project explicitly warns that Windows and Linux must not share `bin`/`obj` artifacts, and recommends an early-import isolation props file for WSL. Without consistently applying that setup, restore/build results can be overwritten or reused across operating systems, leading to confusing failures or invalid platform-specific outputs.

**Recommendation:** Make artifact roots configurable and host-specific by default (for example, separate Windows and Linux roots), and provide a checked-in, supported invocation/script that applies the isolation settings before project evaluation. Document the expected Windows and WSL commands together. Do not rely on users remembering a manual props-file procedure.

**Status after follow-up:** Addressed with host-specific defaults under the shared workspace's `artifacts/windows` and `artifacts/linux` roots, plus checked-in PowerShell and Bash wrappers that pass `HostArtifactRoot` before MSBuild evaluation. The application, shared library references, smoke projects, and external Workbench.Builder project resolve into host-separated paths. Windows and WSL wrapper runs of `LocalPlanningProviderTests` both pass 9/9; property evaluation confirms distinct roots. No existing build artifacts were removed.

**External-project follow-up:** A later Linux build exposed that `HostArtifactRoot` alone did not isolate `Avln_CommonDialogs.Avalonia`, which could reuse Windows restore assets. Both wrappers now inject early isolation props without replacing normal directory imports. Full solution graph checks and a restore/build of that external project passed on Windows and WSL with separate custom roots; the original shared assets were unchanged. Direct `dotnet` invocations that only set `HostArtifactRoot`, explicit intermediate-path overrides and projects bypassing `Microsoft.Common.props` remain outside this wrapper guarantee.

### Local planning scaffold generates a Visual Studio-only shared project

**Location:** `AA98_AvlnCodeStudio.Planning.Local/Services/LocalPlanningProjectScaffolder.cs`

The scaffold writes `.shproj` and `.projitems` files with Visual Studio CodeSharing imports under `MSBuildExtensionsPath32`. These targets are not part of a normal Linux .NET SDK installation, so the generated planning project is not a portable Linux/WSL project even though the main app targets those platforms.

**Recommendation:** Generate an SDK-style project format supported by the .NET SDK on both operating systems, or clearly label this feature as Windows/Visual Studio-specific and keep it out of the Linux-supported workflow. Include an automated Linux SDK evaluation/build test for the generated output.

**Verification status:** The local scaffold emits an SDK-style `net8.0` `.csproj` and a portable `.projitems` import that preserves recursive Markdown inclusion while excluding `bin`, `obj`, and `.vs`; the planning view recognizes generated `.csproj` files paired with `.projitems` without reclassifying ordinary C# projects. Focused evaluation/build tests pass on Windows with .NET SDK 11.0.100 and WSL Ubuntu 26.04 with user-local .NET SDK 10.0.404; the Linux fixture was evaluated and built as `net10.0`. The planning host also builds on Windows. Test execution used a temporary compile exclusion for an unrelated existing expression-tree compiler error in `ProjectWorkflowViewModelTests.cs` and suppressed an existing NU1605 restore downgrade; neither issue was changed. The broader findings tracking remains unchanged.

### Embedded terminal is pipe-backed, not a terminal emulator session

**Location:** `AA98.Terminal.Host/Services/HostedTerminalProcessFactory.cs:18-25`, `AA98.Terminal.Host/Services/HostedTerminalProcess.cs:59-63`

The shell is started with redirected stdin/stdout/stderr and input is sent one line at a time. It has no pseudo-terminal (PTY). Interactive shell prompts, job control, terminal modes, curses/full-screen programs, and many REPLs will not behave like a normal terminal on Windows or Linux/WSL.

**Recommendation:** Use ConPTY on Windows and a Unix PTY on Linux/WSL behind a platform-neutral terminal-session abstraction. If the feature is intentionally a command runner rather than a terminal, describe it as line-oriented and disable/avoid presenting unsupported interactive behaviors.

**Status after follow-up:** Kept intentionally as a line-oriented command runner rather than adding PTY support. The shared `ITerminalSession` contract documents complete-line input; the standalone window title, input labels, persistent mode notice, session status, and `AA98.Terminal.Host/README.md` identify the missing PTY and unsupported interactive scenarios. Process behavior is unchanged. `AA98.Terminal.Host.Tests` passes 10/10 on Windows/net10 and isolated WSL Ubuntu 26.04/net10; the warning notice is not a claim of terminal-emulator support.

### Git process can deadlock and may survive cancellation

**Location:** `AA98_AvlnCodeStudio.Versioning.Git/Services/GitCommandRunner.cs:34-37`

The implementation reads all stdout before reading stderr. If Git fills the stderr pipe while stdout remains open, Git can block on stderr and the caller can wait indefinitely on stdout. Cancellation during stream reads or `WaitForExitAsync` also does not explicitly terminate and reap the child process.

**Recommendation:** Drain stdout and stderr concurrently, await both drains and process exit, and on cancellation kill the process tree and wait for termination in a `finally` path. Test large output on both streams and cancellation while Git is running.

**Status after follow-up:** Addressed in `GitCommandRunner`. Standard output and standard error are read concurrently; process exit and both drains are awaited. Exceptional or cancellation exits terminate the live process tree and reap the root process in `finally`, while normal exit codes and captured output remain unchanged. `GitCommandRunnerTests` passes 2/2 on both Windows/net10 and isolated WSL Ubuntu 26.04/net10, including output exceeding pipe capacity on both streams and cancellation with a child-process check. These tests cover the test-owned process tree, not every possible OS/process race.

### Build toolchain dependencies are noisy and show compatibility risk

**Location:** `AA98_AvlnCodeStudio/Directory.Packages.props:10`, `Libraries/BaseLib/Models/FileProxy.cs:14-15`; affected project references appear in the .NET 10 build output.

The .NET 10 restore/build emitted `NU1701` for `Microsoft.Build 18.12.2` and `Microsoft.IO.Redist 6.1.0` because they restored against .NET Framework assets. These warnings span the Workbench, builder, designer, and test projects and mean a passing local build is not evidence that these combinations are supported. Separately, CA1416 flagged a Windows ACL call that is reachable on all platforms. Focused Linux acceptance initially failed with `NU1605`: `Microsoft.Build.Framework 18.12.2` requires `Microsoft.NET.StringTools (=18.12.2)`, while central resolution selected 18.11.0. The central pin is now 18.12.2; the combined Windows/net10 C# language and project workflow suites pass 44/44 without suppressing NU1605. Linux VSTest acceptance passes 6/6 and RAD Axaml acceptance passes 341 with 3 skips; both used isolated roots and no NU1605 suppression. The RAD run required an empty `RestoreFallbackFolders` override to avoid stale Windows restore assets referencing the Visual Studio fallback folder. Focused net8/net9 builds compiled but their test hosts did not complete: SDK 11 crashed with a `System.Text.Json` `MissingMethodException`, and an SDK 10 retry timed out during VSTest protocol negotiation; those runtimes are not validated by these runs.

**Recommendation:** Align Microsoft.Build, StringTools, Roslyn, and SDK versions with officially supported .NET 10 assets; verify runtime asset selection on Windows and Linux. Guard the ACL call with `OperatingSystem.IsWindows()` or move it behind a platform-specific implementation with a non-Windows behavior. Treat warning reduction as a tracked quality gate.

**Testhost follow-up:** A focused investigation reproduced the SDK 11/VSTest 18.12 protocol crash on net8 and net9 with `System.Text.Json` 11.0.0, and the SDK 10/VSTest 18.7 negotiation timeout on net9. Experiments replacing output assemblies permitted individual tests to run but did not represent the restored dependency graph and are not an accepted fix. A compatible package graph and clean runs for both frameworks remain open; the unsupported-runtime caveat remains unchanged.

**Isolated aligned-graph follow-up (Windows, 2026-11-17):** After the drive interruption, the copied experiment was inspected and rebuilt in a new `out-final-verified` root under `artifacts\fleet-supported-graph-copy-84671c27`; none of the real workspace manifests were edited. The copy pins Hosting, DependencyInjection, DependencyInjection.Abstractions and Logging.Abstractions to 10.0.14 in its solution `Directory.Packages.props`, and changes parent `Packages.props`'s DI.Abstractions pin to 10.0.14. Eight copied library projects' direct DI.Abstractions 11.0.0 references and the copied Config.Service.Tests DI 11.0.0 reference were changed to 10.0.14; the copy also contains a `Libraries\Directory.Packages.props` disabling central package management for those explicitly versioned library projects. These are experimental copy-wide changes, not a proposed minimal patch.

Using SDK 11.0.100, a fresh full-graph forced restore exited 0 without `NoWarn` (13 NU1701 warning lines, including Microsoft.Build and Microsoft.IO.Redist). The new test-project assets resolve on **both** net8 and net9: Hosting, DI, DI.Abstractions, Logging.Abstractions and System.Text.Json 10.0.14; Microsoft.NET.Test.Sdk, TestHost, ObjectModel and TranslationLayer 18.12.0; MSTest 4.5.3. No package at version 11.0.0 appears in either test-project graph. STJ and DI.Abstractions select their respective net8/net9 runtime assets; TranslationLayer selects net8 assets. The output STJ file version is 10.0.1426.52326 on both frameworks. This is the actual restored/built graph, with no DLL replacement. Builds with `--no-restore` succeeded: net8 25 warnings, net9 23 warnings, 0 errors each. Build warnings include net10 project-reference warnings that Microsoft.Build.Utilities.Core and Microsoft.NET.StringTools 18.12.2 do not support net10, in addition to NU1701; absence of the old DI.Abstractions 11 warning does not establish support for the complete graph.

On installed Microsoft.NETCore.App 8.0.33 and 9.0.22, VSTest focused runs passed **5/5 on each**: three `AppContextContractTests` plus `FindTestMethodCandidatesAsync_NUnitCandidateRunsParameterizedCases` and `FindTestMethodCandidatesAsync_XunitCandidateRunsParameterizedCases`. The latter create, restore and run parameterized fixture projects and exercise TranslationLayer discovery; optional debugger paths were not established by these results. Further runs passed **11/11 on each** for `DotNetTestExecutionServiceTests` and `VSTestRunResultTests`, and **16 passed, 4 skipped on each** for the complete `CSharpSyntaxDiagnosticServiceTests` class (including the two integration tests again; skips concern case-sensitive paths on Windows). These sets overlap and are not a full-suite result. TRX and command output are retained as `results-final-verified\*.trx`, `restore-final-verified.log`, `build-final-verified-net*.log`, `targeted-final-verified-net*.log`, `broader-final-verified-net*.log` and `syntax-final-verified-net*.log` inside the copy. The earlier 5/5 runs per target used DI.Abstractions 11 and are not evidence for the final graph.

Reproduction from the copied `Avalonia_Apps\AA98_AvlnCodeStudio` directory, with `$base` set to the copied experiment root and `TEMP`/`TMP` set to its `fixture-final-verified` directory (containing `<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>` so generated fixture projects can use explicit package versions):

```powershell
$project = 'AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj'
$root = Join-Path $base 'out-final-verified'
$inject = Join-Path (Get-Location) 'tools\build\IsolatedArtifacts.props'
dotnet restore $project "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$inject" --force --verbosity minimal
foreach ($tfm in 'net8.0','net9.0') {
  dotnet build $project --no-restore -f $tfm "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$inject" --verbosity quiet
  dotnet test $project --no-build --no-restore -f $tfm "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$inject" --filter 'FullyQualifiedName~AppContextContractTests|FullyQualifiedName~FindTestMethodCandidatesAsync_NUnitCandidateRunsParameterizedCases|FullyQualifiedName~FindTestMethodCandidatesAsync_XunitCandidateRunsParameterizedCases'
  dotnet test $project --no-build --no-restore -f $tfm "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$inject" --filter 'FullyQualifiedName~DotNetTestExecutionServiceTests|FullyQualifiedName~VSTestRunResultTests' --blame-hang-timeout 4m
  dotnet test $project --no-build --no-restore -f $tfm "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$inject" --filter 'FullyQualifiedName~CSharpSyntaxDiagnosticServiceTests' --blame-hang-timeout 4m
}
```

**Limitations/next step:** At the end of the Windows investigation, full suites, Linux, net10 behavior and SDK 10's runner had not been tested for this final variant; see the Linux follow-up below for subsequent results. No product fix, minimal compatible graph, or support for end-of-support net8/net9 runtimes is claimed. Review all affected direct and transitive constraints and warning-bearing dependencies; if adopting an aligned graph, validate it without suppression in the actual coordinated solution and run full suites on both platforms and supported deployment targets.

**Isolated aligned-graph Linux follow-up (2026-11-18):** WSL2 Ubuntu 26.04 x64, kernel 6.18.40.1, has SDKs 10.0.402, 10.0.403 and 10.0.404; SDK 10.0.404 was used. The only installed Microsoft.NETCore.App and Microsoft.AspNetCore.App runtimes are 10.0.13, 10.0.14 and 10.0.15. There is **no net8 or net9 runtime**, so Linux execution/acceptance of those targets is blocked; they were not run using a major-version roll-forward. No SDK, runtime or network package was installed.

The same experimental copy above was restored using only the existing Linux NuGet cache as a package source, with separate Linux package, CLI, HTTP-cache, fixture, intermediate and output directories under `artifacts\fleet-supported-graph-copy-84671c27\linux-check-20261118`. This did not use or overwrite the Windows `out-final-verified` assets, modify product manifests, or suppress warnings. Full-graph forced restore succeeded (13 NU1701 warning lines). The Linux test assets resolve Hosting, DI, DI.Abstractions, Logging.Abstractions and STJ 10.0.14 on both net8 and net9, Test SDK/TestHost/ObjectModel/TranslationLayer 18.12.0 and MSTest 4.5.3. On net10 the named Extensions and test packages have the same versions; STJ is not a package in that target's assets. `--no-restore` builds succeeded for net8 (25 warnings), net9 (23), and net10 (35), each with zero errors. Warnings include NU1701 and the Microsoft.Build.Utilities.Core and Microsoft.NET.StringTools 18.12.2 net10 support warnings. Building older targets does not establish that they run on Linux.

As a separate **net10-only experiment**, VSTest passed 5/5 focused tests (three AppContext contracts and both NUnit/xUnit parameterized fixture discovery/execution tests), 11/11 DotNetTestExecutionService/VSTestRunResult tests, and 16 passed/4 skipped in CSharpSyntaxDiagnosticServiceTests. The syntax set overlaps the focused set. On the case-insensitive mounted checkout, three case-sensitive path tests skipped; the Windows-specific case-alias test also skipped. These are not full-suite, debugger-path, net8/net9 or product-support results. Logs and TRX files are retained in the Linux directory (`restore.log`, `build-net*.log`, `test-*-net10.log`, `results\*.trx`).

Commands from the original workspace root under WSL (fixture-generated restores also use the existing cache via fixture-only `NuGet.Config`; `TMPDIR` remains in the isolated Linux directory):

```bash
base="$PWD/artifacts/fleet-supported-graph-copy-84671c27"
run="$base/linux-check-20261118"
mkdir -p "$run/fixture" "$run/packages" "$run/cli" "$run/http-cache" "$run/results"
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_CLI_HOME="$run/cli" NUGET_PACKAGES="$run/packages" NUGET_HTTP_CACHE_PATH="$run/http-cache" TMPDIR="$run/fixture"
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 DOTNET_CLI_TELEMETRY_OPTOUT=1
printf '%s\n' '<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>' > "$TMPDIR/Directory.Packages.props"
printf '<configuration><packageSources><clear/><add key="cached" value="%s/.nuget/packages" /></packageSources></configuration>\n' "$HOME" > "$TMPDIR/NuGet.Config"
cd "$base/Avalonia_Apps/AA98_AvlnCodeStudio"
project=AA98_AvlnCodeStudio.Tests/AA98_AvlnCodeStudio.Tests.csproj
common=("-p:HostArtifactRoot=$run/out" "-p:CustomAfterDirectoryBuildProps=$PWD/tools/build/IsolatedArtifacts.props" "-p:RestoreFallbackFolders=")
dotnet restore "$project" "${common[@]}" --source "$HOME/.nuget/packages" --force --verbosity minimal
for tfm in net8.0 net9.0 net10.0; do dotnet build "$project" "${common[@]}" --no-restore -f "$tfm" --verbosity quiet; done
focused='FullyQualifiedName~AppContextContractTests|FullyQualifiedName~FindTestMethodCandidatesAsync_NUnitCandidateRunsParameterizedCases|FullyQualifiedName~FindTestMethodCandidatesAsync_XunitCandidateRunsParameterizedCases'
broader='FullyQualifiedName~DotNetTestExecutionServiceTests|FullyQualifiedName~VSTestRunResultTests'
syntax='FullyQualifiedName~CSharpSyntaxDiagnosticServiceTests'
for group in focused broader syntax; do
  filter="${!group}"
  dotnet test "$project" "${common[@]}" --no-build --no-restore -f net10.0 --filter "$filter" --blame-hang-timeout 4m --logger "trx;LogFileName=$group-net10.trx" --results-directory "$run/results" --verbosity minimal
done
```

**Next step:** In an appropriately provisioned Linux environment with actual net8 and net9 runtimes, repeat clean isolated restore/build and focused tests on each runtime, including adapter discovery, then run complete suites and case-sensitive filesystem checks. Investigate existing warnings and dependency compatibility before any adoption. No installation was attempted in this WSL environment, and neither the net10 experiment nor the Windows results constitute Linux net8/net9 acceptance.

**Linux missing-runtime and installer follow-up (2026-11-18):** Actual VSTest invocations against the isolated previously built outputs, using the existing SDK 10.0.404 host and without major roll-forward, each exited 1 before running any test. The net8 testhost requested `Microsoft.NETCore.App` 8.0.0; the net9 testhost requested 9.0.0. Both reported `/home/mir/.dotnet/` as their .NET location and only 10.0.13, 10.0.14 and 10.0.15 as available. Exact commands and environment are retained in `linux-check-20261118/missing-runtime-check.sh`; full output is in `missing-runtime-net8.0.log` and `missing-runtime-net9.0.log`.

The official installer was downloaded from `https://dot.net/v1/dotnet-install.sh` into that same isolated Linux artifact directory (SHA-256 `082f7685e156738a1b2e2ed8381a621870d4ce8e8c59278034556f05c186eb2e`). Official `--dry-run` calls succeeded and resolved Microsoft `builds.dotnet.microsoft.com` Linux x64 runtime payloads 8.0.34 and 9.0.23; see `install-dry-run-8.0.log` and `install-dry-run-9.0.log`. A real install was **not run**: the downloaded installer invokes `mktemp` for archive extraction and copy checks, which is not permitted in this environment, even with an isolated `TMPDIR`. The dry runs prove resolution only, not download, extraction, host discovery, or test success. No runtime was installed and the real home installation and Windows assets were not changed.

```bash
bash artifacts/fleet-supported-graph-copy-84671c27/linux-check-20261118/missing-runtime-check.sh
run="$PWD/artifacts/fleet-supported-graph-copy-84671c27/linux-check-20261118"
curl --fail --location --silent --show-error https://dot.net/v1/dotnet-install.sh --output "$run/dotnet-install.sh"
export TMPDIR="$run/fixture"
for c in 8.0 9.0; do
  bash "$run/dotnet-install.sh" --runtime dotnet --channel "$c" --architecture x64 --install-dir "$run/dotnet-isolated" --no-path --dry-run
done
```

**Remaining blocker:** Run the official installer only in an environment permitting its temporary-directory operations, confined to a dedicated artifact directory. An isolated host must also have access to the existing SDK and its net10 runtime without writing into the existing installation; verify `dotnet --info`, `--list-sdks`, `--list-runtimes`, and actual testhost location/runtime selection before running the focused net8/net9 sets including TranslationLayer, then the full suites. The existing net10 results do not substitute for those runs. Net8/net9 are end-of-support compatibility targets, not a support claim.

**Roslyn downgrade follow-up:** A subsequent isolated Linux restore reported `NU1605`: the external `Workbench.Builder.Core` project now requires `Microsoft.CodeAnalysis.CSharp` 5.12.0, while the Workbench directly references central version 5.9.0. The AXAML resource tests passed only after retrying with `NoWarn=NU1605`; this is not a clean restore or proof of compatibility. The external project change was not reverted, and the package graph needs deliberate alignment and validation.

**Roslyn alignment:** The four central C# and Workspaces package pins are now 5.12.0, matching the net10-only builder's direct CSharp reference. Isolated Windows and Ubuntu 26.04 full-graph restores succeeded without suppressing NU1605. The UI assets resolve Common, CSharp, CSharp.Features, Features, CSharp.Workspaces, Workspaces.Common, Workspaces.MSBuild and Scripting.Common consistently to 5.12.0 for net10 and to 4.14.0 for net8/net9, whose project-specific overrides are unchanged. Windows SDK 11.0.100 and WSL SDK 10.0.404 compiled all three frameworks; net10 language diagnostics, workspace loader, workflow context and workflow view-model tests passed 74, skipped 4 on each platform. Linux test fixtures were initially placed inside the checkout and inherited its central package policy, causing NU1008 in fixture projects with explicit package versions. Retrying with a temporary fixture-only `Directory.Packages.props` setting `ManagePackageVersionsCentrally` to false passed; the fixture directory was then removed. The Linux case-sensitive fixtures were skipped on the case-insensitive mounted checkout; separate ext4-focused diagnostics passed 2/2. Existing NU1701 and unsupported Microsoft.Build/StringTools net10 warnings remain. These checks do not resolve the previously documented net8/net9 testhost failures or establish support for those runtimes.

Verification commands (fresh platform-specific `HostArtifactRoot`, no warning suppression):

```powershell
$root = Join-Path (Get-Location) 'artifacts\roslyn-align-20261116-win-net10'
$project = '.\AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj'
& .\tools\build\dotnet-isolated.ps1 restore $project "-p:HostArtifactRoot=$root" --force --verbosity quiet '-clp:ErrorsOnly'
foreach ($framework in @('net10.0','net8.0','net9.0')) { & .\tools\build\dotnet-isolated.ps1 build $project "-p:HostArtifactRoot=$root" -f $framework --no-restore --verbosity quiet '-clp:ErrorsOnly' }
& .\tools\build\dotnet-isolated.ps1 test $project "-p:HostArtifactRoot=$root" -f net10.0 --no-build --no-restore --filter 'FullyQualifiedName~CSharpSyntaxDiagnosticServiceTests|FullyQualifiedName~CSharpWorkspaceLoaderTests|FullyQualifiedName~ProjectWorkflowContextTests|FullyQualifiedName~ProjectWorkflowViewModelTests' --verbosity quiet
```

```bash
export PATH="$HOME/.dotnet:$PATH"
root="$PWD/artifacts/roslyn-align-20261116-linux-full"
project=AA98_AvlnCodeStudio.Tests/AA98_AvlnCodeStudio.Tests.csproj
bash tools/build/dotnet-isolated.sh restore "$project" "-p:HostArtifactRoot=$root" -p:RestoreFallbackFolders= --force --verbosity quiet -clp:ErrorsOnly
for framework in net10.0 net8.0 net9.0; do bash tools/build/dotnet-isolated.sh build "$project" "-p:HostArtifactRoot=$root" -f "$framework" --no-restore --verbosity quiet -clp:ErrorsOnly; done
fixture="$PWD/artifacts/roslyn-align-20261116-linux-test-tmp"
mkdir -p "$fixture"
printf '%s\n' '<Project><PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup></Project>' > "$fixture/Directory.Packages.props"
for name in CSharpSyntaxDiagnosticServiceTests CSharpWorkspaceLoaderTests ProjectWorkflowContextTests ProjectWorkflowViewModelTests; do TMPDIR="$fixture" bash tools/build/dotnet-isolated.sh test "$project" "-p:HostArtifactRoot=$root" -f net10.0 --no-build --no-restore --filter "FullyQualifiedName~$name" --logger console --verbosity minimal; done
rm -rf -- "$fixture"
```

The initial framework-filtered restore was insufficient for `Avln_CommonDialogs.Avalonia`'s net8 target; the verified restore above evaluates the full multi-target graph.

### Credential library still advertises end-of-support target frameworks

**Location:** `AA98_AvlnCodeStudio.Config.Secrets/AA98_AvlnCodeStudio.Config.Secrets.csproj:4-18`

The credential project conditionally targets .NET 8, 9 and 10. Microsoft ended support for .NET 8 and .NET 9 on November 10, 2026; retaining those target frameworks can be useful for compatibility testing, but it does not make deployment on those runtimes supported or serviced. The Linux backend's direct dependency, `Ace4896.DBus.Services.Secrets` 1.6.0, is MIT-licensed; the inspected restore resolves `Tmds.DBus.Protocol` 0.92.0, which includes the fix for GHSA-xrw6-gwf8-vvr9. No published advisory for the direct package was listed in the checked GitHub NuGet advisory query; this was not a complete transitive dependency audit.

**Recommendation:** Decide whether to remove net8/net9 from the supported target matrix or retain them strictly for compatibility builds while requiring a supported runtime (currently .NET 10) for deployment. Keep dependency/advisory checks scoped and repeat them when updating the package.

## Low priority

### Main window minimum size is inflexible

**Location:** `AA98_AvlnCodeStudio.UI/Views/MainWindow.axaml:15-18`

The main window sets a fixed minimum of 900 x 600 logical pixels. Compact screens, high scaling factors, or a constrained WSLg display can make it impossible to fit the full window.

**Recommendation:** Reduce the minimum size, make workspace panes collapse/scroll at smaller sizes, and verify at small logical resolutions and high DPI on both platforms.

**Status after follow-up:** The minimum is now 720 x 500 logical pixels and the toolbar scrolls horizontally when needed. The existing View menu can collapse the workspace sidebar to make editor space; this is a manual choice. Programmatic desktop smokes on Windows and WSLg/net10 exercised the compact layout, pane visibility, editor, diagnostics and restoration at full size. Physical high-DPI, keyboard and screen-reader acceptance remains open.

### Avalonia MVVM template pins an older package version

**Location:** `AA98_AvlnCodeStudio.Model/ProjectTemplates/avalonia-mvvm.json:5-6`, `Packages.props:10-18`

The generated Avalonia project template pins core packages to Avalonia 12.1.4, while central solution package versions are 12.1.5. Newly generated apps can therefore start behind the version used by the tool itself.

**Recommendation:** Keep template package versions synchronized with the supported central version or generate a central package management file so the versions have one source of truth. Add a template validation smoke that restores/builds its output.

**Status after follow-up:** The MVVM template now uses Avalonia 12.1.6, matching the central package version. A regression checks alignment, and the generated projects for net8/net9/net10 were built with separated MVVM sources and external build artifacts. The initial focused test run reported an unspecified failure with quiet output; the repeated focused run passed.

## Verification record

- Environment: Windows 10, .NET SDK 10.0.403 / runtime 10.0.14.
- `dotnet test .\AA98_AvlnCodeStudio.slnx --framework net10.0`: the first run had one failing secrets-vault contract test. Rerunning the secrets test project passed 15 tests with 4 skipped; the other test assemblies passed (Workbench 711 passed, 1 skipped; editor 17 passed; AI 12 passed; terminal 10 passed). The filtered vault test passed separately. The original failure was therefore transient/not reproduced, but should be monitored in CI.
- `tools/WorkbenchArchitectureSmoke`, `tools/WorkbenchMvvmSmoke`, and `tools/ProjectSettingsReviewSmoke` all completed successfully on Windows. They perform programmatic UI actions and do not replace physical keyboard or screen-reader acceptance.
- The Workbench launched successfully. Screenshot: `C:\Users\Mir\AppData\Local\Temp\mammouth\workbench-smoke.png`.
- WSL was present, but Ubuntu 22.04 had no .NET SDK and no GUI display variables in this session, so Linux-native build/smoke execution was blocked. `tools/ReleaseAcceptance/README.md:93-103` records prior Linux .NET 10 and WSLg smoke passes; those are historical records, not a fresh run in this review.
- No lint or typecheck command was found in the inspected project guidance. Build warnings included legacy package compatibility, platform API, nullable, obsolete API, and analyzer warnings. No lint-specific gate could be run.

### Legacy net8/net9 testhost graph recheck after Roslyn 5.12 alignment (Windows)

**Conditions and result:** On Windows 10 x64, SDKs 11.0.100 and 10.0.403 were separately selected from the installed SDKs; Microsoft.NETCore.App 8.0.33 and 9.0.22 were installed. Each SDK used its own new `HostArtifactRoot`, a full forced restore with no `NU1605` suppression, and successful net8/net9 builds (288/286 warnings respectively). Using the restored outputs unchanged, SDK 11's VSTest aborted *both* full runs before executing tests: protocol negotiation raised `MissingMethodException` for `JsonObjectInfoValues<T>.set_ObjectCreator`. SDK 10's VSTest aborted *both* full runs after a 30-second protocol-negotiation timeout; this timeout is not proof of its underlying cause. Thus the previously reported 711 passed/5 skipped on net8/net9 was not reproduced in these current-asset VSTest runs; it is not evidence that 711 tests were executed or passed here.

Both `AA98_AvlnCodeStudio.Tests` `project.assets.json` files resolve the same net8/net9 host graph: `Microsoft.NET.Test.Sdk`, `Microsoft.TestPlatform.TestHost`, `ObjectModel`, and `TranslationLayer` 18.12.0, `MSTest`/adapter 4.5.3, `Microsoft.Testing.Platform` 2.5.3, Roslyn CSharp 4.14.0, and `System.Text.Json` 11.0.0 (netstandard2.0 runtime asset). Test output `testhost.dll` and CommunicationUtilities are file version 18.1200.26.56015; STJ file version is 11.0.26.53018. `Microsoft.Extensions.Hosting` 11.0.0 is referenced by the MCP and Axaml designer hosts; its Configuration.Json/Logging.Console/Logging.EventSource dependencies require STJ 11.0.0. Directly pinning an older STJ alone would therefore not be a clean graph. Runtimeconfig requests Microsoft.NETCore.App 8.0.0/9.0.0; the installed servicing runtimes above supply the respective major versions. TestPlatform package build-transitive warnings explicitly say the 11.0 dependencies do not support net8/net9. The runner SDK and the restored testhost package are distinct: SDK 10 selecting its own VSTest does not downgrade the testhost dependency graph.

A separate clean SDK 11 graph/build with `EnableMSTestRunner=true`, without DLL substitution, allowed an MSTest/Microsoft Testing Platform process to run a small test selection: `AppContextContractTests` passed 3/3 on each of net8 and net9. This bypasses the outer VSTest protocol, but is **not** a complete fix: an unfiltered net8 MTP run was intentionally stopped after prolonged execution; before stopping it, the NUnit and xUnit adapter-discovery integration tests failed with the same `MissingMethodException` in the in-process VSTest TranslationLayer. The run also reported a catalog assertion and two file-in-use cleanup failures; these are observations, not diagnosed regressions. A broader filtered net8 run timed out at 90 seconds and was stopped, and a full net9 MTP run was not completed. No complete 711/5 result or supported runtime combination has been demonstrated. net8/net9 remain compatibility build targets, not validated/supported deployments by this investigation; no target policy was changed.

**Next step:** Coordinate a compatible set of Hosting/Extensions, STJ and TestPlatform/TranslationLayer dependencies for both older target frameworks, with explicit transitive constraints and a clean restore (do not suppress NU1605), then run the *complete* test suite including adapter discovery on both frameworks and platforms. MTP may be explored as an outer-runner workaround but does not cure the application's own TranslationLayer incompatibility. No minimal package-only solution is established by these observations, and no shared manifest was changed.

Reproduction from the repository root (the wrapper injects its isolation props; last `HostArtifactRoot` wins):

```powershell
$base = (Get-Location).Path
$project = Join-Path $base 'AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj'
$script = Join-Path $base 'tools\build\dotnet-isolated.ps1'
$env:VSTEST_CONNECTION_TIMEOUT = '30'
foreach ($sdk in @('11','10')) {
  $root = Join-Path $base "artifacts\fleet-testhost-graph-sdk$sdk-84671c27"
  New-Item -ItemType Directory -Force $root | Out-Null
  if ($sdk -eq '10') { Set-Content (Join-Path $root 'global.json') '{"sdk":{"version":"10.0.403","rollForward":"disable"}}' }
  Push-Location $(if ($sdk -eq '10') { $root } else { $base })
  dotnet --version
  & $script restore $project "-p:HostArtifactRoot=$root" --force --verbosity quiet '-clp:ErrorsOnly'
  foreach ($f in @('net8.0','net9.0')) { & $script build $project "-p:HostArtifactRoot=$root" -f $f --no-restore --verbosity quiet '-clp:ErrorsOnly' }
  foreach ($f in @('net8.0','net9.0')) { & $script test $project "-p:HostArtifactRoot=$root" -f $f --no-build --no-restore --verbosity minimal }
  Pop-Location
}
```

```powershell
$root = Join-Path $base 'artifacts\fleet-testhost-graph-mtp-sdk11-84671c27'
$env:TESTINGPLATFORM_TELEMETRY_OPTOUT = '1'
& $script restore $project "-p:HostArtifactRoot=$root" '-p:EnableMSTestRunner=true' --force --verbosity quiet '-clp:ErrorsOnly'
foreach ($f in @('net8.0','net9.0')) {
  & $script build $project "-p:HostArtifactRoot=$root" '-p:EnableMSTestRunner=true' -f $f --no-restore --verbosity quiet '-clp:ErrorsOnly'
  & dotnet (Join-Path $root "bin\AA98_AvlnCodeStudio.Tests\Debug\$f\AA98_AvlnCodeStudio.Tests.dll") --filter 'FullyQualifiedName~AppContextContractTests' --progress off --timeout 45s --results-directory (Join-Path $root "results-$f")
}
# The partial unfiltered net8 run used: dotnet <net8 test DLL> --no-progress
```

### Standalone builder runtime metadata (2026-11-21)

`AA98.Builder.Host build` delegates to the Workbench-backed compiler, not a full SDK build by default. The shared `ICodeStudioBuilderService` contract explicitly does not guarantee launch-ready metadata. The Workbench debug workflow separately runs an SDK build before launch.

An owned Windows/net10 fixture reproduced the problem with SDK 11.0.100: build a console app, add a project reference to a library whose method is actually called, build that library and restore the app, then invoke the standalone host. It returned `succeeded=True`, exit 0 and zero diagnostics; the library DLL was present but the existing app `.deps.json` still lacked it. Running the app failed with `FileNotFoundException`. An SDK `dotnet build --no-restore` then put `Library.dll` in the dependency manifest and the app printed `dependency-loaded`, exit 0. Fixture projects and all their outputs were isolated under the unique `artifacts\fleet-builder-runtime-1121-cb9db67a7ccf499e918d2c823202c343` root.

The external `Workbench.Builder.Core` implementation only invokes its SDK build when the `.deps.json` or `.runtimeconfig.json` is missing, so existing metadata can be stale; it also emits through Roslyn. Replacing that compilation path with an unconditional SDK build here would change the existing compilation contract, diagnostics, targets and side effects. No external source was changed. Automatically refreshing metadata in the shared builder would require a separate contract and implementation decision covering configuration, framework, RID, failure and cancellation behavior.

The runtime-builder regression now runs the real standalone host after adding a non-constant project dependency on net10, then verifies that the SDK runtime build refreshes the manifest and the executable runs; the host help has a focused test. On Windows the isolated wrapper ran the host, runtime-builder and adapter tests: the initial test project run passed 8/8 before the final help test was added. A subsequent full test-project build failed in an unrelated modified `AxamlDesignerViewModelTests.cs` at `IPropertyItem.IsReadOnly` (CS1061); no foreign change was reverted. A separate owned test project linking the three actual test files and referencing the actual host project, with isolated transitive outputs, passed all 9/9 final tests. Its temporary project file was removed. Formatting was verified with `dotnet format whitespace --verify-no-changes` on changed C# files, launched through the isolated MSBuild wrapper with the same root and injected props. Existing NU1701 dependency warnings remain; Linux and other dependency-change forms were not verified.

The standalone host now offers an explicit `build <project.csproj> --refresh-runtime` option. After a successful compiler build it runs the existing runtime builder (`dotnet build --no-restore`) with the requested configuration and framework. A failed refresh returns an error instead of reporting build success; cancellation propagates. `runtimeRefresh=Succeeded` indicates that the SDK build succeeded, not that launch was verified. The reported artifact list is from the initial builder, not a fresh SDK artifact inventory. Restore dependencies beforehand; the SDK build executes project targets and can rewrite output. Without the option, behavior is unchanged. This does not change the separate builder service's contract or implement RID selection.

The real host option was tested on Windows/net10 against a stale manifest after adding an actual project dependency; the resulting executable loaded the dependency. Together with host failure, cancellation, option and existing adapter tests, 16/16 tests passed in the full isolated test project. Changed C# files passed the targeted whitespace check. Linux and other dependency-change forms remain unverified.
