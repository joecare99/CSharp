# Task T-RepoMigrator-034 - Test archive import first slice

## Status

Done

## Parent

- Backlog Item `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Goal

Close the highest-priority automated coverage gaps for the first archive-import delivery slice.

## Scope

- Add or complete tests for normalized models, provider selection, directory source discovery, zip inspection, ordering, naming, persistence, and resume
- Keep the test suite aligned with MSTest and NSubstitute conventions used in the repository
- Cover first-slice repository-backed destination behavior and Git ref resume helpers
- Defer HTTP-source and additional archive-format coverage
- Keep fixtures readable and deterministic
- Cover the corrected project boundaries so provider-specific tests reference the owning provider projects rather than Core-only placement assumptions

## Detailed Work Packages

1. Add unit tests for models, factories, ordering, and naming
2. Add source-provider and driver tests for local-directory plus `.zip`
3. Add persistence tests for `plan.json` and `state.json` roundtrip behavior
4. Add orchestration tests for resume between commit, tag, and branch steps
5. Verify the new tests build and run successfully in the existing RepoMigrator test project

## Deliverables

- First-slice automated test coverage for archive import
- Deterministic fixtures and test seams
- Validation evidence through successful build and test execution

## Dependencies

- `T-RepoMigrator-022` - `Define test strategy for archive import planning and resume`
- `T-RepoMigrator-025` - `Implement normalized migration source and destination models`
- `T-RepoMigrator-026` - `Implement source and destination provider abstractions`
- `T-RepoMigrator-027` - `Implement archive plan and state models`
- `T-RepoMigrator-028` - `Implement directory archive source provider`
- `T-RepoMigrator-029` - `Implement zip archive driver`
- `T-RepoMigrator-030` - `Implement archive ordering and naming services`
- `T-RepoMigrator-031` - `Implement archive import planner and DevOps state store`
- `T-RepoMigrator-032` - `Implement repository-backed destination provider and Git ref operations`
- `T-RepoMigrator-033` - `Implement archive migration service and resume flow`

## Acceptance Criteria

- The first archive-import slice has explicit automated coverage across its highest-risk behaviors
- Tests remain aligned with repository testing conventions
- Build and test validation is recorded once implementation lands
- Tests reflect the corrected provider-project ownership boundaries

## Validation Evidence

- First-slice archive-import coverage is present across normalized models, archive plan/state models, local directory discovery, Zip inspection/extraction, ordering, naming, DevOps persistence, and resume-aware orchestration
- Provider-specific tests reference the owning provider projects (`RepoMigrator.Providers.Archive`, `RepoMigrator.Providers.Compression.Zip`, and repository-backed Git provider behavior)
- Targeted first-slice validation pass completed for:
  - `RepoMigrator.Tests.MigrationSourceDestinationModelsTests`
  - `RepoMigrator.Tests.ArchiveImportModelsTests`
  - `RepoMigrator.Tests.DirectoryArchiveSnapshotSourceProviderTests`
  - `RepoMigrator.Tests.ZipArchiveDriverTests`
  - `RepoMigrator.Tests.ArchiveOrderingServiceTests`
  - `RepoMigrator.Tests.ArchiveRefNamingServiceTests`
  - `RepoMigrator.Tests.ArchiveImportPlannerTests`
  - `RepoMigrator.Tests.DevOpsArchiveImportStateStoreTests`
  - `RepoMigrator.Tests.ArchiveMigrationServiceTests`
- Targeted validation result: `39/39` tests passed
- Full workspace build passed after completing the first-slice archive-import implementation
