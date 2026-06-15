# Assessment: SDK-style Conversion

## Scope
- Selected project: `CSharpBible/AddPageWPF/AddPageWPF.csproj`

## Projects to Convert
| Project | Path | packages.config | Custom Imports | Special Type | Risk |
|---------|------|----------------|----------------|-------------|------|
| AddPageWPF | CSharpBible/AddPageWPF/AddPageWPF.csproj | No | Standard `Microsoft.Common.props` and `Microsoft.CSharp.targets` | WPF desktop app | Medium |

## Already SDK-style (no action needed)
- none within selected scope

## Baseline
- Project builds: Yes
- Warning count: 0

## Key Findings
- Legacy non-SDK project file with explicit `Compile`, `Page`, `EmbeddedResource`, and `None` includes.
- WPF project type GUIDs are present; conversion should use `Microsoft.NET.Sdk.WindowsDesktop` with `UseWPF` enabled while keeping the existing .NET Framework target.
- `Properties/AssemblyInfo.cs` contains assembly metadata and `ThemeInfo`; conversion must avoid duplicate auto-generated assembly attributes.
- No `packages.config` file or custom post-build/import logic was found in the selected project.
- `App.config`, generated resource/settings files, and custom output paths should be preserved during conversion.
