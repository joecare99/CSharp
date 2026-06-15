# SDK-style Project Conversion

## Strategy
Convert the selected WPF project directly with the dedicated SDK-style conversion flow while preserving the current .NET Framework target.

## Preferences
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Task

## Source Control
- **Source Branch**: master
- **Working Branch**: sdk-style-conversion
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Build Tool Decisions
- **CSharpBible/AddPageWPF/AddPageWPF.csproj**: msbuild.exe (SDK-style WPF project targeting .NET Framework 4.8 with XAML and `.resx` resources)
