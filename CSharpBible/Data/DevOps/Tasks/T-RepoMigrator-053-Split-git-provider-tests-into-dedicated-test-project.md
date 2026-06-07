# Task T-RepoMigrator-053 - Split Git provider tests into dedicated test project

## Status

Done

## Parent

- Task `T-RepoMigrator-011` - `Close priority test coverage gaps`
- Task `T-RepoMigrator-052` - `Split SVN tests into dedicated test projects`

## Goal

Move Git-provider-related coverage out of the broad `RepoMigrator.Tests` project into a dedicated MSTest project with clear ownership for `RepoMigrator.Providers.Git`.

## Scope

- Create a dedicated MSTest project for `RepoMigrator.Providers.Git`
- Move Git-provider-related tests from `RepoMigrator.Tests` into the scoped project
- Keep solution-file handling out of scope for this slice

## Deliverables

- New `RepoMigrator.Providers.Git.Tests` project
- Moved Git-provider-related tests with preserved behavior coverage
- Validation evidence for direct project-based test execution and workspace build

## Implementation Notes

- Created `RepoMigrator.Providers.Git.Tests` as a dedicated MSTest project with localized MSTest assembly settings and minimal references to `RepoMigrator.Core` and `RepoMigrator.Providers.Git`
- Added `NSubstitute` to the new project for the Git remote seam and destination-provider coverage
- Moved `GitProviderTests`, `GitProviderHelperTests`, `GitProviderRemoteTests`, `GitReferenceNameResolverTests`, `GitStructuredChangeSetSinkTests`, `GitTargetRefOperationsTests`, and `VersionControlMigrationDestinationProviderTests` into `RepoMigrator.Providers.Git.Tests`
- Removed the moved Git-provider-related test files from `RepoMigrator.Tests` without changing the solution file

## Validation Evidence

- `dotnet test RepoMigrator\\RepoMigrator.Providers.Git.Tests\\RepoMigrator.Providers.Git.Tests.csproj` -> passed, total: 54; failed: 0; successful: 54; skipped: 0
- Workspace build -> passed

## Detailed Work Packages

1. Inspect Git provider test dependencies and split ownership by provider scope
2. Create a dedicated MSTest project for `RepoMigrator.Providers.Git`
3. Move the Git provider tests into `RepoMigrator.Providers.Git.Tests`
4. Validate the new test project directly without changing the solution file

## Acceptance Criteria

- Git-provider-related tests no longer live in `RepoMigrator.Tests`
- A dedicated MSTest project exists for `RepoMigrator.Providers.Git`
- The new project can be built and run directly by project path
- The current solution file remains untouched

## Risks

- Some tests may rely on helpers that currently exist only in the aggregate test project and may need to be localized
- Some Git-adjacent tests may actually belong to the GitBranchSplitter tool and should remain outside this slice

## Open Questions

- Should `VersionControlMigrationDestinationProviderTests` remain with the Git provider as long as the implementation lives in `RepoMigrator.Providers.Git`, or should it later move to a more generic destination-provider test project?
- Should GitBranchSplitter tests be split into their own dedicated tool test project in the next slice?
