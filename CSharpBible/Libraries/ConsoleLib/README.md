# ConsoleLib

Rich, testable console UI toolkit for .NET / Windows terminals. Provides an abstraction (`IConsole`) plus higher level controls (Label, TextBox, ListBox, Application, layout helpers) with a simple invalidation + message queue system. Targets multiple frameworks including classic .NET Framework (for legacy host apps) and modern .NET 6–9.

## Purpose
Enable structured, MVVM-friendly console applications (prototyping, headless admin tools, data / search demos) without dropping to raw `System.Console` calls. Focus on:
- Composability of controls
- Deterministic repainting (no flicker)
- Simple binding hooks (lightweight, reflection-based)
- Cross-target (net462+ through net9.0)

## Key Features
- `Application` root control with message loop integration.
- Focus / active control handling.
- Text input with caret, multi-line editing, navigation.
- List rendering (scrolling, selection – extensible).
- Basic 2?way binding support for `TextBox` (model <-> text) and (extension ready) selection.
- Drawing API via `TextCanvas` (rect fill, bordered boxes, character output).
- Separation of concerns through `IConsole` abstraction (enables mocking and testing without real console).

## Targets
```
net462; net472; net48; net481; net6.0-windows; net7.0-windows; net8.0-windows; net9.0-windows
```

## Getting Started
```bash
# Add project reference or copy NuGet once published
# Example (from a consumer project folder):
# dotnet add reference ../../CSharpBible/Libraries/ConsoleLib/ConsoleLib.csproj
```

## Basic Usage
```csharp
var app = new Application(console, extendedConsole)
{
    BackColor = ConsoleColor.DarkBlue
};
new Label { Parent = app, Text = "Hello ConsoleLib", Dimension = new(0,0,30,1)};
app.Run();
```

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
