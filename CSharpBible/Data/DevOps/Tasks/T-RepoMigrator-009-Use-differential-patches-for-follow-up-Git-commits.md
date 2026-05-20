# Task T-RepoMigrator-009 - Use differential patches for follow-up Git commits

## Status

Completed

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-04` - `Advanced migration execution modes`

## Goal

Reduce migration overhead after the initial commit by applying follow-up changes as differential file patches instead of rewriting the complete working tree snapshot each time.

## Scope

- Keep the initial commit behavior functionally unchanged.
- Replace full file-level overwrite behavior for subsequent commits with differential synchronization in the Git provider.
- Preserve commit ordering, branch handling, and `.git` administrative data.

## Assumptions

- Migration ordering remains controlled by `MigrationService`; provider synchronization must stay deterministic.
- For unchanged files, content comparison can avoid unnecessary rewrites.
- Removing files from the snapshot should remove them from the target working tree unless they are inside `.git`.

## Implementation Summary

- Updated `RepoMigrator.Providers.Git.GitProvider.SyncDirectoryContents` to:
  - build source and destination file maps by relative path,
  - delete destination files not present in the source snapshot,
  - copy only added or changed files,
  - skip unchanged files after byte-wise comparison,
  - skip `.git` administrative paths,
  - remove empty directories after deletions.
- Added helper methods for path filtering, content equality checks, and empty-directory cleanup.

## Validation

- Added tests in `RepoMigrator.Tests/GitProviderHelperTests.cs`:
  - `IsGitAdministrativePath_ReturnsExpectedValue`
  - `SyncDirectoryContents_AppliesDifferentialPatchWithoutOverwritingUnchangedFile`
- Build and tests executed in RepoMigrator test project.

## Open Questions

- Should unchanged-file detection later use hash caching to reduce read I/O for very large files?
- Should we introduce provider-level metrics for copied/deleted/skipped files to expose migration efficiency in UI logs?
