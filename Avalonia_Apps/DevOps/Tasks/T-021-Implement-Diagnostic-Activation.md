# T-021 Implement Diagnostic Activation

## Parent
B-Diagnostics-Jump-To-Code | Previous: T-020 | Next: T-022

## Status
Done

## Request
Add an activation command or interaction to DiagnosticCollectionViewModel and DiagnosticListView. The UI calls the mapping adapter and the injected navigator, never AvaloniaEdit or an editor tab directly.

## Planned tests
- Activating a navigable diagnostic delegates once to the navigator.
- Diagnostics without a location disable activation or expose the documented unavailable reason.
- Navigator exception/cancellation produces a recoverable UI error state.
- Activation leaves collection order, selection, severity presentation, and DebugDiagnosticConsumer output unchanged.
- Avalonia binding test verifies command availability and activation binding.

## Gate and completion
Completed: `DiagnosticCollectionViewModel` now maps diagnostics through `DiagnosticLocationMapper` and delegates usable locations only through injected `ICodeNavigator`. `DiagnosticListView` binds the per-item activation command without a direct editor dependency. Unavailable locations and navigator exceptions/cancellation expose recoverable `ActivationErrorText` while preserving the diagnostic collection and debug consumer behavior.

Validation: Diagnostics UI build passed for net8.0, net9.0, and net10.0. `DiagnosticsConsumerTests` passed 21/21 across net8.0, net9.0, and net10.0. SVN revision: 1849.

Handoff: T-022 Validate Diagnostics Jump-to-Code is In Progress.
