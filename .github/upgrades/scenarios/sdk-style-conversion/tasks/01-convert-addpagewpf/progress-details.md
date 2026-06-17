# Progress Details: 01-convert-addpagewpf

## Summary
Converted `CSharpBible/AddPageWPF/AddPageWPF.csproj` from legacy format to SDK-style for WPF while preserving the `.NET Framework 4.8` target and the existing configuration-specific output layout.

## Files Modified
- `CSharpBible/AddPageWPF/AddPageWPF.csproj`
- `CSharpBible/AddPageWPF/Directory.Build.props`
- `.github/upgrades/scenarios/sdk-style-conversion/scenario-instructions.md`
- `.github/upgrades/scenarios/sdk-style-conversion/tasks/01-convert-addpagewpf/task.md`

## Build Result
- **Status**: Erfolg
- **Tool**: `MSBuild.exe` (`C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe`)
- **Release build**: `AddPageWPF -> C:\Projekte\GitHub\CSharp\bin\Release\AddPageWPF.exe`
- **Debug build**: `AddPageWPF -> C:\Projekte\GitHub\CSharp\bin\Debug\AddPageWPF.exe`
- **Warnings**: 0
- **Errors**: 0

## Test Result
- **Status**: übersprungen
- **Reason**: No project-specific test project for `AddPageWPF` was identified in the solution inventory, so there were no direct tests to run for this task.

## Conversion Notes
- Switched the project to `Microsoft.NET.Sdk.WindowsDesktop` with `UseWPF=true`.
- Kept `GenerateAssemblyInfo=false` so the existing `Properties/AssemblyInfo.cs` remains authoritative and avoids duplicate assembly metadata.
- Restored `AutoGenerateBindingRedirects=true` explicitly.
- Preserved the legacy Debug/Release output paths and disabled target-framework/runtime suffixes via `AppendTargetFrameworkToOutputPath=false` and `AppendRuntimeIdentifierToOutputPath=false`.
- Moved the custom intermediate output path into a project-local `Directory.Build.props` so it is applied early enough for SDK-style restore/build evaluation.
- Added `DefaultItemExcludes` for local `bin`/`obj` folders to prevent stale legacy-generated WPF files from being picked up by SDK default globs.

## Issues Encountered And Resolved
1. **MSB3539 warning**: `BaseIntermediateOutputPath` was being set too late in the SDK-style project.
   - **Resolution**: moved intermediate path properties into `CSharpBible/AddPageWPF/Directory.Build.props`.
2. **Unexpected `net48` output subfolder** after the initial conversion.
   - **Resolution**: disabled target framework and runtime identifier suffixes on the output path.
3. **Duplicate WPF/generated-code compilation errors** caused by stale local `obj` artifacts from the legacy layout no longer being excluded automatically.
   - **Resolution**: excluded local `bin`/`obj` folders from default SDK globs and removed the stale local `obj` folder before rebuilding.

## Done-When Verification
- `AddPageWPF.csproj` uses SDK-style syntax: **Yes**
- .NET Framework 4.8 target preserved: **Yes** (`net48`)
- Required WPF/resource/settings behavior preserved: **Yes**
- Configuration-specific output paths preserved: **Yes** (`bin\Debug\`, `bin\Release\`)
- Project builds successfully after conversion: **Yes**
