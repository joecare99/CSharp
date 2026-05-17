# Task T-RepoMigrator-038 - Retarget archive-provider tests and project references after relocation

## Status

Done

## Parent

- Task `T-RepoMigrator-035` - `Relocate provider-specific archive and compression files out of core`
- Backlog Item `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Goal

Retarget tests, project references, and validation paths after archive and compression responsibilities are relocated out of `RepoMigrator.Core`.

## Scope

- Update test project references to the new archive and Zip provider projects
- Move or retarget tests that currently assume archive-specific Core placement
- Update any DI or composition-root references affected by the relocation
- Re-run and document the first-slice validation set after relocation
- Identify any remaining tests that still rely on outdated project-boundary assumptions

## Detailed Work Packages

1. Test-reference updates
   - add references to the archive-provider and Zip provider projects
   - remove obsolete assumptions about archive-specific files living in Core
   - keep test ownership readable and deterministic
2. Test relocation or retargeting
   - retarget archive model tests
   - retarget directory archive source provider tests
   - retarget Zip driver tests once implemented
3. Build and validation pass
   - run targeted tests after project relocation
   - run a full relevant build
   - record validation outcomes in task notes
4. Cleanup review
   - identify remaining outdated namespaces, using directives, or comments
   - note any follow-up gaps that should become additional tasks

## Deliverables

- Updated test project references
- Retargeted archive and compression provider tests
- Validation evidence after relocation
- A short cleanup note listing any remaining follow-up gaps

## Dependencies

- `T-RepoMigrator-036` - `Create archive provider project and move archive-specific models out of core`
- `T-RepoMigrator-037` - `Create Zip compression provider project and implement zip driver there`

## Acceptance Criteria

- Tests reference the owning provider projects instead of outdated Core placement
- Targeted archive and Zip tests run successfully after relocation
- Build and validation evidence is recorded
- Remaining cleanup gaps are visible if any still exist

## Validation Evidence

- `RepoMigrator.Tests` references both `RepoMigrator.Providers.Archive` and `RepoMigrator.Providers.Compression.Zip`
- Targeted test pass completed for:
  - `RepoMigrator.Tests.MigrationSourceDestinationModelsTests`
  - `RepoMigrator.Tests.ArchiveImportModelsTests`
  - `RepoMigrator.Tests.DirectoryArchiveSnapshotSourceProviderTests`
  - `RepoMigrator.Tests.ZipArchiveDriverTests`
- Validation result: `24/24` tests passed
- Full workspace build passed after the retargeting updates

## Cleanup Review

- No remaining outdated Core-placement assumptions were identified in the retargeted archive and Zip test slice
- No additional follow-up task was required from this relocation validation pass
