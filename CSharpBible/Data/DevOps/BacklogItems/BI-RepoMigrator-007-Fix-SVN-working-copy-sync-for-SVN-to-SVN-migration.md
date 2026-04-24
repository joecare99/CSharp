# Backlog Item BI-RepoMigrator-007 - Fix SVN working-copy sync for SVN-to-SVN migration

## Status

Completed

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-02` - `Git and SVN provider support`

## Goal

Stabilize SVN-to-SVN snapshot migration by preserving the administrative structure of the temporary target working copy before `svn commit` is executed.

## Description

A failing SVN-to-SVN transfer revealed that the target working copy could be damaged during snapshot synchronization. The migration flow exported source content into a temporary directory and then synchronized that content into the temporary SVN target working copy before calling `svn commit`.

The root cause was incomplete protection of administrative `.svn` content inside the synchronization routine. While the top-level `.svn` directory was intended to be preserved, nested administrative paths such as `.svn/tmp` and `.svn/pristine/...` could still be removed during cleanup of files and empty directories. That left the working copy in an invalid state and caused `svn commit` to fail with `E720003: Unable to create pristine install stream`.

The implemented fix updates the synchronization logic so every path below a `.svn` segment is excluded from cleanup and copy operations. This keeps the working copy metadata intact while still allowing regular repository content to be refreshed from the exported snapshot.

A second issue became visible after the administrative-folder fix: remote SVN source and target operations did not honor `RepositoryEndpoint.BranchOrTrunk`. As a result, probe, export, checkout, and repository URL resolution could silently operate on the repository base URL instead of the intended branch or trunk path. The provider logic was updated so remote SVN endpoints now resolve `UrlOrPath` together with `BranchOrTrunk` before those operations are executed.

A third issue affected timestamp preservation. The provider extracted the committed SVN revision by concatenating every digit from the full `svn commit` output. When changed file names or paths contained numbers, the computed revision could be wrong. The subsequent `svn:date` revprop update then targeted a non-existent or unrelated revision and failed silently. The parser was tightened so it now reads the revision only from the actual commit completion line.

A fourth issue appeared during longer SVN-to-SVN sequences. The temporary target working copy was reused across multiple commits without an explicit `svn update` after each successful commit. Because SVN working copies can remain mixed-revision after commits, later changes against directories touched in earlier revisions could fail with `E155011 out of date`. The provider now updates the temporary target working copy to `HEAD` after each successful commit so the next migration step starts from a current base state.

A fifth issue appeared on repository revisions with very large delete sets. The provider collected every missing path into a single `svn delete --force` command line. On Windows, a revision with many removed files and long Delphi-era paths could exceed the process argument length limit and fail before `svn` even started, producing a `Der Dateiname oder die Erweiterung ist zu lang` process-start error. The deletion logic now writes the missing paths into a temporary targets file and calls `svn delete --force --targets ...` from the working-copy directory.

A regression test was added to verify that nested `.svn` administrative folders survive synchronization and that obsolete business files are still removed as expected.

## Acceptance Criteria

- SVN-to-SVN snapshot migration no longer removes nested administrative paths below `.svn`
- Temporary working-copy folders such as `.svn/tmp` remain intact after synchronization
- Existing pristine metadata below `.svn/pristine` remains intact after synchronization
- Remote SVN source and target operations honor `BranchOrTrunk` when a branch or trunk path is configured
- SVN commit revision extraction remains correct even when commit output contains file or path names with digits
- Sequential SVN target commits refresh the temporary working copy to `HEAD` after each successful commit
- Large SVN delete sets no longer depend on one oversized Windows command line
- Regular repository files that are no longer present in the source snapshot are still removed from the target working copy
- Updated source snapshot files are still copied into the target working copy
- Automated regression coverage exists for the protected `.svn` synchronization behavior

## Validation

- `dotnet build RepoMigrator\RepoMigrator.Tests\RepoMigrator.Tests.csproj`
- `dotnet test RepoMigrator\RepoMigrator.Tests\RepoMigrator.Tests.csproj --filter SyncDirectory_PreservesAdministrativeSvnFolders`
- `dotnet test RepoMigrator\RepoMigrator.Tests\RepoMigrator.Tests.csproj --filter "SyncDirectory_PreservesAdministrativeSvnFolders|ResolveRemoteEndpointPath_AppendsBranchOrTrunk_WhenProvided"`
- `dotnet test RepoMigrator\RepoMigrator.Tests\RepoMigrator.Tests.csproj --filter "ExtractCommittedRevision_ReturnsRevisionFromCommitLine|SyncDirectory_PreservesAdministrativeSvnFolders|ResolveRemoteEndpointPath_AppendsBranchOrTrunk_WhenProvided"`

## Implementation Notes

- `RepoMigrator.Providers.SvnCli.SvnCliProvider.SyncDirectory` now filters all paths that contain a `.svn` segment
- `RepoMigrator.Providers.SvnCli.SvnCliProvider` now resolves remote endpoint URLs from `UrlOrPath` plus `BranchOrTrunk` for probe, export, checkout, and repository URL access
- `RepoMigrator.Providers.SvnCli.SvnCliProvider.ExtractCommittedRevision` now parses only the commit completion line instead of collecting all digits from the full output
- `RepoMigrator.Providers.SvnCli.SvnCliProvider.CommitSnapshotAsync` now runs `svn update` on the temporary target working copy after successful commits to avoid mixed-revision `out of date` errors
- `RepoMigrator.Providers.SvnCli.SvnCliProvider.CommitSnapshotAsync` now deletes missing paths through a temporary `--targets` file so revisions with many long paths remain below Windows process argument limits
- `RepoMigrator.Tests.SvnCliProviderSyncDirectoryTests` verifies preservation of `.svn/tmp` and `.svn/pristine` content
- `RepoMigrator.Tests.SvnCliProviderPathResolutionTests` verifies remote SVN path resolution with and without `BranchOrTrunk`
- `RepoMigrator.Tests.SvnCliProviderCommittedRevisionTests` verifies committed revision parsing for English and localized output that includes digits in paths
- `RepoMigrator.Tests.csproj` now references `RepoMigrator.Providers.SvnCli` so the regression test can validate the provider implementation

## Open Questions

- Should the SVN synchronization flow remain file-system-based long term, or should it later be replaced with more SVN-native working-copy update semantics for larger migrations?
