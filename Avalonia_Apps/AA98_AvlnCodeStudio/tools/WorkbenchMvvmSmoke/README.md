# Workbench MVVM smoke test

Run from the repository root in an interactive desktop session. The wrapper
sets `HostArtifactRoot` before MSBuild evaluates the solution and referenced
projects, so every project in the graph uses the host-specific artifact tree:

```powershell
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchMvvmSmoke\WorkbenchMvvmSmoke.csproj
```

On WSL/Linux, run the matching wrapper:

```bash
bash tools/build/dotnet-isolated.sh run --project tools/WorkbenchMvvmSmoke/WorkbenchMvvmSmoke.csproj
```

By default, artifacts are written beneath the shared workspace's
`artifacts/windows` or `artifacts/linux` directory, respectively. The scripts
also support a caller-selected `HostArtifactRoot` by editing the wrapper or
invoking `dotnet` directly with `-p:HostArtifactRoot=<absolute-path>`. Existing
`UpDir`-based invocations remain supported. If the Linux SDK is installed in a
nonstandard location, add it to `PATH` and set `DOTNET_ROOT` so MSBuild can
discover it.

The test opens a temporary synthetic C# project in the real Workbench, generates
AXAML bindings and a CommunityToolkit ViewModel through the normal review dialog,
including AutoCompleteBox text, ColorPicker color, TabControl selected-index and
MenuItem command bindings, checks editor/file
synchronization and workspace undo/redo, and builds the generated project against
Avalonia and the ColorPicker package. The restored C# 14/Toolkit 8.4.2 fixture
also verifies public partial observable properties and their initial values.
The Workbench and standalone designer probe the target project's actual Roslyn
compilation and source generator before using this syntax; if support cannot be
established (including multi-target projects), they retain the field-based form.
The smoke uses isolated configuration and deletes its fixture after the Workbench
exits. It is not a physical keyboard or
accessibility acceptance test.
