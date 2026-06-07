# ConsoleLib

Rich, testable console UI toolkit for .NET / Windows terminals. Provides an abstraction (`IConsole`) plus higher level controls (Label, TextBox, ListBox, Application, layout helpers) with a simple invalidation + message queue system. Targets multiple frameworks including classic .NET Framework (for legacy host apps) and modern .NET 6–9.

## Purpose
Enable structured, MVVM-friendly console applications (prototyping, headless admin tools, data / search demos) without dropping to raw `System.Console` calls. Focus on:
- Composability of controls
- Deterministic repainting (no flicker)
- Simple binding hooks (lightweight, reflection-based)
- Modern .NET target set for the reusable core library

## Key Features
- `Application` root control with message loop integration.
- Focus / active control handling.
- Text input with caret, multi-line editing, navigation.
- List rendering (scrolling, selection – extensible).
- Basic 2?way binding support for `TextBox` (model <-> text) and (extension ready) selection.
- Drawing API via `TextCanvas` (rect fill, bordered boxes, character output).
- Separation of concerns through `IConsole` and `IWidgetSet` abstractions.
- Backend-specific rendering and host-loop implementations can live in separate projects such as `ConsoleLib.ExtCon` or `ConsoleLib.WinForms`.

## Targets
```
net8.0; net9.0; net10.0
```

## Getting Started
```bash
# Add project reference or copy NuGet once published
# Example (from a consumer project folder):
# dotnet add reference ../../CSharpBible/Libraries/ConsoleLib/ConsoleLib.csproj
```

## Basic Usage
Reference the core project plus a concrete widget-set backend such as `ConsoleLib.ExtCon`, then create the application with an `IWidgetSet` implementation.

## Binding Example
```csharp
// viewModel implements INotifyPropertyChanged with property SearchText
var box = new TextBox { Parent = app, Dimension = new(0,1,40,1), MultiLine = false };
box.BindTwoWay(viewModel, nameof(viewModel.SearchText));
```

## Design Notes
- Redraws are explicit (`Invalidate`) -> message queue flush ensures stable frame.
- Minimal allocations per frame; string building localized.
- Thread safety: coarse `lock` in drawing primitives (adequate for UI thread use, not for high-frequency multi-thread rendering).
- The core library intentionally no longer contains the concrete ExtendedConsole-backed widget set.

## Roadmap
- Horizontal / vertical layout managers.
- Improved ListBox virtualization & selection binding promoted to public API.
- Color themes + style presets.
- Optional diff-based redraw to minimize console cursor movement.

## Contributing
1. Follow repository AI / code style guidelines.
2. One public type per file.
3. PR must include scenario description and (if feasible) a small demo snippet.

## License
(Insert license notice here – e.g. MIT / proprietary.)
