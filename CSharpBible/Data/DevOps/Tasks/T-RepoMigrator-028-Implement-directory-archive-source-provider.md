# Task T-RepoMigrator-028 - Implement directory archive source provider

## Status

Completed

## Parent

- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`

## Goal

Implement the first archive-backed migration source provider for local directory inputs.

## Scope

- Implement normalized detection and handling for local directory archive sources
- Add first-slice discovery logic for local files with configured extensions
- Support absolute and relative paths in the intended workspace context
- Emit deterministic discovered archive items for downstream inspection and planning
- Defer HTTP index handling to a later slice

## Detailed Work Packages

1. Implement `DirectoryArchiveSnapshotSourceProvider` or equivalent first-slice source-provider class
2. Implement discovered-item creation for supported local archive files
3. Define validation behavior for missing paths, empty directories, and unsupported file sets
4. Add MSTest coverage for absolute paths, relative paths, filtering, and deterministic ordering of discovery output

## Deliverables

- First-slice local-directory archive source provider implementation
- Deterministic discovered archive item output
- Unit tests covering discovery behavior and validation paths

## Dependencies

- `T-RepoMigrator-019` - `Define archive source and driver service contracts`
- `T-RepoMigrator-026` - `Implement source and destination provider abstractions`

## Acceptance Criteria

- Local archive directories can be discovered through the broader source-provider model
- Relative and absolute path scenarios are covered by tests
- Discovery output is deterministic and ready for planning

## Implementation Summary

- Added `RepoMigrator.Core/Services/DirectoryArchiveSnapshotSourceProvider.cs` as the first archive-backed `IMigrationSourceProvider` implementation for local directory inputs.
- Implemented support for:
  - absolute and relative directory paths,
  - deterministic archive discovery ordering,
  - extension-based filtering,
  - optional recursive scanning,
  - validation for missing directories and directories without matching archive files.
- Used normalized `MigrationSourcePlan` and `MigrationSourcePlanItem` output so downstream planning can consume the provider without archive-specific orchestration assumptions.
- Added `RepoMigrator.Tests/DirectoryArchiveSnapshotSourceProviderTests.cs` covering absolute paths, relative paths, recursive filtering behavior, missing-directory validation, empty-match validation, and `CanHandle` behavior.

## Architecture Correction Note

- A later architecture clarification established that provider-specific implementations should live in dedicated provider projects rather than in `RepoMigrator.Core`.
- `DirectoryArchiveSnapshotSourceProvider` should therefore be relocated into an archive-provider project as part of `T-RepoMigrator-035 - Relocate provider-specific archive and compression files out of core`.
- The behavior implemented in this task remains useful, but the owning project boundary must be corrected.

## Validation

- `run_build` completed successfully.
- Targeted test execution passed for `RepoMigrator.Tests.DirectoryArchiveSnapshotSourceProviderTests`.

## Follow-up Maintenance Note

- A regression was identified while validating the archive smoke test against `C:\Projekte\Cpp\xpdf`: the default directory discovery logic recognized `.tar.gz` files but skipped `.tgz` files, even though both represent gzip-compressed tar archives in real snapshot collections.
- The source-provider default extension list was extended to include `.tgz`.
- Additional MSTest coverage now verifies that default archive discovery includes both `.tar.gz` and `.tgz` inputs.
- Validation after the fix:
  - targeted tests for `RepoMigrator.Tests.DirectoryArchiveSnapshotSourceProviderTests` and `RepoMigrator.Tests.TarGzArchiveDriverTests` passed,
  - the `RepoMigrator.Tools.ArchiveSmokeTest` run against `C:\Projekte\Cpp\xpdf` completed successfully and listed the previously skipped `.tgz` snapshots.
