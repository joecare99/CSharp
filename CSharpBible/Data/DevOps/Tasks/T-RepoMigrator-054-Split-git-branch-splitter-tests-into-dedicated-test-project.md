# Task T-RepoMigrator-054 - Split GitBranchSplitter tests into dedicated test project

## Status

Done

## Parent

- Task `T-RepoMigrator-011` - `Close priority test coverage gaps`
- Task `T-RepoMigrator-053` - `Split Git provider tests into dedicated test project`

## Goal

Move GitBranchSplitter-related coverage out of the broad `RepoMigrator.Tests` project into a dedicated MSTest project with clear ownership for `RepoMigrator.Tools.GitBranchSplitter`.

## Scope

- Create a dedicated MSTest project for `RepoMigrator.Tools.GitBranchSplitter`
- Move GitBranchSplitter-related tests from `RepoMigrator.Tests` into the scoped project
- Split mixed generated-command-help coverage so the GitBranchSplitter assertion lives with the tool
- Keep solution-file handling out of scope for this slice

## Deliverables

- New `RepoMigrator.Tools.GitBranchSplitter.Tests` project
- Moved GitBranchSplitter-related tests with preserved behavior coverage
- Validation evidence for direct project-based test execution and workspace build

## Implementation Notes

- Created `RepoMigrator.Tools.GitBranchSplitter.Tests` as a dedicated MSTest project with localized MSTest assembly settings and a minimal reference to `RepoMigrator.Tools.GitBranchSplitter`
- Moved `GitBranchSplitOptionsTests`, `GitBranchSplitServiceTests`, `GitBranchSplitterProgramTests`, and `GitPathBranchSplitPlannerTests` into `RepoMigrator.Tools.GitBranchSplitter.Tests`
- Split `GeneratedCommandHelpTests` so the GitBranchSplitter help assertion now lives in the dedicated tool test project while the aggregate project keeps the remaining tool help coverage
- Removed the moved GitBranchSplitter-related test files from `RepoMigrator.Tests` and removed the now-unneeded `RepoMigrator.Tools.GitBranchSplitter` reference from the aggregate test project without changing the solution file

## Validation Evidence

- Visual Studio Test Explorer project run `RepoMigrator.Tools.GitBranchSplitter.Tests` -> passed, total: 21; failed: 0; successful: 21; skipped: 0
- Workspace build -> passed

## Detailed Work Packages

1. Inspect GitBranchSplitter test dependencies and split ownership by tool scope
2. Create a dedicated MSTest project for `RepoMigrator.Tools.GitBranchSplitter`
3. Move the GitBranchSplitter tests into `RepoMigrator.Tools.GitBranchSplitter.Tests`
4. Split the mixed generated-help coverage to keep tool-specific ownership clear
5. Validate the new test project directly without changing the solution file

## Acceptance Criteria

- GitBranchSplitter-related tests no longer live in `RepoMigrator.Tests`
- A dedicated MSTest project exists for `RepoMigrator.Tools.GitBranchSplitter`
- The new project can be built and run directly by project path
- The current solution file remains untouched

## Risks

- `GitBranchSplitServiceTests` rely on Git and LibGit2Sharp behavior and may need an explicit package reference if transitive compile assets are insufficient
- Generated command help coverage can become fragmented if mixed tool assertions are not split carefully

## Open Questions

- Should future command-line tool splits extract additional shared test helpers into a common test-utility project once more dedicated tool test projects exist?
