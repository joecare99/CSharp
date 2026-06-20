# Task T-WorkbenchBuilder-008 - Plan source-generator and Portable PDB support slices

## Status

Draft

## Parent

- Backlog Item `BI-WorkbenchBuilder-004` - `Plan V1.2 compilation, emit, and debug symbols`

## Goal

Make source-generator compatibility and debug-symbol generation explicit planning tracks before V1.2 implementation grows too far.

## Scope

- Identify the minimum source-generator expectations for early builder scenarios
- Define the default Portable PDB stance for the first emit slice
- Clarify which Portable PDB behavior belongs directly to the first emit implementation versus later refinement
- Separate source-generator planning from the first class-library and console emit baseline
- Separate mandatory first-slice work from later refinement work

## Acceptance Criteria

- Source-generator and Portable PDB needs are visible and not implicit assumptions
- The first emit slice has clear boundaries around what is and is not supported
- The default Portable PDB expectation for supported emit scenarios is explicit
- Source-generator planning is explicitly staged after the first emit baseline instead of being blended into it prematurely
- Follow-up implementation can proceed incrementally rather than reopening the full design

## Dependencies

- Future V1.2 compilation planning
- User requirements around source generators and debugging

## Notes

For the current V1.2 direction, Portable PDB should be treated as the default symbol format for supported emit scenarios. Source generators and analyzer-sensitive builds remain important, but they should be planned as explicit follow-up slices after the first implementation baseline for SDK-style class-library and console emit scenarios is stable.
