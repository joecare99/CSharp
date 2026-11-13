# Development review findings

Review scope: Windows/.NET 10 build and test run, documented Workbench desktop smoke flows, launch/screenshot check, static review of application and shared project setup, with emphasis on Windows and Linux/WSL portability. Findings are ordered by impact. No source changes were made during this review.

## High priority

### Planning document writes can escape the selected repository

**Location:** `AA98_AvlnCodeStudio.Planning.Local/Services/LocalPlanningProvider.cs:88-105`, `:142-156`

`item.SourcePath` is accepted as-is and combined with the repository root. Rooted paths or traversal components such as `..` can resolve outside the selected repository. The resulting directory can be created and `File.WriteAllTextAsync` can overwrite a file at that external location. The generated fallback path also relies on input values rather than verifying the final canonical path remains within the intended planning root. A symlink inside the root may also redirect the write outside it.

**Recommendation:** Canonicalize the repository root and every target; reject rooted paths and paths outside the allowed root using a separator-aware containment check. Resolve/check symlinks (or use a write strategy that prevents following them) before creating directories or writing. Add tests for rooted paths, traversal, and symlink escape attempts, asserting no external file is created or changed.

**Status after follow-up:** Addressed in `LocalPlanningProvider.WriteAsync`. Rooted and parent-traversal paths are rejected, planning and generated fallback paths are constrained to the selected planning root, and existing symlink targets are resolved and checked before directory creation and writing. Regressions cover rooted paths, POSIX and Windows-style traversal, generated fallback paths, planning-root symlinks, and document-directory symlinks. `LocalPlanningProviderTests` passes 9/9 on Windows/net10 and isolated WSL Ubuntu 26.04/net10.

## Medium priority

### Windows and Linux can collide in shared build artifacts

**Location:** `AA98_AvlnCodeStudio.props:1-12`, `Solution.props:3-5`, `tools/ReleaseAcceptance/README.md:46-51`, `tools/WorkbenchMvvmSmoke/README.md:9-13`

Build output and intermediate paths are shared outside the checkout. The project explicitly warns that Windows and Linux must not share `bin`/`obj` artifacts, and recommends an early-import isolation props file for WSL. Without consistently applying that setup, restore/build results can be overwritten or reused across operating systems, leading to confusing failures or invalid platform-specific outputs.

**Recommendation:** Make artifact roots configurable and host-specific by default (for example, separate Windows and Linux roots), and provide a checked-in, supported invocation/script that applies the isolation settings before project evaluation. Document the expected Windows and WSL commands together. Do not rely on users remembering a manual props-file procedure.

**Status after follow-up:** Addressed with host-specific defaults under the shared workspace's `artifacts/windows` and `artifacts/linux` roots, plus checked-in PowerShell and Bash wrappers that pass `HostArtifactRoot` before MSBuild evaluation. The application, shared library references, smoke projects, and external Workbench.Builder project resolve into host-separated paths. Windows and WSL wrapper runs of `LocalPlanningProviderTests` both pass 9/9; property evaluation confirms distinct roots. No existing build artifacts were removed.

### Local planning scaffold generates a Visual Studio-only shared project

**Location:** `AA98_AvlnCodeStudio.Planning.Local/Services/LocalPlanningProjectScaffolder.cs:54-60`, `:73-88`

The scaffold writes `.shproj` and `.projitems` files with Visual Studio CodeSharing imports under `MSBuildExtensionsPath32`. These targets are not part of a normal Linux .NET SDK installation, so the generated planning project is not a portable Linux/WSL project even though the main app targets those platforms.

**Recommendation:** Generate an SDK-style project format supported by the .NET SDK on both operating systems, or clearly label this feature as Windows/Visual Studio-specific and keep it out of the Linux-supported workflow. Include an automated Linux SDK evaluation/build test for the generated output.

### Embedded terminal is pipe-backed, not a terminal emulator session

**Location:** `AA98.Terminal.Host/Services/HostedTerminalProcessFactory.cs:18-25`, `AA98.Terminal.Host/Services/HostedTerminalProcess.cs:59-63`

The shell is started with redirected stdin/stdout/stderr and input is sent one line at a time. It has no pseudo-terminal (PTY). Interactive shell prompts, job control, terminal modes, curses/full-screen programs, and many REPLs will not behave like a normal terminal on Windows or Linux/WSL.

