# Task T-RepoMigrator-029 - Implement zip archive driver

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`

## Goal

Implement the first archive driver for `.zip` inspection and extraction.

## Scope

- Implement `.zip` driver selection and matching
- Read entry metadata needed for ordering and diagnostics
- Extract a complete snapshot into a working directory
- Report unsupported or invalid zip content clearly
- Keep later `.7z`, `.tar`, and `.tar.gz` support out of this slice

## Detailed Work Packages

1. Implement `ZipArchiveDriver`
2. Implement inspection output for entry timestamps and candidate ordering metadata
3. Implement full extraction to a temp directory
4. Add tests for driver selection, inspection results, extraction, and invalid-archive handling

## Deliverables

- `.zip` archive driver implementation
- Inspection and extraction behavior with tests
- First-slice driver compatibility note in code comments or documentation where useful

## Dependencies

- `T-RepoMigrator-019` - `Define archive source and driver service contracts`
- `T-RepoMigrator-026` - `Implement source and destination provider abstractions`

## Acceptance Criteria

- `.zip` archives can be selected, inspected, and extracted through the driver abstraction
- Tests cover valid and invalid zip scenarios
- The implementation is isolated enough to allow later format drivers without redesign
