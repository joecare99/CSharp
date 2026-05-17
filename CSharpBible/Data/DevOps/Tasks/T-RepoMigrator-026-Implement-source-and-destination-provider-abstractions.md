# Task T-RepoMigrator-026 - Implement source and destination provider abstractions

## Status

Completed

## Parent

- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`

## Goal

Implement the broader provider abstractions that keep RepoMigrator.Core symmetric across migration sources and destinations.

## Scope

- Add `IMigrationSourceProvider` and `IMigrationSourceProviderFactory`
- Add `IMigrationDestinationProvider` and `IMigrationDestinationProviderFactory`
- Keep `IVersionControlProvider` focused on genuine repository operations
- Define minimal first-slice members needed by archive import orchestration
- Keep dependency-injection usage straightforward for later registration work

## Detailed Work Packages

1. Add abstraction files under `RepoMigrator.Core/Abstractions`
2. Add factory contracts and basic selection expectations
3. Add XML documentation for the abstraction boundaries
4. Add tests covering unsupported-provider resolution expectations if factory implementations are introduced in this slice

## Deliverables

- New source-provider and destination-provider abstraction files
- Clear separation between the broader provider layer and `IVersionControlProvider`
- Initial tests if selection behavior is implemented now

## Dependencies

- `T-RepoMigrator-024` - `Define migration destination provider abstractions`
- `T-RepoMigrator-025` - `Implement normalized migration source and destination models`

## Acceptance Criteria

- Source and destination provider abstractions exist in RepoMigrator.Core
- The abstractions are symmetric enough for future archive-output destinations
- `IVersionControlProvider` remains repository-specific

## Implementation Summary

- Added normalized source-planning support models:
  - `MigrationSourcePlan`
  - `MigrationSourcePlanItem`
- Added broader provider abstraction files in `RepoMigrator.Core/Abstractions`:
  - `IMigrationSourceProvider`
  - `IMigrationSourceProviderFactory`
  - `IMigrationDestinationProvider`
  - `IMigrationDestinationProviderFactory`
- Kept `IVersionControlProvider` unchanged and repository-specific while introducing the higher-level source and destination provider seams.
- Added `MigrationProviderAbstractionsTests` to cover default model initialization and basic interface assignability through test doubles.

## Architecture Correction Note

- The provider-agnostic abstractions added in this task remain valid Core ownership.
- Any future provider-specific contracts beneath these abstractions, including archive-specific and compression-specific contracts, should live in dedicated provider projects rather than in `RepoMigrator.Core`.

## Validation

- `run_build` completed successfully.
- Targeted tests passed:
  - `MigrationSourcePlan_Defaults_AreInitialized`
  - `MigrationSourcePlanItem_Defaults_AreInitialized`
  - `MigrationProviderInterfaces_AreAssignable_FromTestDoubles`
