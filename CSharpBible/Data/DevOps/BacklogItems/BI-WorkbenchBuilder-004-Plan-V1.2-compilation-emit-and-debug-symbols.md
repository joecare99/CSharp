# Backlog Item BI-WorkbenchBuilder-004 - Plan V1.2 compilation, emit, and debug symbols

## Status

In Progress

## Parent

- Feature `F-WorkbenchBuilder-03` - `V1.2 compilation, emit, and debug symbols`

## Goal

Turn the user-approved V1.2 direction into concrete implementation slices for Roslyn compilation, executable output, and debug-symbol generation.

## Description

The current builder state is now advanced enough that V1.2 planning and implementation can proceed together. The workspace already has a loader, resolver, inspection service, formatter, host, test baseline, and the first working compilation slice. The next phase should harden how those outputs become Roslyn compilation inputs, confirm the supported project categories, and broaden emit validation incrementally.

Executable output for non-library, non-test projects is an explicit user requirement. Portable PDB generation is also relevant because debugging and breakpoints should remain possible. Source generators are important enough that the planning should not treat them as an afterthought, even if the first implementation splits them into a dedicated follow-up slice.

The first implementation-ready planning target is now constrained to a practical baseline: SDK-style class libraries and console apps should be supported for emit, while test projects should still be classified explicitly but should not be emitted in the initial V1.2 slice. This backlog item therefore needs to define contract boundaries, emit decisions, and artifact expectations around that narrower first milestone instead of around an open-ended compilation ambition.

## Scope

- Define the first Roslyn compilation service boundaries
- Specify executable-versus-library emit decisions for the first supported scenarios
- Clarify explicit non-emit handling for test-project classifications
- Clarify Portable PDB debug-symbol expectations and defaults
- Identify source-generator prerequisites and decision points
- Split the work into implementation-ready tasks

## Acceptance Criteria

- V1.2 planning and implementation are aligned enough to continue without reopening the full architecture question
- Supported initial emit scenarios are explicit for class-library and console-project baselines
- Test-project non-emit behavior is explicit in the planning contract
- Debug-symbol and source-generator requirements are visible in the backlog
- Follow-up tasks are small enough to execute incrementally

## Assumptions

- V1.1 inspection remains the input baseline for V1.2
- Portable PDBs are the practical symbol format target for the first slice
- Source-generator support may need a separate implementation task after the first emit baseline

## Risks

- V1.1 may still miss compilation-critical metadata
- Source-generator requirements may significantly widen the scope of the first emit slice
- Executable output may require more runtime-asset handling than the initial plan expects

## Open Questions

- Which initial project shapes are mandatory after the class-library and console baseline is locked?
- Should analyzer and source-generator handling be split into separate milestones?
- Which artifact layout is required for the first executable-output success criterion?

## Next Refinement Steps

1. Harden the minimal compilation service API and its supported baseline behavior
2. Keep the supported initial project categories and explicit test-project exclusion behavior stable while coverage expands
3. Split remaining source-generator and later emit work into executable tasks

## Planned Implementation Tasks

- `T-WorkbenchBuilder-007` - `Specify V1.2 compilation and emit contracts`
- `T-WorkbenchBuilder-008` - `Plan source-generator and Portable PDB support slices`
