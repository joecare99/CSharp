# XAML Decompiler Tool

## Status
- State: In Progress
- Goal: Create a WPF tool that reconstructs XAML and code-behind from generated C# source files.

## Background
The workspace contains generated MAUI source files such as `SEW.MOVIMONITOR.MainPage` and `SEW.MOVIMONITOR.App`. The requested tool should load a single generated `.cs` file, analyze the generated `InitializeComponent` pattern and related metadata, show a preview, and save reconstructed `<File>.xaml` and `<File>.xaml.cs` files.

## Scope
- Add a new WPF desktop project to the solution.
- Add a reusable core library for parsing and decompilation.
- Add automated MSTest coverage for parser and output generation.
- Keep the first implementation focused on the generator patterns currently present in this repository.

## Planned Architecture
- `XamlDecompiler.Core`
  - Source models
  - Parser for generated MAUI source files
  - XAML/code-behind reconstruction service
- `XamlDecompiler.App`
  - WPF UI for loading a file, previewing output, and saving reconstructed files
- `XamlDecompiler.Tests`
  - Parser and reconstruction tests

## Notes
- The implementation is heuristic and targets the generated source layout visible in the current repository.
- Complex nested templates may require follow-up improvements for exact 1:1 reconstruction.
- Parser hardening added regression coverage for `SEW.MOVIMONITOR.MainPage` SourceGen patterns: inline `RowDefinitionCollection` / `ColumnDefinitionCollection` assignments and primitive local variables reused as `ResourceDictionary` values.
- Parser hardening added regression coverage for `SEW.MOVIMONITOR.AppShell` Shell SourceGen patterns: `public` and `public partial` classes, `ShellContent` inserted through cast `ICollection<T>` adds including `GetValue(...ItemsProperty)` access and cast child arguments, direct `Route` assignments, and `DataTemplate(typeof(...))` constructor values.
- Parser hardening added regression coverage for `SEW.MOVIMONITOR.Pages.AboutHeaderContentView` SourceGen patterns: general static type-value queries in expression resolution, currently exercised by `Device.GetNamedSize(NamedSize.Title, label.GetType())` mapping to `FontSize="Title"`.
- Parser hardening added regression coverage for `SEW.MOVIMONITOR.Pages.AboutPage` resource SourceGen patterns: general resource-markup passthrough handling for values produced by `ProvideValue(...)`, currently exercised by `StaticResourceExtension` styles wrapped in generated ternary passthrough expressions.
- Validation: targeted regressions passed with `dotnet test XamlDecompiler.Tests/XamlDecompiler.Tests.csproj --no-restore --filter Decompile_AppShellSource_ReconstructsShellContentChild`, `dotnet test XamlDecompiler.Tests/XamlDecompiler.Tests.csproj --no-restore --filter Decompile_AboutHeaderContentView_ReconstructsNamedFontSizeValue`, and `dotnet test XamlDecompiler.Tests/XamlDecompiler.Tests.csproj --no-restore --filter Decompile_AboutPageStaticResourceStyle_ReconstructsGeneralResourceReference` (each 1/1 successful).
