# Task T-RepoMigrator-011 - Close priority test coverage gaps

## Status

Completed

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-04` - `Advanced migration execution modes`

## Goal

Increase coverage in high-risk migration paths and previously uncovered service contracts after introducing patch-based synchronization behavior.

## Scope

- Add unit tests for provider selection and unsupported repo type handling.
- Add App.Logic service tests for endpoint mapping and repo-type-specific history routing.
- Add Git provider path tests for safe no-op flush and external git command failure propagation.
- Add MigrationService pipeline regression test for export-failure cleanup reporting.

## Implementation Summary

- Added `ProviderFactoryTests`:
  - Git provider resolution
  - SVN provider resolution in mixed registration scenarios
  - Unsupported repo type exception path
- Added `MigrationEndpointFactoryTests`:
  - Source endpoint mapping
  - Target endpoint mapping
- Added `RecentPathHistoryServiceTests`:
  - Add/get behavior by repo type
  - Separation of histories across repo types
- Extended `GitProviderTests`:
  - `FlushAsync_WhenNoPushTargetConfigured_CompletesWithoutError`
  - `RunGitAsync_WhenCommandFails_ThrowsInvalidOperationException`
- Extended `MigrationServiceTests`:
  - `MigrateAsync_PipelinedExecution_ReportsCleanupWhenExportFails`

## Validation

- RepoMigrator test project build and test execution completed successfully.
- New tests execute with existing MSTest + NSubstitute setup.

## Open Questions

- Should additional deterministic test seams be introduced for remote Git operations (HEAD/tag/branch listing) to raise coverage further without network dependencies?
