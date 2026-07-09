# Backlog Item BI-WorkbenchBuilder-003 - Expand builder regression coverage

## Status

Completed

## Parent

- Feature `F-WorkbenchBuilder-01` - `V1.1 project inspection and reference discovery`
- Feature `F-WorkbenchBuilder-02` - `Thin host and inspection output contracts`

## Goal

Keep automated validation visible and growing while the builder architecture is still in active refinement.

## Description

The builder projects include MSTest coverage for loader, detector, resolver, formatter, inspection service, host parser, host orchestration, visible multi-target behavior, and the first compilation and emit slices. This backlog item kept test expansion explicit while the architecture moved from inspection-only planning into an implemented baseline.

The focus is not only test quantity. The more important concern is preserving trust in the structured inspection contract, reference classification behavior, and host adapter semantics while the next iteration moves toward emit and source-generator-sensitive work.

## Scope

- Expand sample-project-based tests
- Add edge-case coverage for diagnostics and resolution behavior
- Keep formatter and host tests aligned with contract expectations
- Add an explicit multi-target sample-project path for V1.1 hardening
- Identify coverage gaps that must be closed before V1.2 emit work advances

## Acceptance Criteria

- Builder test coverage is tracked as an explicit work item rather than incidental cleanup
- The most important V1.1 behaviors have regression coverage
- Remaining high-risk gaps are visible before V1.2 starts growing quickly

## Assumptions

- MSTest remains the preferred test framework for this slice
- Sample test projects continue to be the main validation strategy for builder behavior

## Risks

- New features may outgrow the test data set too quickly
- Environment-sensitive tests may become flaky if restore or SDK assumptions are not controlled

## Open Questions

- Which scenarios remain mandatory after the first multi-target sample is added: analyzer-heavy projects, package-heavy projects, or executable-specific samples?
- Should additional test helpers be introduced to control restore and environment preconditions more explicitly?

## Next Refinement Steps

1. Preserve coverage for the established V1.1 baseline while V1.2 expands
2. Add targeted tests and sample inputs when later waves introduce new risk areas
3. Reassess gaps before broadening beyond the current supported emit baseline

## Planned Implementation Tasks

- `T-WorkbenchBuilder-005` - `Add coverage for reference categories and missing-input diagnostics`
- `T-WorkbenchBuilder-006` - `Broaden sample project coverage for builder tests`

## Notes

The compact multi-target SDK-style sample-project path is now part of the established validation baseline and should remain covered as later waves extend compilation behavior.
