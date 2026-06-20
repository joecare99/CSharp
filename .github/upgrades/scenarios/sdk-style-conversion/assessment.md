# Assessment: SDK-style Conversion

## Scope
- Selected project: `CSharpBible/AddPageWPF/AddPageWPF.csproj`

## Projects to Convert
| Project | Path | packages.config | Custom Imports | Special Type | Risk |
|---------|------|----------------|----------------|-------------|------|
| AddPageWPF | CSharpBible/AddPageWPF/AddPageWPF.csproj | No | Standard `Microsoft.Common.props` and `Microsoft.CSharp.targets` | WPF desktop app | Medium |
| CSV_Viewer | CSharpBible/CSV_Viewer/CSV_Viewer.csproj | No | `Microsoft.Common.props`, `Microsoft.CSharp.targets` | WinForms desktop app | Medium |
| CSV_ViewerTest | CSharpBible/CSV_ViewerTest/CSV_ViewerTest.csproj | No | Legacy MSTest/NuGet `.props` and `.targets`, `Microsoft.CSharp.targets` | MSTest unit test project | Medium |
| CSharpBibleTest | CSharpBible/CSharpBibleTest/CSharpBibleTest.csproj | No | Legacy MSTest/NuGet `.props` and `.targets`, `Microsoft.CSharp.targets` | MSTest unit test project | Medium |

## Already SDK-style (no action needed)
- none within selected scope

## Baseline
- Solution builds: No
- Warning count: n/a because the baseline solution currently fails with unrelated errors outside the conversion scope

## Key Findings
- Five remaining solution projects still use legacy non-SDK project files with `ToolsVersion`-based MSBuild format.
- `CSV_Viewer` is a .NET Framework 4.8 WinForms application with explicit source/resource includes, `Properties/AssemblyInfo.cs`, and custom Debug/Release output paths that must be preserved.
- `CSV_ViewerTest` and `CSharpBibleTest` are legacy MSTest projects targeting .NET Framework 4.8 and importing test/NuGet `.props` and `.targets`; conversion must preserve their test behavior while moving package usage fully into SDK-style project metadata.
- None of the remaining legacy solution projects use `packages.config`, so package migration risk is limited to preserving direct assembly/package references inside the converted SDK-style files.
- The full solution baseline is already red because of unrelated compile/resource issues in other projects, so validation should focus on building each converted project directly and avoiding regressions in the touched test projects.
- A separate file `CSharpBible/DB/MdbBrowser/MdbBrowser2.csproj` is legacy-style on disk but is not referenced by `CSharpBible.slnx`; it is excluded from this scenario scope.
