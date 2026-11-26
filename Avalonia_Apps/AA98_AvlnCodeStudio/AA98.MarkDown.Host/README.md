# AA98.MarkDown.Host

AA98.MarkDown.Host is a lightweight Avalonia desktop host for editing multiple Markdown files in tabs.

## Features

- New markdown document creation
- Open existing `.md` files by path
- Save and Save As for active tab
- Multi-tab document handling
- Open markdown links from document content into additional tabs

## Notes

- File open/save is path-based in this first host slice.
- HTTP/HTTPS links are intentionally not opened by this host.

## Save and external changes

The host tracks dirty state by comparing the buffer with its last loaded or
saved text. Reopening an already open path selects its existing tab rather than
reloading it. Closing a dirty tab or window asks to save, discard or cancel;
cancelling leaves the remaining tabs open. These close prompts are not
external-file conflict checks.

There is currently no documented or implemented external-change detection,
comparison, warning or conflict resolution for Save or Save As. In a disposable
fixture on 2026-11-25, after a dirty buffer's file was changed externally,
Save wrote the buffer over that change and marked the tab clean. The choice of
future conflict handling is open; do not rely on the host to protect external
changes. The low-level `CloseSelectedDocument` view-model method explicitly
closes without the window's save checks.

Save As captures the original tab before opening the picker. Switching tabs
does not redirect the save or change the later selection's path input. If that
tab changes, is removed or replaced, the window closes, or its data context
changes while the picker is pending, the operation is abandoned. Cancelling
the picker leaves the document and files unchanged; during a close request it
also prevents closing. These checks do not detect external file changes or
change the existing overwrite behavior for the selected destination.

## Package compatibility

AA98 imports the shared `..\Packages.props` and locally overrides only
`Avalonia.AvaloniaEdit` to 12.0.0 and `Markdown.Avalonia` to 12.0.2.
Avalonia itself remains at the shared version (currently 12.2.0). On
2026-11-25, NuGet.org's published stable versions did not include 12.2.0
for either extension: the latest stable versions were 12.0.0 and 12.0.2,
respectively ([AvaloniaEdit versions](https://api.nuget.org/v3-flatcontainer/avalonia.avaloniaedit/index.json),
[Markdown versions](https://api.nuget.org/v3-flatcontainer/markdown.avalonia/index.json)).

For net8.0 and net10.0, the published dependency metadata declares
Avalonia >= 12.0.0 for [AvaloniaEdit 12.0.0](https://www.nuget.org/packages/Avalonia.AvaloniaEdit/12.0.0)
and for Markdown's [Tight](https://www.nuget.org/packages/Markdown.Avalonia.Tight/12.0.2),
[SyntaxHigh](https://www.nuget.org/packages/Markdown.Avalonia.SyntaxHigh/12.0.2),
[Html](https://www.nuget.org/packages/Markdown.Avalonia.Html/12.0.2) and
[Svg](https://www.nuget.org/packages/Markdown.Avalonia.Svg/12.0.2) 12.0.2
components. SyntaxHigh also requires AvaloniaEdit >= 12.0.0. These are minimum
version constraints, not proof of complete runtime compatibility with every
later Avalonia release. The overrides apply only to projects evaluating AA98's
`Directory.Packages.props`; referenced projects with their own package props,
including the standalone ImageEditor tree, evaluate their own versions.

## Renderer smoke

The Workbench architecture smoke's `--markdown-only` mode starts this host's
real Avalonia application and window, without starting the Workbench or image
editor. It opens a disposable Markdown file in the host and checks the laid-out
`MarkdownScrollViewer` visual tree: a heading (the editor maps `#` to
`Heading2`), paragraph text and a fenced C# block backed by AvaloniaEdit.
It then inserts text into the editor's document without saving, verifies the
rendered paragraph, dirty state and unchanged file, and checks undo and redo
before switching tabs. A second disposable file is opened and edited; switching
between the two tabs checks each dirty buffer and its rendered paragraph, with
both files still unchanged on disk. Fixture files are created with unique names
in the working directory and deleted when the host exits.

The smoke then changes the first fixture on disk while its buffer is dirty,
reopens its path and verifies the existing buffer is retained. It cancels the
actual dirty-tab and dirty-window prompts and verifies both buffers and disk
files remain intact. Finally it reports the observed Save outcome without
asserting an external-conflict policy.
The extended cancellation and Save observation passed on Windows and WSLg
with net10 on 2026-11-25 using isolated outputs and the regular package graph.

The smoke also uses a controlled asynchronous fake save picker in real host
windows. It checks a pending picker across tab switches, an unchanged context,
cancellation, changed or restored document state, removed or replaced tabs,
data-context replacement, window closure, and close-tab/window save flows.
The same tab-switch test failed before the fix. The prompts and windows are
real Avalonia controls, but the picker is not a native picker.
These picker and close-flow checks passed on Windows and WSLg with net10 on
2026-11-25 using separate isolated build outputs.

The smoke uses document API edits and view-model selection, not physical
keyboard or mouse input; it does not check undo history across tab switches,
native picker behavior, discard decisions, Save As conflicts,
syntax colors, links, images, accessibility or all Markdown
syntax. On 2026-11-25 the extended mode passed on Windows and WSLg with net10,
Avalonia 12.2.0, Avalonia.AvaloniaEdit 12.0.0 and Markdown.Avalonia 12.0.2,
using the regular package graph and isolated build outputs. Build warnings
including NU1701 and the Microsoft.Build.Utilities.Core net10 support warning
remain. The changed smoke file passed `dotnet format whitespace
--verify-no-changes`.

Windows commands, run from the physical checkout in an interactive session:

```powershell
$root = Join-Path (Get-Location) 'artifacts\markdown-live-example-win'
$p = Join-Path (Get-Location) 'tools\build\IsolatedArtifacts.props'
$proj = 'tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj'
dotnet restore $proj "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet build $proj --framework net10.0 --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet run --project $proj --framework net10.0 --no-build --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" -- --markdown-only
dotnet format whitespace . --folder --include tools\WorkbenchArchitectureSmoke\MarkdownRendererSmoke.cs --verify-no-changes --verbosity minimal
```

In WSLg with the Linux SDK:

```bash
export DOTNET_ROOT="$HOME/.dotnet" PATH="$HOME/.dotnet:$PATH"
root="$PWD/artifacts/markdown-live-example-linux"
p="$PWD/tools/build/IsolatedArtifacts.props"
proj="tools/WorkbenchArchitectureSmoke/WorkbenchArchitectureSmoke.csproj"
dotnet restore "$proj" "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet build "$proj" --framework net10.0 --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet run --project "$proj" --framework net10.0 --no-build --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" -- --markdown-only
```
