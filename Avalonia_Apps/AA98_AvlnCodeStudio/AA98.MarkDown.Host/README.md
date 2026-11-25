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
This tests the integration of the selected packages, not syntax colors,
links, images, physical input, accessibility or all Markdown syntax.

On 2026-11-25 the mode passed on Windows and WSLg with net10, Avalonia 12.2.0,
Avalonia.AvaloniaEdit 12.0.0 and Markdown.Avalonia 12.0.2. Build warnings
including NU1701 remain. The new smoke file passed `dotnet format whitespace
--verify-no-changes`; checking it together with `Program.cs` reported existing
whitespace errors elsewhere in `Program.cs`.

Windows commands, run from the physical checkout in an interactive session:

```powershell
$root = Join-Path (Get-Location) 'artifacts\mrw'
$env:NUGET_PACKAGES = Join-Path (Get-Location) 'artifacts\pgw\p'
$p = Join-Path (Get-Location) 'tools\build\IsolatedArtifacts.props'
$proj = 'tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj'
dotnet restore $proj "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet build $proj --framework net10.0 --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet run --project $proj --framework net10.0 --no-build --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" -- --markdown-only
dotnet format whitespace . --folder --include tools\WorkbenchArchitectureSmoke\MarkdownRendererSmoke.cs --verify-no-changes --verbosity minimal
dotnet format whitespace . --folder --include tools\WorkbenchArchitectureSmoke\MarkdownRendererSmoke.cs tools\WorkbenchArchitectureSmoke\Program.cs --verify-no-changes --verbosity minimal
```

In WSLg with the Linux SDK:

```bash
export DOTNET_ROOT="$HOME/.dotnet" PATH="$HOME/.dotnet:$PATH"
root="$PWD/artifacts/mrl"
export NUGET_PACKAGES="$PWD/artifacts/fleet-package-graph-linux-84671c27/packages"
p="$PWD/tools/build/IsolatedArtifacts.props"
proj="tools/WorkbenchArchitectureSmoke/WorkbenchArchitectureSmoke.csproj"
dotnet restore "$proj" "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet build "$proj" --framework net10.0 --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" --verbosity minimal
dotnet run --project "$proj" --framework net10.0 --no-build --no-restore "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$p" -- --markdown-only
```
