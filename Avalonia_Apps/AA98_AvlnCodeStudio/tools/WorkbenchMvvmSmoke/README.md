# Workbench MVVM smoke test

Run from the repository root in an interactive desktop session:

```powershell
dotnet run --project tools\WorkbenchMvvmSmoke\WorkbenchMvvmSmoke.csproj
```

On Linux, use `tools/WorkbenchMvvmSmoke/WorkbenchMvvmSmoke.csproj`. In a
shared Windows/WSL checkout, build with an isolated Linux artifacts root and
import the isolation props after `Directory.Build.props`; do not share `bin`
and `obj` outputs with Windows. If the Linux SDK is installed in a nonstandard
location, add it to `PATH` and set `DOTNET_ROOT` so MSBuild can discover it.

The test opens a temporary synthetic C# project in the real Workbench, generates
AXAML bindings and a CommunityToolkit ViewModel through the normal review dialog,
including AutoCompleteBox text, ColorPicker color, TabControl selected-index and
MenuItem command bindings, checks editor/file
synchronization and workspace undo/redo, and builds the generated project against
Avalonia and the ColorPicker package. It uses isolated configuration and deletes
its temporary fixture after the Workbench exits. It is not a physical keyboard or
accessibility acceptance test.
