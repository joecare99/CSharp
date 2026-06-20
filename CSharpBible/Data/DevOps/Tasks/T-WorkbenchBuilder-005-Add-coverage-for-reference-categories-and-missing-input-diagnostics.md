# Task T-WorkbenchBuilder-005 - Add coverage for reference categories and missing-input diagnostics

## Status

In Progress

## Parent

- Backlog Item `BI-WorkbenchBuilder-003` - `Expand builder regression coverage`

## Goal

Increase test confidence for reference classification and diagnostic emission in edge or failure-oriented inspection scenarios.

## Scope

- Add test scenarios that exercise missing compile items or missing project references
- Validate that diagnostic codes and severities remain observable
- Add assertions around the visible behavior of the multi-target sample where diagnostics or degraded per-target behavior must remain explicit
- Cover additional reference categories where the current test set is too shallow
- Add failure-oriented NIO sample projects where users should be guided directly to the faulty source location
- Protect file, line, and column propagation so IDE-oriented consumers can navigate to diagnostics reliably

## Acceptance Criteria

- New tests protect important diagnostic and classification behavior
- The most visible V1.1 warning paths are covered
- The test intent remains readable through focused sample cases
- At least one intentionally broken sample validates location-rich diagnostics through file, line, and column assertions

## Dependencies

- `ProjectInspectionServiceTests`
- `ReferenceResolverTests`
- Current test-data project set or targeted additions to it

## Notes

If the new multi-target sample exposes current V1.1 limitations or degraded reference-resolution behavior, those outcomes should be protected through focused assertions here rather than remaining implicit in the sample alone.

The first multi-target hardening increment is now in place: loader, inspection, and reference-resolution tests protect the visible default-first-target behavior and explicit requested-target behavior for the new `MultiTargetLibrary` sample. Additional degraded-diagnostic cases remain open for later sample additions.

For the next hardening increment, builder tests should no longer focus only on successful IO cases. They should also include NIO or intentionally broken samples whose diagnostics must point the caller directly to the failing file and source position.
