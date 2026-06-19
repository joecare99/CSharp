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
- Separate mandatory first-slice work from later refinement work

## Acceptance Criteria

- Source-generator and Portable PDB needs are visible and not implicit assumptions
- The first emit slice has clear boundaries around what is and is not supported
- Follow-up implementation can proceed incrementally rather than reopening the full design

## Dependencies

- Future V1.2 compilation planning
- User requirements around source generators and debugging
