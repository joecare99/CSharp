# 01-convert-addpagewpf: Convert AddPageWPF to SDK-style

Replace the legacy project structure in `CSharpBible/AddPageWPF/AddPageWPF.csproj` with an SDK-style WPF project while preserving the existing .NET Framework target, output paths, application entry points, generated resource/settings files, and assembly metadata behavior. The conversion must keep the project building in Visual Studio without changing the application's runtime behavior.

Key concerns are the WPF project type, the current `AssemblyInfo.cs` metadata that can conflict with SDK-generated attributes, and preserving configuration-specific output paths that differ from SDK defaults.

**Done when**: `AddPageWPF.csproj` uses SDK-style syntax, keeps the .NET Framework 4.8 target, preserves required WPF/resource/settings behavior, and the project builds successfully after conversion.

## Research Findings

### Scope assessment
- Single-project conversion only: `CSharpBible/AddPageWPF/AddPageWPF.csproj`
- No project-to-project references were found for `AddPageWPF`; blast radius is limited to this WPF application.
- Topological ordering was obtained from the workspace ordering tool; `AddPageWPF.csproj` is an independent leaf for this task and can be converted in isolation.
- Decomposition is not required because this is one uniform SDK-style conversion concern on a single project.

### Current project shape
- The project is a legacy WPF desktop application targeting `.NET Framework 4.8` (`<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>`).
- It currently defines `OutputType=WinExe`, `RootNamespace=AddPageWPF`, `AssemblyName=AddPageWPF`, `AutoGenerateBindingRedirects=true`, and deterministic compilation.
- Custom build paths must be preserved:
  - `BaseOutputPath=..\..\bin\$(MSBuildProjectName)\`
  - `BaseIntermediateOutputPath=..\..\obj\$(MSBuildProjectName)\`
  - `IntermediateOutputPath=..\..\obj\$(MSBuildProjectName)\`
  - Debug `OutputPath=..\..\bin\Debug\`
  - Release `OutputPath=..\..\bin\Release\`
- The legacy project explicitly includes WPF items and generated files:
  - `App.xaml`, `App.xaml.cs`
  - `MainWindow.xaml`, `MainWindow.xaml.cs`
  - `Properties/Resources.resx`, `Properties/Resources.Designer.cs`
  - `Properties/Settings.settings`, `Properties/Settings.Designer.cs`
  - `App.config`

### Assembly metadata and generated code
- `CSharpBible/AddPageWPF/Properties/AssemblyInfo.cs` contains assembly metadata attributes, `ThemeInfo`, `ComVisible(false)`, and explicit version attributes.
- SDK-style conversion must avoid duplicate generated assembly attributes; `GenerateAssemblyInfo` will likely need to remain disabled so the existing `AssemblyInfo.cs` stays authoritative.
- `ThemeInfo` remains required for WPF resource lookup and must continue to come from `AssemblyInfo.cs`.

### Dependencies and build constraints
- Only framework references are used (`System`, `System.Xaml`, `WindowsBase`, `PresentationCore`, `PresentationFramework`, etc.); there is no `packages.config` and no package migration work is required.
- No custom imports beyond the standard legacy `Microsoft.Common.props` / `Microsoft.CSharp.targets` were found.
- Because the converted project is SDK-style WPF targeting .NET Framework with XAML and `.resx` resources, project validation should use full Visual Studio MSBuild rather than `dotnet build`.

### Files expected to change
- `CSharpBible/AddPageWPF/AddPageWPF.csproj`

### Files to verify but not necessarily modify
- `CSharpBible/AddPageWPF/Properties/AssemblyInfo.cs`
- `CSharpBible/AddPageWPF/App.config`
- `CSharpBible/AddPageWPF/Properties/Resources.resx`
- `CSharpBible/AddPageWPF/Properties/Settings.settings`

### Test coverage
- No project-specific test project for `AddPageWPF` was identified in the solution inventory.
- Validation for this task is therefore project build validation only; progress details must explicitly state that no project-specific tests exist.

### Execution plan
1. Convert `AddPageWPF.csproj` with the dedicated SDK-style conversion tool.
2. Review the converted project for WPF/resource/settings behavior and assembly attribute generation.
3. Apply only minimal follow-up fixes required by the conversion.
4. Build the project directly with Visual Studio MSBuild and resolve any conversion-related warnings or errors.
5. Record results in `progress-details.md` and then finalize the task through the orchestrator.
