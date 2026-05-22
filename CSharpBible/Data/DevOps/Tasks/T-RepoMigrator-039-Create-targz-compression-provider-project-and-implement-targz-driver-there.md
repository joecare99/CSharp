# Task T-RepoMigrator-039 - Create TarGz compression provider project and implement tar.gz driver there

## Status

Completed

## Parent

- Task `T-RepoMigrator-035` - `Relocate provider-specific archive and compression files out of core`
- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`

## Goal

Create the dedicated TarGz compression provider project and implement the first `.tar.gz` driver in that project.

## Scope

- Create a dedicated TarGz compression provider project
- Implement `.tar.gz` archive inspection and extraction there
- Keep TarGz-specific dependencies isolated to the TarGz provider project
- Expose TarGz behavior through the archive-driver abstraction used by the archive-provider project
- Add tests for valid and invalid `.tar.gz` scenarios

## Detailed Work Packages

1. Project creation
   - create the TarGz compression provider project with the appropriate target framework
   - reference `RepoMigrator.Core` and the archive-provider project only as needed by the abstraction boundary
   - add the project to the solution and to the test project references if needed
2. Driver implementation
   - implement `TarGzArchiveDriver`
   - implement matching, inspection, and extraction behavior using built-in .NET TAR and GZip APIs
   - keep TarGz-specific helpers inside the TarGz project
3. Integration
   - wire the TarGz driver into the archive-driver selection flow
   - confirm the archive-provider project depends on abstractions, not on Core-owned TarGz details
   - keep later `.tar` and additional compression providers out of scope
4. Validation
   - add unit tests for matching, inspection, extraction, and invalid archive handling
   - build and run targeted tests

## Deliverables

- New TarGz compression provider project in the solution
- `.tar.gz` driver implementation outside `RepoMigrator.Core`
- Tests covering first-slice TarGz scenarios
- Updated references aligned to the corrected project boundaries

## Dependencies

- `T-RepoMigrator-035` - `Relocate provider-specific archive and compression files out of core`
- `T-RepoMigrator-036` - `Create archive provider project and move archive-specific models out of core`

## Acceptance Criteria

- TarGz-specific implementation files live in a dedicated TarGz compression provider project
- `.tar.gz` archives can be inspected and extracted through the intended abstraction boundary
- Tests cover valid and invalid TarGz scenarios
- Build and affected tests pass

## Validation Evidence

- Added dedicated compression provider project `RepoMigrator.Providers.Compression.TarGz`
- Implemented `TarGzArchiveDriver` in `RepoMigrator/RepoMigrator.Providers.Compression.TarGz/TarGzArchiveDriver.cs`
- Added targeted MSTest coverage in `RepoMigrator/RepoMigrator.Tests/TarGzArchiveDriverTests.cs`
- Workspace build passed after integration
