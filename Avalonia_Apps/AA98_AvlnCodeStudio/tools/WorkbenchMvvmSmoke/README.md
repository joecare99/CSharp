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
