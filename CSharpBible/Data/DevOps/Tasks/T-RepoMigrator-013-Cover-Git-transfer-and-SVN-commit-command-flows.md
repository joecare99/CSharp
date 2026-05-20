# Task T-RepoMigrator-013 - Cover Git transfer and SVN commit command flows

## Status

Completed

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-04` - `Advanced migration execution modes`

## Goal

Increase coverage for high-risk provider orchestration paths by testing command composition and control flow in Git native transfer and SVN commit execution.

## Scope

- Add deterministic `GitProvider.TransferAsync` orchestration test using command seam.
- Add deterministic `SvnCliProvider.CommitSnapshotAsync` command-flow test using command seam.
- Preserve production behavior by keeping default process runners unchanged.

## Implementation Summary

- `GitProviderRemoteTests` extended with:
  - `TransferAsync_WithRemoteTarget_PushesSelectedBranchAndTag`
  - validates remote ref listing calls and branch/tag push command composition
  - validates progress reporting for branch and tag transfer events
- `SvnCliProviderCommandTests` extended with:
  - `CommitSnapshotAsync_WhenMissingFilesDetected_ExecutesDeleteCommitRevpropsAndUpdate`
  - validates command chain: add, status, delete (targets), commit, revprops, update
  - validates revision extraction from commit output drives revprop commands

## Validation

- RepoMigrator test project executed successfully after new tests.
- Coverage pass executed with GREEN status and updated percentage.

## Open Questions

- Should additional tests cover `TryRunSvnAsync` failure suppression paths explicitly to lock revprop optional behavior?
