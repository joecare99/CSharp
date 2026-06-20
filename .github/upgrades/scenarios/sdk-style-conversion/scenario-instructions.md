# SDK-style Project Conversion

## Strategy
Convert all remaining legacy non-SDK-style projects in `CSharpBible.slnx` to SDK-style while preserving their current target frameworks and validating each converted project individually.

## Preferences
- **Flow Mode**: Automatic
- **Commit Strategy**: After Each Task

## Source Control
- **Source Branch**: master
- **Working Branch**: master
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## Build Tool Decisions
- **CSharpBible/AddPageWPF/AddPageWPF.csproj**: msbuild.exe (SDK-style WPF project targeting .NET Framework 4.8 with XAML and `.resx` resources)

## Decisions
- Expand the scenario scope from `AddPageWPF` only to all remaining legacy non-SDK-style projects in `CSharpBible.slnx`.
- Treat `AddPageWPF` as already converted and exclude it from the remaining conversion work.
- Keep working on `master` and leave the existing uncommitted changes untouched while performing the remaining SDK-style conversions.
