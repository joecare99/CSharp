# AA98-T044 Add Linux Shell Startup Validation

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl038-Linux-Shell-Startup-Baseline.md`

## Goal
Add validation coverage for the Linux shell startup baseline.

## Scope
- Add smoke or architecture validation for startup composition where practical.
- Document any manual Linux smoke check that cannot yet be automated.
- Keep validation focused on startup readiness.

## Execution Notes
1. Prefer automated tests for startup services and composition boundaries.
2. Add a manual smoke checklist only where UI launch automation is not yet available.
3. Keep validation reusable for future CI.

## Acceptance Criteria
- A repeatable validation path exists for shell startup.
- The validation is linked from the backlog item or task notes.

## Validation
- Run the new targeted tests or smoke validation.
- Record pass/fail status and blockers.

## Implementation Notes
- Automated startup validation extends `AA98_AvlnCodeStudio.Tests/Startup/AppStartupCompositionTests.cs` with repeatable guardrail coverage for startup service registration and public startup seam argument checks.
- Manual Linux full-UI startup validation is documented in `../Validation/AA98-Linux-Shell-Startup-Smoke-Checklist.md` until UI launch automation is stable enough for CI.

## Validation Result
- Targeted startup composition tests were added for service registration, missing top-level provider rejection, desktop initialization diagnostics, and missing service provider rejection.
- Manual smoke validation path is documented and ready for Linux execution recording.

## Status
- Completed
