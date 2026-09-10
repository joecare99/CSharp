# B-Diagnostics-Jump-To-Code

## Parent
F-Shared-CodeStudio-Components

## Status
Done

## Request
Extend the existing app-wide diagnostic UI and debug output so diagnostics carrying SourcePath, LineNumber, and ColumnNumber can activate the neutral code-location navigator. Preserve existing debug output behavior.

## Acceptance criteria
- Diagnostic UI activation delegates only through the navigator capability.
- Diagnostics without a usable source location have a defined unavailable state.
- Navigator failures are shown without losing the diagnostic list.
- DebugDiagnosticConsumer keeps emitting diagnostics.

## Tasks
T-020 Done, T-021 Done, T-022 Done

## Gates
G-Architecture after T-020; G-Integration after T-022.