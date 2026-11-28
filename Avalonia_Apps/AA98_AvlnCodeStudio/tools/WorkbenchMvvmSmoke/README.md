# Workbench MVVM smoke test

Run from the repository root in an interactive desktop session. The wrapper
sets `HostArtifactRoot` before MSBuild evaluates the solution and referenced
projects, so every project in the graph uses the host-specific artifact tree:

```powershell
$root = Join-Path (Get-Location).Path ("artifacts\workbench-mvvm-windows-" + [guid]::NewGuid().ToString("N"))
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchMvvmSmoke\WorkbenchMvvmSmoke.csproj "-p:HostArtifactRoot=$root"
```

On WSL/Linux, run the matching wrapper:

```bash
root="$(pwd)/artifacts/workbench-mvvm-linux-$(date +%s%N)"
bash tools/build/dotnet-isolated.sh run --project tools/WorkbenchMvvmSmoke/WorkbenchMvvmSmoke.csproj "-p:HostArtifactRoot=$root"
```

By default, artifacts are written beneath the shared workspace's
`artifacts/windows` or `artifacts/linux` directory, respectively. The scripts
also support a caller-selected `HostArtifactRoot` through
`-p:HostArtifactRoot=<absolute-path>` on the wrapper. Existing
`UpDir`-based invocations remain supported. If the Linux SDK is installed in a
nonstandard location, add it to `PATH` and set `DOTNET_ROOT` so MSBuild can
discover it. Run from a physical workspace path rather than a path traversing a
junction or symbolic link: configuration layer security rejects reparse points.
The Workbench's version-control discovery also requires an `svn` client on
`PATH` when the fixture is inside an SVN working copy.

The test opens a temporary synthetic C# project in the real Workbench, generates
AXAML bindings and a CommunityToolkit ViewModel through the normal review dialog,
including AutoCompleteBox text, ColorPicker color, TabControl selected-index and
MenuItem command bindings. A named ToggleButton with a literal checked state
and command parameter is checked for its TwoWay state binding, generated
parameterized command and initial ViewModel value. The smoke checks editor/file
synchronization and workspace undo/redo, and builds the generated project against
Avalonia and the ColorPicker package. The restored C# 14/Toolkit 8.4.2 fixture
also verifies public partial observable properties and their initial values.
The Workbench and standalone designer probe the target project's actual Roslyn
compilation and source generator before using this syntax; if support cannot be
established (including multi-target projects), they retain the field-based form.
The fixture's restore and build also use the isolation wrapper with an artifact
root unique to that fixture. The smoke uses isolated configuration and deletes
its fixture after the Workbench exits. It invokes the review buttons
programmatically; it is not a physical keyboard, screen reader or accessibility
acceptance test.
The smoke process and its synthetic fixture builds opt out of Avalonia build
telemetry; this does not change the environment of the invoking shell.
On Windows, an earlier run completed the checks but `Directory.Delete` failed
with `IOException` because the fixture directory was in use. Avalonia
BuildServices 11.3.2 starts a detached collector in the build task's current
directory, which can retain that directory; the fixture build's task directory
was independently verified. The locking process in that earlier run was not
identified, so the collector is a plausible cause, not a confirmed attribution.
With the smoke-local opt-out, a fresh Windows run completed and cleaned up.

The incremental pass also covers simple `CompiledBinding` and `ReflectionBinding`
paths on catalog-supported properties and commands. Existing AXAML stays
unchanged, and the ViewModel edit is reviewed, cancellable and undoable. Explicit
sources and nested DataContext/template scopes remain rejected rather than
being guessed. Nested `x:DataType` scopes are also treated conservatively:
bindings there are not planned against the root ViewModel, and fresh generation
refuses named supported controls in such a scope. Complex paths and unsupported
controls are not generated.

The smoke also edits a generated root-DataContext view using the shared visual
binding editor. It reviews and applies a compiled property binding and a
reflection command binding with their new ViewModel members, checks both source
files and editor synchronization through workspace undo/redo, and rebuilds the
fixture. The review buttons are invoked programmatically; this is not physical
UX acceptance.

The Workbench smoke also refreshes the selected text binding's ViewModel
context and checks that the bound member is recognized without losing a pending
mode edit. It also opens the standalone host window, checks its shared designer,
and exercises the refresh and designer undo/redo programmatically. These checks
do not verify physical mouse or keyboard interaction, screen-reader behavior, or
native desktop integration.

The Workbench fixture also edits the open ViewModel buffer without saving,
changes the open AXAML buffer to bind to the new member, and holds the real
semantic compilation with its test hook while the user changes binding mode and
trigger. It checks the resulting member selection, designer undo/redo, and that
neither file on disk changed. A second delayed analysis is superseded by a
newer ViewModel buffer and must not overwrite it. This exercises the Workbench
buffer overlay; the standalone host has no separate C# editor buffer.

The referenced-project fixture uses a consumer view with a runtime DataContext
from a separate `ProjectReference` library. Its real Workbench C# tab contains
an unsaved replacement member. A controlled compilation gate holds the context
while mode and trigger are edited, then the smoke checks selection, AXAML
apply/undo/redo, and byte-identical backing files. It also checks A→B→A buffer
changes, cancellation, explicit discard/close and reopening the dirty library
tab. In the still-open workspace it edits the consumer project in its normal
text editor, confirms an unsaved project buffer does not alter the saved
reference context, then saves removal, re-addition and a false reference
condition. No test-side cache invalidation or workspace reopening is used.
The save occurs while the designer is active and a synthetic gate holds an
older compilation; cancellation, pending mode/trigger, the unsaved library
buffer and byte-identical ViewModel/AXAML files are checked.

