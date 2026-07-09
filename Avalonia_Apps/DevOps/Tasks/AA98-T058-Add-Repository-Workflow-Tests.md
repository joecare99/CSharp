# AA98-T058 Add Repository Workflow Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl042-Local-Repository-Workflow-Baseline.md`

## Goal
Add validation for local repository contracts and inspection behavior.

## Scope
- Test repository context modeling.
- Test status parsing or adapter behavior with controlled data.
- Document manual validation for real repository scenarios.

## Execution Notes
1. Prefer fixture-based tests for deterministic results.
2. Avoid depending on the developer's current Git state in unit tests.

## Acceptance Criteria
- Repository inspection has repeatable validation.
- Manual repository smoke checks are documented if required.

## Delivered
- Extended deterministic Git adapter coverage for capability suppression and disabled change enumeration so repository workflow request flags are validated without depending on a live repository.
- Added fallback version control service tests to verify conservative repository-context preservation and optional capability suppression in the provider-neutral baseline.
- Documented a concise manual smoke-check flow for validating the Git-backed repository inspection against a real local AA98 repository when interactive verification is needed.

## Validation
- Run targeted repository tests.
- `dotnet test AA98_AvlnCodeStudio/AA98_AvlnCodeStudio.Tests/AA98_AvlnCodeStudio.Tests.csproj --no-restore --filter "(FullyQualifiedName~GitVersionControlServiceTests|FullyQualifiedName~NullVersionControlServiceTests)"`

## Manual Smoke Checks
1. Open AA98 in a working tree with a known branch and at least one local file change.
2. Resolve repository status through the Git-backed versioning registration.
3. Confirm that repository root, repository name, active reference, and detached-state mapping match the local Git state.
4. Confirm that staged, unstaged, renamed, and ignored entries are mapped as expected for representative files.

## Status
- Completed
