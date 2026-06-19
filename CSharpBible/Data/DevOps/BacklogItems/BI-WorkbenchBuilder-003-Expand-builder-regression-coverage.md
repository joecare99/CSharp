# Backlog Item BI-WorkbenchBuilder-003 - Expand builder regression coverage

## Status

Draft

## Parent

- Feature `F-WorkbenchBuilder-01` - `V1.1 project inspection and reference discovery`
- Feature `F-WorkbenchBuilder-02` - `Thin host and inspection output contracts`

## Goal

Keep automated validation visible and growing while the builder architecture is still in active refinement.

## Description

The builder projects already include first MSTest coverage for loader, detector, resolver, formatter, inspection service, host parser, and host orchestration. That is a strong start for a new architecture slice, but the current scenario surface is still small. This backlog item keeps test expansion explicit so later implementation work does not outpace validation.

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

1. Identify the most important uncovered V1.1 scenarios
2. Add targeted tests and sample inputs
3. Reassess gaps before V1.2 implementation begins

## Planned Implementation Tasks

- `T-WorkbenchBuilder-005` - `Add coverage for reference categories and missing-input diagnostics`
- `T-WorkbenchBuilder-006` - `Broaden sample project coverage for builder tests`

## Notes

The next concrete sample-project expansion for this backlog item should be a compact multi-target SDK-style project, for example with `TargetFrameworks=net8.0;net10.0`, so V1.1 can validate target-framework recognition and best-effort multi-target behavior explicitly.
