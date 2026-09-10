# T-020 Map Diagnostics to Code Locations

## Parent
B-Diagnostics-Jump-To-Code | Previous: T-007 | Next: T-021

## Status
Done

## Request
Create a diagnostic-to-CodeLocation mapping adapter based on the AppKomponentBaseLib diagnostic contract. It handles absent/invalid SourcePath, LineNumber, and ColumnNumber without requiring UI or editor types.

## Implementation
- Extended `AppKomponentBaseLib.Diagnostics.Diagnostic` with the optional
  one-based `ColumnNumber` field.
- Added `Diagnostics.Navigation` as a UI-neutral adapter library referencing
  `AppKomponentBaseLib` and `Code.Navigation`.
- Added explicit mapping statuses for missing source paths, invalid paths, and
  invalid coordinates.
- Relative paths are rejected; absolute paths are normalized by `CodeLocation`.
- The mapper never changes the diagnostic code, message, severity, or source
  payload.

## Planned tests
- [x] Valid source path, line, and column map to equivalent CodeLocation.
- [x] File-only diagnostic maps to a path-only CodeLocation.
- [x] Missing, relative, malformed, zero, and negative location fields return defined unavailable results.
- [x] Mapping does not alter severity, code, message, or diagnostic payload.
- [x] AppKomponentBaseLib contract tests verify optional ColumnNumber behavior.

## Validation evidence
- `AppKomponentBaseLib.Tests`: 2/2 passed on net8.0.
- `Diagnostics.Navigation.Tests`: 9/9 passed on net8.0, net9.0, and net10.0.
- `Diagnostics.Navigation` builds successfully on net8.0, net9.0, and net10.0.
- No Avalonia, AvaloniaEdit, or editor-host dependency exists in the adapter.
- G-Architecture approved for the shared base-contract extension and neutral
  diagnostics mapping boundary.

## Handoff
- Current task: Done.
- Next task: T-021 Implement Diagnostic Activation.

## Gate and completion
G-Architecture passed. MSTest/build evidence, documentation, SVN revision,
next-task activation, backlog update, and feature handoff are required and
recorded.
