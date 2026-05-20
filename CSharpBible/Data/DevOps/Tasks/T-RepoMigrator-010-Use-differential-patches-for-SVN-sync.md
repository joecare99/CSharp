# Task T-RepoMigrator-010 - Use differential patches for SVN sync

## Status

Completed

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-04` - `Advanced migration execution modes`

## Goal

Reduce file system overhead for SVN-based migration commits by applying only changed file content instead of rewriting all files for every synchronized snapshot.

## Scope

- Update SVN synchronization logic used by `SvnCliProvider`.
- Keep SVN add/delete status handling and commit flow unchanged.
- Preserve `.svn` administrative directories and files.

## Assumptions

- `SvnProvider` delegates to `SvnCliProvider`, so one sync implementation is sufficient for both providers.
- Byte-wise equality checks are acceptable for unchanged-file detection in the current implementation.

## Implementation Summary

- Reworked `SvnCliProvider.SyncDirectory` to:
  - build source and destination relative file maps,
  - delete files absent in the source snapshot,
  - copy only new or changed files,
  - skip unchanged files via byte-wise comparison,
  - keep `.svn` paths excluded from synchronization/deletion,
  - remove empty non-administrative directories.
- Added helper methods for file content equality and empty-directory cleanup.

## Validation

- Added test `SyncDirectory_AppliesDifferentialPatchWithoutOverwritingUnchangedFile` in `RepoMigrator.Tests/SvnCliProviderSyncDirectoryTests.cs`.
- Build and test validation executed for RepoMigrator projects.

## Open Questions

- Should future iterations include optional hashing/cache metrics for large-file comparisons?
- Should migration telemetry expose skipped/copied/deleted file counts in progress reporting?
