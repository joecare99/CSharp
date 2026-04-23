# Backlog Item BI-RepoMigrator-008 - Avoid Git ref collisions in projected sub-branches

## Status

Completed

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-05` - `Target projection and branch transformation tools`

## Goal

Keep branch-splitting migrations compatible with Git ref rules when root-level files and projected child branches must coexist.

## Description

A projection-based SVN-to-Git migration failed once a changeset required both the root branch itself and child branches below the same namespace, for example `Develop` and `Develop/Multimedia`.

Git cannot store both a branch ref and nested refs beneath the same name because `refs/heads/Develop` would have to exist both as a ref and as a directory prefix. The original projection planner emitted exactly that conflicting structure whenever root-level files were mapped to the root branch while subdirectory content was mapped to nested child branches.

The implemented fix keeps the configured target branch as the namespace root and routes root-level files into a dedicated child branch when projected child branches also exist. The current reserved branch name is `_root`, with automatic suffixing if a collision already exists.

Examples:
- `Develop/Multimedia`
- `Develop/Tools`
- `Develop/_root` for files that live at the repository root in the exported snapshot

If only root-level files exist and no projected child branch is needed, the configured target branch is still used directly.

## Acceptance Criteria

- Projection planning no longer emits both `Root` and `Root/...` branch refs for the same snapshot set
- Root-level files are assigned to a dedicated child branch when projected child branches exist
- The dedicated root-content branch remains deterministic and collision-safe
- Existing branch projection depth behavior stays intact for level 1 and level 2 projection
- Automated tests cover the conflict-free root-content branch behavior

## Validation

- `dotnet test RepoMigrator\RepoMigrator.Tests\RepoMigrator.Tests.csproj --filter "BuildPlans_|MigrateAsync_ProjectsSnapshotsIntoConfiguredSubdirectoryBranches|SyncDirectory_PreservesAdministrativeSvnFolders|ResolveRemoteEndpointPath_AppendsBranchOrTrunk_WhenProvided|ExtractCommittedRevision_ReturnsRevisionFromCommitLine"`
- `dotnet build RepoMigrator\RepoMigrator.App.Wpf\RepoMigrator.App.Wpf.csproj`

## Implementation Notes

- `RepoMigrator.Core.SubdirectoryBranchProjectionPlanner` now assigns root-level files to `/<root>/_root` when nested projected branches also exist
- `SubdirectoryBranchProjectionPlannerTests` and `MigrationServiceTests` were updated to cover the new root-content branch behavior
- The UI guidance in `MigrationOptionsView.xaml` now explains that root files may be routed into `/_root`

## Open Questions

- Should the reserved child branch name remain `_root`, or should it become a configurable naming policy later?
