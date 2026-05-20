# Task T-RepoMigrator-012 - Add command-runner test seams for provider coverage

## Status

Completed

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-04` - `Advanced migration execution modes`

## Goal

Enable deterministic unit tests for remote Git and SVN CLI command paths without requiring real external repositories or process-side effects.

## Scope

- Introduce provider-local command execution seams for `GitProvider` and `SvnCliProvider`.
- Keep default production command execution behavior unchanged.
- Add high-impact tests for remote/command paths that were previously near zero coverage.

## Implementation Summary

- `GitProvider`:
  - Added private static command delegate `s_runGitCommandAsync`.
  - Routed `RunGitAsync` through the delegate.
  - Extracted original process implementation into `RunGitProcessAsync`.
- `SvnCliProvider`:
  - Added private static command delegate `s_runSvnCommandAsync`.
  - Routed `RunSvnAsync` through the delegate.
  - Extracted original process implementation into `RunSvnProcessAsync`.
- Added tests:
  - `GitProviderRemoteTests`
    - remote selection data parsing (`--symref`, `--heads`, `--tags`)
    - remote probe result details with command output
  - `SvnCliProviderCommandTests`
    - selection data revision parsing from XML log
    - remote initialize checkout command path when wc-root probe fails
    - materialize export command composition with revision and branch path

## Validation

- RepoMigrator test suite executed successfully after seam and test additions.
- Coverage run updated to verify improved branch/path coverage in provider command flows.

## Open Questions

- Should command seam injection be promoted from private static delegates to explicit internal interfaces for stricter test isolation and parallel-test safety?
