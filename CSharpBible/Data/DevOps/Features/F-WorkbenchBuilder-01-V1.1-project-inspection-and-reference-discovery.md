# Feature F-WorkbenchBuilder-01 - V1.1 project inspection and reference discovery

## Status

In Progress

## Parent

- Epic `E-WorkbenchBuilder-001` - `Roslyn-based build workbench`

## Goal

Provide a stable V1.1 inspection pipeline that can load an SDK-style project, collect compile data, classify the project shape, resolve relevant references, and expose the result through a structured model.

## Summary

This feature represents the current center of gravity of the `Workbench.Builder` implementation. The workspace already contains a host-neutral core with `ProjectLoadRequest`, `LoadedProjectModel`, `ProjectInspectionResult`, `MsBuildProjectLoader`, `TestProjectDetector`, `ReferenceResolver`, and `ProjectInspectionService`. Sample tests also exist for loading, resolving, and inspection orchestration.

The remaining refinement focus is less about starting the feature and more about validating and hardening what now exists on disk. The feature should clearly define the expected V1.1 contract boundaries, identify correctness gaps, and make sure the inspection result stays stable enough to serve later host, UI, and compilation slices.

## In Scope

- MSBuild-backed loading of SDK-style project metadata
- Collection of project properties, compile items, project references, and package references
- Detection of test-project heuristics
- Resolution of project, framework, package, metadata, and analyzer references
- Composition of a stable `ProjectInspectionResult`
- Diagnostics that expose missing compile items, project references, or resolved references
- Regression coverage for the current inspection flow

## Out of Scope

- Actual Roslyn compilation and emit
- Full source-generator execution
- Complete support for all legacy or platform-specific project styles
- Final formatting or UX concerns beyond the structured inspection contract itself

## Acceptance Criteria

- A caller can inspect an SDK-style project through one stable service entry point
- The inspection result exposes project metadata, compile items, project references, package references, resolved references, and diagnostics
- Missing files or references are surfaced as diagnostics rather than being silently ignored
- Test-project classification is represented explicitly in the inspection result
- Sample project tests validate the loader, resolver, and orchestration path
- The feature documents the remaining known correctness gaps before V1.2 begins

## Dependencies

- MSBuild-based evaluation via `Workbench.Builder.Core`
- Sample test projects under `Workbench.Builder.Core.Tests/TestData`
- Future host and formatter consumers in `Workbench.Builder.Host`

## Risks

- CLI-based reference resolution may differ across SDK versions or environments
- Multi-targeting behavior may need additional explicit handling
- Analyzer and package reference classification may require later refinement
- Inspection diagnostics may need clearer severity rules once more scenarios are covered

## Open Questions

- Which target-framework fallback behavior should be guaranteed in V1.1?
- Should restore-state expectations be made explicit in the service contract?
- How should unresolved analyzer references be reported compared to ordinary metadata references?
- Which additional sample projects are needed to cover realistic edge cases?

## Next Refinement Steps

1. Validate the current implementation against the intended V1.1 contract
2. Expand regression coverage around reference categories and diagnostics
3. Document known gaps in multi-targeting and restore-dependent behavior
4. Freeze the V1.1 inspection result shape before V1.2 emit work starts