**Recommendation:** Use ConPTY on Windows and a Unix PTY on Linux/WSL behind a platform-neutral terminal-session abstraction. If the feature is intentionally a command runner rather than a terminal, describe it as line-oriented and disable/avoid presenting unsupported interactive behaviors.

### Git process can deadlock and may survive cancellation

**Location:** `AA98_AvlnCodeStudio.Versioning.Git/Services/GitCommandRunner.cs:34-37`

The implementation reads all stdout before reading stderr. If Git fills the stderr pipe while stdout remains open, Git can block on stderr and the caller can wait indefinitely on stdout. Cancellation during stream reads or `WaitForExitAsync` also does not explicitly terminate and reap the child process.

**Recommendation:** Drain stdout and stderr concurrently, await both drains and process exit, and on cancellation kill the process tree and wait for termination in a `finally` path. Test large output on both streams and cancellation while Git is running.

### Build toolchain dependencies are noisy and show compatibility risk

**Location:** `AA98_AvlnCodeStudio/Directory.Packages.props:10`, `Libraries/BaseLib/Models/FileProxy.cs:14-15`; affected project references appear in the .NET 10 build output.

The .NET 10 restore/build emitted `NU1701` for `Microsoft.Build 18.11.0` and `Microsoft.IO.Redist 6.1.0` because they restored against .NET Framework assets. It also emitted package support warnings for `Microsoft.NET.StringTools 18.11.0` on `net10.0`. These warnings span the Workbench, builder, designer, and test projects and mean a passing local build is not evidence that these combinations are supported. Separately, CA1416 flagged a Windows ACL call that is reachable on all platforms.

**Recommendation:** Align Microsoft.Build, StringTools, Roslyn, and SDK versions with officially supported .NET 10 assets; verify runtime asset selection on Windows and Linux. Guard the ACL call with `OperatingSystem.IsWindows()` or move it behind a platform-specific implementation with a non-Windows behavior. Treat warning reduction as a tracked quality gate.

## Low priority

### Main window minimum size is inflexible

**Location:** `AA98_AvlnCodeStudio.UI/Views/MainWindow.axaml:15-18`

The main window sets a fixed minimum of 900 x 600 logical pixels. Compact screens, high scaling factors, or a constrained WSLg display can make it impossible to fit the full window.

**Recommendation:** Reduce the minimum size, make workspace panes collapse/scroll at smaller sizes, and verify at small logical resolutions and high DPI on both platforms.

### Avalonia MVVM template pins an older package version

**Location:** `AA98_AvlnCodeStudio.Model/ProjectTemplates/avalonia-mvvm.json:5-6`, `Packages.props:10-18`

The generated Avalonia project template pins core packages to Avalonia 12.1.4, while central solution package versions are 12.1.5. Newly generated apps can therefore start behind the version used by the tool itself.

**Recommendation:** Keep template package versions synchronized with the supported central version or generate a central package management file so the versions have one source of truth. Add a template validation smoke that restores/builds its output.

## Verification record

- Environment: Windows 10, .NET SDK 10.0.403 / runtime 10.0.14.
- `dotnet test .\AA98_AvlnCodeStudio.slnx --framework net10.0`: the first run had one failing secrets-vault contract test. Rerunning the secrets test project passed 15 tests with 4 skipped; the other test assemblies passed (Workbench 711 passed, 1 skipped; editor 17 passed; AI 12 passed; terminal 10 passed). The filtered vault test passed separately. The original failure was therefore transient/not reproduced, but should be monitored in CI.
- `tools/WorkbenchArchitectureSmoke`, `tools/WorkbenchMvvmSmoke`, and `tools/ProjectSettingsReviewSmoke` all completed successfully on Windows. They perform programmatic UI actions and do not replace physical keyboard or screen-reader acceptance.
- The Workbench launched successfully. Screenshot: `C:\Users\Mir\AppData\Local\Temp\mammouth\workbench-smoke.png`.
- WSL was present, but Ubuntu 22.04 had no .NET SDK and no GUI display variables in this session, so Linux-native build/smoke execution was blocked. `tools/ReleaseAcceptance/README.md:93-103` records prior Linux .NET 10 and WSLg smoke passes; those are historical records, not a fresh run in this review.
- No lint or typecheck command was found in the inspected project guidance. Build warnings included legacy package compatibility, platform API, nullable, obsolete API, and analyzer warnings. No lint-specific gate could be run.
