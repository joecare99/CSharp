# ConsoleLib

Rich, testable console UI toolkit for .NET terminals. The core is host-neutral;
`ConsoleLib.ExtCon` provides the Windows/ExtendedConsole backend and
`ConsoleLib.Posix` provides ANSI/VT output and input for Linux, macOS, SSH,
tmux, and redirected streams.

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
- List rendering (scrolling, selection � extensible).
- Basic 2?way binding support for `TextBox` (model <-> text) and (extension ready) selection.
- Drawing API via `TextCanvas` (rect fill, bordered boxes, character output).
- Separation of concerns through `IConsole` and `IWidgetSet` abstractions.
- Backend-specific rendering and host-loop implementations can live in separate projects such as `ConsoleLib.ExtCon` or other host-specific adapters.
- CXAML loading, deterministic source generation, and the Avalonia designer are
  available through the CXAML projects and use the same runtime loader.

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
Reference the core project plus a concrete backend such as `ConsoleLib.ExtCon`
or `ConsoleLib.Posix`, then create the application with an `IWidgetSet`
implementation. For a declarative view, embed a `.cxaml` file and load it with
`CxamlLoader`; the parallel examples under `Calc`, `ConsoleApps`, `Games`, and
`OpenAI` demonstrate this without replacing the imperative applications.

## POSIX host safety

Use `PosixTerminalTransport` with redirected streams in CI and a TTY in
interactive sessions. Raw mode is scoped to the host lifetime and restored on
normal completion, cancellation, and transport failure. Terminals without SGR
mouse support remain keyboard-operable; clipboard integration is opt-in.

## Validation

The split test projects are `ConsoleLib.CoreTests`, `ConsoleLib.ExtConTests`,
`ConsoleLib.PosixTests`, `ConsoleLib.Cxaml.DesignerTests`, and
`ConsoleLib.Cxaml.ExamplesTests`. Run them individually when diagnosing a
backend and collect line coverage with:

```text
dotnet test <test-project.csproj> --collect:"XPlat Code Coverage"
```

## Application services

The core also provides independent, DI-friendly application services:
`ApplicationMessageQueue` and `ApplicationDispatcher` provide an instance-scoped
FIFO dispatch queue, while `ApplicationScheduler` schedules work onto that
dispatcher through the `IScheduler` and `IClock` abstractions. These services
are additive and do not replace the legacy `Control.MessageQueue`.

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
- The `ConsoleLib` core itself is intended to stay host- and OS-neutral; platform-specific event or rendering integrations belong in separate backend projects.

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
(Insert license notice here � e.g. MIT / proprietary.)
