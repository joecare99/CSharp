# Task T-WorkbenchBuilder-007 - Specify V1.2 compilation and emit contracts

## Status

In Progress

## Parent

- Backlog Item `BI-WorkbenchBuilder-004` - `Plan V1.2 compilation, emit, and debug symbols`

## Goal

Define the first implementation-ready contracts that transform V1.1 inspection data into V1.2 compilation and emit behavior.

## Scope

- Define the first compilation service boundaries
- Specify supported initial emit scenarios for SDK-style class libraries and console projects
- Clarify executable-versus-library output rules
- Classify test-project shapes explicitly and specify their non-emit behavior in the first slice
- Describe expected primary artifacts, Portable PDB defaults, and success conditions
- Specify how failure-oriented diagnostics expose file, line, and column information for users and IDE callers

## Acceptance Criteria

- The V1.2 contract is concrete enough to start coding
- Initial supported scenarios are explicit for class-library and console-project baselines
- Emit rules follow the earlier user intent for executable output where appropriate
- Test-project non-emit behavior is explicit in the contract
- Portable PDB output expectations are explicit enough for implementation and tests
- Failure results preserve location-rich diagnostics in a text format suitable for direct IDE navigation

## Dependencies

- `BuildProjectInfo`
- `ProjectInspectionResult`
- Future Roslyn compilation services

## Deliverables

- A concise implementation-facing contract for compilation requests, emit decisions, artifacts, and results
- A documented first-slice rule set for library, console, and test-project classifications
- A clear statement that source-generator execution is planned separately from the first emit baseline
- A documented text-diagnostic shape that points users and IDEs to the faulty file location

## Notes

The current first-slice expectation is intentionally narrow: emit should succeed for supported SDK-style class libraries and non-test console projects. Test projects should continue to be recognized through existing classification logic, but the V1.2 contract should treat them as non-emit until a later dedicated slice expands the supported matrix.

The next refinement also needs to protect failure-oriented NIO cases explicitly. Broken source samples should demonstrate that compiler diagnostics keep file, line, and column information intact and that plain-text output follows an IDE-friendly location format instead of reducing failures to generic messages.
