# Task T-RepoMigrator-024 - Define migration destination provider abstractions

## Status

Draft

## Parent

- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`
- Backlog Item `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`
- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Define the destination-side provider abstractions that keep RepoMigrator.Core symmetric across source and destination extensibility.

## Scope

- Define `IMigrationDestinationProvider` and `IMigrationDestinationProviderFactory`
- Define normalized destination models such as `MigrationDestinationDefinition`, `MigrationDestinationKind`, and `MigrationDestinationCommit`
- Clarify the relationship between `IMigrationDestinationProvider` and `IVersionControlProvider`
- Define which responsibilities stay in the broader destination provider versus the repository-specific provider
- Record future fit for sequential archive emission as a non-first-slice destination type

## Detailed Work Packages

1. Provider-boundary analysis
   - list current target-side responsibilities in `MigrationService`
   - separate generic destination concerns from VCS-specific operations
   - document which responsibilities should remain repository-specific
2. Destination model draft
   - define the normalized destination definition shape
   - define commit/write metadata independent from Git/SVN naming
   - define destination-kind enumeration and extensibility rules
3. Abstraction draft
   - define interface members for initialize, write snapshot, finalize, and capability checks
   - define factory-selection rules and failure behavior
   - define how progress reporting flows through the abstraction
4. Mapping strategy
   - map repository-backed destinations onto `IVersionControlProvider`
   - describe how a future sequential archive destination would fit without redesign
   - identify orchestration impact on archive import and repository migration paths

## Deliverables

- A concrete destination-provider abstraction proposal
- A normalized destination model proposal
- A boundary note describing how `IVersionControlProvider` fits below the broader destination-provider layer
- A list of follow-up code and test tasks implied by the design

## Dependencies

- `T-RepoMigrator-017` - `Draft archive source core architecture`
- `T-RepoMigrator-023` - `Define concrete code target state for archive import`

## Acceptance Criteria

- A symmetric destination-provider abstraction is documented
- The relationship between destination providers and `IVersionControlProvider` is explicit
- The design supports repository-backed destinations now and sequential archive output later
- The orchestration impact is identified without forcing immediate implementation of archive-output targets

## Open Questions

- Should destination providers expose capabilities explicitly, or should capability checks stay implementation-specific for the first slice?
- Should destination initialization own branch-selection semantics, or should that remain in destination definitions?
