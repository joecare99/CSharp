# AA98-T043 Implement Linux Shell Startup Fixes

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl038-Linux-Shell-Startup-Baseline.md`

## Goal
Implement the smallest set of fixes required for AA98 shell startup on Linux.

## Scope
- Fix blockers discovered by `AA98-T042`.
- Prefer platform-neutral abstractions over Linux-specific shortcuts.
- Preserve Windows compatibility unless a documented reason exists.

## Execution Notes
1. Apply fixes in startup/composition/path handling areas only.
2. Avoid packaging or distribution work in this task.
3. Keep diagnostics actionable for later self-hosting work.

## Implementation Notes
- Extracted the AA98 desktop startup path into explicit `App.InitializeDesktop`, `App.CreateServiceProvider`, and `App.CreateMainWindow` methods so startup composition can be exercised directly.
- Added actionable `InvalidOperationException` diagnostics for desktop initialization and main-window creation failures.
- Enabled DI validation during service-provider creation via `ServiceProviderOptions` with `ValidateOnBuild` and `ValidateScopes`.
- Added startup-composition tests that verify core service registration, null-argument guarding, and wrapped startup diagnostics under headless test conditions.

## Acceptance Criteria
- The shell startup path no longer depends on avoidable Windows-only assumptions.
- Startup failures produce useful diagnostic information.

## Validation
- Run targeted build/tests for changed shell and composition projects.
- Record any unrelated solution-wide failures separately.
- `AA98_AvlnCodeStudio.Tests` executed successfully with 102/102 tests passing.
- Solution-wide restore still reports unrelated iOS/Android restore failures outside this task scope.

## Status
- Completed
