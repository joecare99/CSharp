# Backlog Item: Implement SummaryTool within Ollama.Tools

## Feature Link
[Feature: Advanced Text and Document Analysis Capabilities](../Features/Feat-02-AdvTextAndDocAnalysis.md)

## Status
Draft

## Description
As a user, I want the system to be able to summarize large bodies of plain text or PDF text, so I can grasp the essence quickly without routing everything manually to an LLM prompt myself.

## Acceptance Criteria
- A new `SummaryTool` class implements `IContentAnalysisTool`.
- `ContentAnalysisRouter` can handle a new `Summarize` mode.
- The `Ollama.Wpf.TextAnalysis` interface offers a specific button or toggle "Create Summary".

## Tasks
- Add new `ContentAnalysisMode.Summarize` to the base enum.
- Implement `SummaryTool` stub and schema in `Ollama.Tools`.
- Add Dependency Injection registration in `App.xaml.cs`.
- Update `ContentAnalysisRouter.Detect()` or `Route()` to inject `SummaryTool` when chosen.
- Add UI Element in `MainWindow.xaml` and Command in `MainWindowViewModel` to trigger Summarization.
- Add dedicated Unit Test Task: Implement coverage for `SummaryTool` bounding box validation.