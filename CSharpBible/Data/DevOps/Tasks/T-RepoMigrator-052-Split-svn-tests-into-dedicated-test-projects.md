# Task T-RepoMigrator-052 - Split SVN tests into dedicated test projects

## Status

Done

## Parent

- Task `T-RepoMigrator-011` - `Close priority test coverage gaps`
- Task `T-RepoMigrator-051` - `Split convert_patches tests into dedicated test project`

## Goal

Move SVN-related coverage out of the broad `RepoMigrator.Tests` project into dedicated MSTest projects with clear ownership for `RepoMigrator.Providers.SvnCli` and `RepoMigrator.Providers.Svn`.

## Scope

- Create a dedicated MSTest project for `RepoMigrator.Providers.SvnCli`
- Create a dedicated MSTest project for `RepoMigrator.Providers.Svn`
- Move SVN-related tests from `RepoMigrator.Tests` into the matching scoped projects
- Keep solution-file handling out of scope for this slice

## Deliverables

- New `RepoMigrator.Providers.SvnCli.Tests` project
- New `RepoMigrator.Providers.Svn.Tests` project
- Moved SVN-related tests with preserved behavior coverage
- Validation evidence for direct project-based test execution and workspace build

## Implementation Notes

- Created `RepoMigrator.Providers.SvnCli.Tests` as a dedicated MSTest project with localized MSTest assembly settings and minimal references to `RepoMigrator.Core` and `RepoMigrator.Providers.SvnCli`
- Created `RepoMigrator.Providers.Svn.Tests` as a dedicated MSTest project with localized MSTest assembly settings, `NSubstitute`, and minimal references to `RepoMigrator.Core`, `RepoMigrator.Providers.Svn`, and `RepoMigrator.Providers.SvnCli`
- Moved `SvnCliProviderCommandTests`, `SvnCliProviderCommittedRevisionTests`, `SvnCliProviderHelperTests`, `SvnCliProviderPathResolutionTests`, `SvnCliProviderSyncDirectoryTests`, and `SvnRevisionRangeResolverTests` into `RepoMigrator.Providers.SvnCli.Tests`
- Moved `SvnStructuredChangeSetSinkTests` into `RepoMigrator.Providers.Svn.Tests`
- Removed the moved SVN-related test files from `RepoMigrator.Tests` without changing the solution file

## Validation Evidence

- `dotnet test RepoMigrator\\RepoMigrator.Providers.SvnCli.Tests\\RepoMigrator.Providers.SvnCli.Tests.csproj` -> passed, total: 39; failed: 0; successful: 39; skipped: 0
- `dotnet test RepoMigrator\\RepoMigrator.Providers.Svn.Tests\\RepoMigrator.Providers.Svn.Tests.csproj` -> passed, total: 5; failed: 0; successful: 5; skipped: 0
- Workspace build -> passed

## Detailed Work Packages

1. Inspect SVN test dependencies and split ownership by provider scope
2. Create dedicated MSTest projects for `SvnCli` and `Svn`
3. Move the `SvnCli` tests into `RepoMigrator.Providers.SvnCli.Tests`
4. Move the structured SVN sink coverage into `RepoMigrator.Providers.Svn.Tests`
5. Validate both new test projects directly without changing the solution file

## Acceptance Criteria

- SVN-related tests no longer live in `RepoMigrator.Tests`
- Separate dedicated MSTest projects exist for `RepoMigrator.Providers.SvnCli` and `RepoMigrator.Providers.Svn`
- The new projects can be built and run directly by project path
- The current solution file remains untouched

## Risks

- Some tests may rely on helpers that currently exist only in the aggregate test project and may need to be localized
- Future provider-level test splits may reveal additional shared test utilities that should be extracted deliberately

## Open Questions

- Should `SvnRevisionRangeResolverTests` remain with `SvnCli` because of current ownership, or should that helper later move to a more shared project and test project?
- Should later aggregation happen via a dedicated collector test project or only at CI orchestration level?
