# AA98-Bl038 Linux Shell Startup Baseline

## Parent
- Feature: `../Features/AA98-F41-Linux-Workbench-Base.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
A developer can start the AA98 workbench on Linux and see actionable diagnostics when startup fails.

## Scope
- Validate the main workbench startup path on Linux.
- Remove or isolate Windows-only startup assumptions.
- Ensure dependency composition reports useful errors.
- Prepare a thin shell host path for focused validation.

## Acceptance Criteria
- AA98 has a documented Linux startup path.
- Startup blockers are categorized as platform, dependency, configuration, or missing implementation.
- A smoke validation task exists for shell startup.

## Implementation Tasks
- `AA98-T042 Inspect Linux Shell Startup Path`
- `AA98-T043 Implement Linux Shell Startup Fixes`
- `AA98-T044 Add Linux Shell Startup Validation`

## Assumptions
- Packaging is not part of this backlog item.
- The main workbench remains Avalonia-based.

## Open Questions
- Which Linux distribution is the first explicit smoke-test target?

## Next Refinement Steps
1. Execute task `AA98-T042` first to discover actual blockers.
2. Refine `AA98-T043` if discovery shows multiple independent fixes.

## Discovery Notes
- The current AA98 shell entry point is `AA98_AvlnCodeStudio.UI/Program.cs`, which starts Avalonia through the classic desktop lifetime.
- `AA98_AvlnCodeStudio.UI/App.axaml.cs` couples service-provider creation and `MainWindow` construction to `IClassicDesktopStyleApplicationLifetime`.
- A Linux blocker is not a direct Windows API call in startup, but the absence of a dedicated startup smoke-validation path and actionable diagnostics when desktop/session integration fails.
- Related Linux portability work was identified in the editor workflow: fallback directory selection uses `Environment.SpecialFolder.MyDocuments`, and current tests exercise only Windows-style paths.

## Implementation Progress
- `AA98-T043` introduced an explicit desktop initialization seam and startup diagnostics in `AA98_AvlnCodeStudio.UI/App.axaml.cs`.
- Core startup composition is now validated by targeted tests in `AA98_AvlnCodeStudio.Tests/Startup/AppStartupCompositionTests.cs`.
- `AA98-T044` completed the repeatable startup validation path by extending automated startup guardrail tests and documenting a manual Linux smoke checklist in `../Validation/AA98-Linux-Shell-Startup-Smoke-Checklist.md`.

## Task Status Snapshot
- `AA98-T042 Inspect Linux Shell Startup Path` - Completed
- `AA98-T043 Implement Linux Shell Startup Fixes` - Completed
- `AA98-T044 Add Linux Shell Startup Validation` - Completed

## Status
- Completed
