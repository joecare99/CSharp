# Task T-RepoMigrator-032 - Implement repository-backed destination provider and Git ref operations

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`

## Goal

Implement the first repository-backed migration destination provider and the target-side Git ref operations needed for archive import resume behavior.

## Scope

- Implement a destination provider that bridges normalized destination definitions to repository-backed target behavior
- Extend Git target behavior with commit-id, tag, and branch operations needed for resume-safe execution
- Keep repository-specific operations beneath the broader destination-provider abstraction
- Avoid implementing sequential archive output in this slice
- Preserve existing repository migration behavior as far as possible
- Keep repository-backed destination implementations in destination- or provider-specific projects rather than in `RepoMigrator.Core`

## Detailed Work Packages

1. Implement `VersionControlMigrationDestinationProvider` or equivalent bridge
2. Extend repository-target operations needed for tag and branch idempotency checks
3. Add tests covering destination-provider behavior and Git ref helper behavior
4. Validate that existing repository migration tests remain compatible or are adjusted minimally

## Deliverables

- Repository-backed destination provider implementation
- Git ref operations needed for archive-import resume
- Tests covering destination writes and ref existence behavior

## Dependencies

- `T-RepoMigrator-024` - `Define migration destination provider abstractions`
- `T-RepoMigrator-026` - `Implement source and destination provider abstractions`

## Acceptance Criteria

- The first destination provider can write snapshots to repository-backed targets through the broader destination abstraction
- Git-specific ref operations required for resume are available and tested
- Existing repository behavior is preserved or intentionally adapted with tests
- The implementation-project ownership is explicit and consistent with the corrected provider boundaries
