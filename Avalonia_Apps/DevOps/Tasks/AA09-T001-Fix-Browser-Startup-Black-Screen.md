# AA09-T001 Fix Browser Startup Black Screen

## Parent
- Backlog Item: `AA09 local browser host stabilization for dialog demo`

## Goal
Fix the browser startup path of `AA09_DialogBoxes` so the application shows a usable main view instead of a black empty screen after the loading phase.

## Scope
- Diagnose the browser startup behavior in the shared app project and browser host.
- Replace invalid browser lifetime initialization that uses a `Window` as the single-view root.
- Keep desktop dialog behavior intact while providing a browser-compatible root view.
- Validate the AA09 build and relevant tests after the fix.

## Assumptions
- The browser host itself starts correctly and reaches the shared Avalonia application.
- The black screen is caused by invalid view/lifetime composition rather than missing static web assets.
- Dialog interactions may require browser-specific compromises if they depend on owned windows.

## Open Questions
- Which dialog interactions should be available in browser mode if secondary windows are unsupported.
- Whether additional browser-only UX fallback is needed beyond the startup fix.

## Tasks
- [x] Analyze the AA09 browser startup and app initialization path.
- [x] Introduce a browser-compatible single-view root.
- [x] Adjust browser-critical dialog interaction paths if needed.
- [x] Validate targeted build and relevant AA09 tests.
- [x] Mark this task completed after validation.

## Notes
- This task documents a local workspace fix for the browser host startup path.
- Validation result: `AA09_DialogBoxes` and `AA09_DialogBoxes.Browser` build successfully, and `AA09_DialogBoxes_Tests` builds successfully.
- Relevant AA09 tests passed without failures; UI-bound tests that require unavailable Avalonia platform services remain intentionally skipped/inconclusive in the current test environment.
