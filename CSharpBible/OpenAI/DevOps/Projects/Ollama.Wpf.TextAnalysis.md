# Project: Ollama.Wpf.TextAnalysis

## Status
In Progress

## Purpose
`Ollama.Wpf.TextAnalysis` is the WPF sample application for local text and code analysis.

## Current Responsibilities
- Host the WPF UI.
- Provide a folder-selection workflow.
- Trigger content analysis through a service abstraction.
- Display analysis progress and results.

## Architecture Notes
- Uses `CommunityToolkit.Mvvm`.
- Uses `Microsoft.Extensions.DependencyInjection` / host composition.
- Keeps UI strings and user-facing status in the ViewModel and UI layer.

## Notable Types
- `App`
- `MainWindow`
- `MainWindowViewModel`
- `ContentAnalysisService`

## Related Planning Items
- [Epic: WPF App for Advanced Text Analysis and Processing](../Epics/Epic-WpfTextAnalysis.md)
- [Feature: Initial WPF Application and Router Infrastructure](../Features/Feat-01-WpfAppAndRouter.md)
- [Feature: Advanced Text and Document Analysis Capabilities](../Features/Feat-02-AdvTextAndDocAnalysis.md)

## Platform Context
- Uses the shared Ollama platform for local model access.
- Should remain separate from picture analysis scenarios.