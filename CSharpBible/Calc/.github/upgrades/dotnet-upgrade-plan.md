# .NET 8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade UWP_00_Test\UWP_00_Test.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name | Description |
|:-------------|:-----------:|

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### UWP_00_Test\UWP_00_Test.csproj modifications

Project properties changes:
  - Include shared props: `..\MVVM_Tutorial.props`
  - Enable nullable reference types: `nullable` set to `enable`
  - Migrate project to SDK style targeting .NET 8 (Windows): adopt `Microsoft.NET.Sdk` with appropriate target framework if required by the app model

Other changes:
  - Review and adjust Windows application model dependencies as needed for .NET 8 (e.g., Windows App SDK / WinUI migration if applicable)
