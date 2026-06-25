# Feature F-WorkbenchBuilder-01 - V1.1 project inspection and reference discovery

## Status

Completed

## Parent

- Epic `E-WorkbenchBuilder-001` - `Roslyn-based build workbench`

## Goal

Provide a stable V1.1 inspection pipeline that can load an SDK-style project, collect compile data, classify the project shape, resolve relevant references, and expose the result through a structured model.

## Summary

This feature established the first stable inspection baseline of the `Workbench.Builder` implementation. The workspace contains a host-neutral core with `ProjectLoadRequest`, `LoadedProjectModel`, `ProjectInspectionResult`, `MsBuildProjectLoader`, `TestProjectDetector`, `ReferenceResolver`, and `ProjectInspectionService`. Sample tests exist for loading, resolving, orchestration, and visible multi-target behavior.

The feature focus shifted from initial creation to validation and hardening, and the baseline is now strong enough to support later host, UI, and compilation slices. Remaining refinements are follow-up work rather than blockers for the V1.1 milestone.

V1.1 should prefer observability over silent incompleteness. If data cannot be resolved reliably, the inspection result should remain as complete as practical while exposing the limitation through structured diagnostics instead of omitting the affected area without explanation.

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
- Projects that depend on restore state still produce a best-effort inspection result when restore data is incomplete, with explicit warnings for degraded reference resolution
- Multi-target projects are at least recognized as such, and V1.1 documents whether the current slice inspects one target framework at a time or multiple target frameworks through best-effort iteration
- Unresolved analyzer references are reported explicitly rather than being collapsed into generic missing-reference handling
- Sample project tests validate the loader, resolver, and orchestration path
- Known remaining correctness gaps are documented explicitly and do not block the first V1.2 slice

## V1.1 Contract Clarifications

- V1.1 remains inspection-first and best-effort rather than attempting a full build-grade project-system emulation.
- For projects using `TargetFramework`, the inspection pipeline should expose the inspected target framework directly.
- For projects using `TargetFrameworks`, V1.1 should recognize the multi-target shape explicitly and evolve toward best-effort per-target inspection rather than pretending that the project is inherently single-target.
- If the current implementation can only inspect one target framework reliably, that limitation must be documented as a known gap instead of being treated as the intended final behavior.
- Inspection without a prior restore is allowed, but restore-dependent reference resolution may be incomplete and should emit clear warnings.
- Project metadata and declarative item discovery should remain available even when restore-dependent resolved-reference data is incomplete.
- Unresolved analyzer references should remain visible as analyzer-specific diagnostics because analyzers matter to later compilation slices even if V1.1 does not execute them.
- V1.1 guarantees observability of degraded states before completeness of every project-system detail.

## Dependencies

- MSBuild-based evaluation via `Workbench.Builder.Core`
- Sample test projects under `Workbench.Builder.Core.Tests/TestData`
- Future host and formatter consumers in `Workbench.Builder.Host`

## Risks

- CLI-based reference resolution may differ across SDK versions or environments
- Multi-targeting behavior may need additional explicit handling
- Analyzer and package reference classification may require later refinement
- Best-effort inspection across multiple target frameworks may require result-shape extensions or additional diagnostics to avoid ambiguous output
- Inspection diagnostics may need clearer severity rules once more scenarios are covered

## Open Questions

- What exact result shape should represent best-effort inspection across multiple target frameworks without destabilizing current consumers?
- Which warning codes should be reserved specifically for restore-dependent degradation and analyzer-resolution degradation?
- Which minimum additional sample-project set is required before the V1.1 contract can be treated as hardened?

## Next Refinement Steps

1. Preserve the V1.1 inspection contract while V1.2 broadens compilation behavior
2. Expand coverage only where later waves expose new inspection regressions or ambiguity
3. Carry forward any remaining analyzer- or restore-specific refinements as follow-up work rather than reopening the V1.1 baseline