Successful AA98 text-editor saves of `.csproj` files raise a save event. The
Workbench cancels the active binding analysis, invalidates affected semantic
snapshots (including consumers), then queues the active designer again.
Unsaved project buffers are not evaluated; Roslyn loads the saved project
file. External edits, imports and changes performed outside that editor save
event are not watched and have no automatic refresh guarantee. Use **Edit →
Refresh binding context from saved projects** after externally editing a saved
workspace project's `ProjectReference`. It invalidates workspace project
snapshots and queues the active Workbench designer without saving or reloading
editor buffers. Unsaved `.csproj` XML is not evaluated; unsaved C# buffers
remain overlaid. This is not a general watcher, workspace project-list reload,
import-change guarantee, or standalone-host refresh. The service's
explicit `RefreshProjectContextAsync` is a separate host API: it invalidates
snapshots but does not itself notify the designer. No standalone-host project
file watcher is implied. The focused
`ProjectSave_QueuesReferenceSnapshotRefreshBehindRunningAnalysis` test holds
the service gate independently and checks that saving waits to invalidate the
cached snapshot, while an unsaved project edit does not invalidate it.
The service gate serializes compilations: a canceled older compilation is
released before the newer one starts, not completed out of order. It does not
itself simulate an already-computed stale result arriving after the newer result.

The external-reference scenario writes the consumer project on disk while the
workspace remains open. An ordinary C#-buffer refresh still sees the cached
reference until the Workbench refresh command is invoked. It then verifies
external removal, re-addition and a false condition, with a dirty project
editor and dirty library C# editor left untouched. A computed old result is
held until after the new result is published; mode/trigger, designer
apply/undo/redo, and switching workspaces while invalidation waits are checked.
The menu command is invoked programmatically, not by physical input.

On 2026-11-28 this external-reference scenario passed on Windows net10.0 with
`.artifacts-external-ref-win-final-41308b85cceb40b3b325552a868fe0d7` and on
Ubuntu 26.04 WSLg net10.0 with
`.artifacts-external-ref-linux-final-1795866033020689144`. Builds retained the
existing NU1701, CS8600, CS8603, CA1416, AVA2003 and dependency-injection
target-framework support warnings. The two focused binding-context tests passed
on net10.0 on both hosts. An earlier multi-framework invocation
returned failure despite its net10.0 tests passing: the Windows net8 test host
hit a System.Text.Json `MissingMethodException`, and WSL lacked the net8/9
runtimes. A separately quoted WSL `--reference-only` invocation above passed;
an earlier incorrectly quoted invocation entered the full smoke and failed
building its unrelated Avalonia fixture with a resource-file lock.

The reference smoke additionally gates the Workbench refresh **after** the real
semantic service returns and releases its compilation gate, but before the
Workbench checks and publishes the result. The hook passes the result unchanged,
ignores cancellation while holding it, and does not decide whether it is stale.
The smoke waits for a newer refresh to finish publishing before releasing the
old one, then awaits the old refresh and checks that no binding-row publication
occurred. It covers a replacement unsaved C# buffer, an A→B→A buffer transition
where B and the second A both publish, tab and AXAML-path changes, a workspace
switch and return, an unsaved C# buffer at a different path, cancellation, and a
late synthetic exception. Mode and trigger edits survive; the fixture also
checks the backing files and designer undo/redo. The per-request cancellation
token is the existing identity guard in these refresh cases; these checks do not
prove a monotonic revision scheme.

This gate exercises the Workbench refresh-to-designer handoff, not the separate
missing-member apply/review path or the standalone host refresh. The AXAML path
case switches tabs; the C# path case explicitly discards the synthetic dirty tab
and opens another file, not a rename. It does not exercise every tool window.
The workspace case reopens the original workspace after another project. All
desktop interaction is synthetic, not physical acceptance.

On 2026-11-28 the expanded `--reference-only` smoke passed on Windows with
net10.0 from the physical workspace using the isolated artifact root
`.artifacts-fleet-binding-late-physical-win-eb016278556846e6adada4da404882da`.
It also passed on Ubuntu 26.04 under WSLg with SDK 10.0.404 and the isolated
root `.artifacts-fleet-binding-late-linux-1795853573678876402`.
Three focused binding/project-save tests and targeted whitespace verification
for the Workbench and smoke files also passed. Existing NU1701, CS8600, CS8603,
CA1416, AVA2003 and dependency-injection support warnings remained.

For just this fixture, append `-- --reference-only` to either command above.

On 2026-11-28, the `--reference-only` run completed with exit code 0 on
Ubuntu 26.04 under WSLg (`DISPLAY=:0`, .NET SDK 10.0.404), using the fresh
Linux artifact root
`.artifacts-workbench-reference-wslg-500753e86b464f5383b2f54b89921074`.
It reported successful referenced-overlay, cancellation, unsaved-project and
saved reference removal/addition/condition checks. No SVN client was on this
run's Linux `PATH`; it did not block this fixture. The build emitted existing
NU1701, CS8600, CS8603, CA1416 and AVA2003 warnings, plus dependency-injection
net8/net9 support warnings. This programmatic run is not physical desktop
acceptance and does not change the external-edit limitations above.
