# Task T-RepoMigrator-037 - Create Zip compression provider project and implement zip driver there

## Status

Completed

## Parent

- Task `T-RepoMigrator-035` - `Relocate provider-specific archive and compression files out of core`
- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`

## Goal

Create the dedicated Zip compression provider project and implement the first `.zip` driver in that project.

## Scope

- Create a dedicated Zip compression provider project
- Implement `.zip` archive inspection and extraction there
- Keep Zip-specific dependencies isolated to the Zip provider project
- Expose Zip behavior through the archive-driver abstraction used by the archive-provider project
- Add tests for valid and invalid `.zip` scenarios

## Detailed Work Packages

1. Project creation
   - create the Zip compression provider project with the appropriate target framework
   - reference `RepoMigrator.Core` and the archive-provider project only as needed by the abstraction boundary
   - add the project to the solution and to the test project references if needed
2. Driver implementation
   - implement `ZipArchiveDriver`
   - implement matching, inspection, and extraction behavior
   - keep Zip-specific helpers inside the Zip project
3. Integration
   - wire the Zip driver into the archive-driver selection flow
   - confirm the archive-provider project depends on abstractions, not on Core-owned Zip details
   - keep later compression providers out of scope
4. Validation
   - add unit tests for matching, inspection, extraction, and invalid zip handling
   - build and run targeted tests

## Deliverables

- New Zip compression provider project in the solution
- `.zip` driver implementation outside `RepoMigrator.Core`
- Tests covering first-slice Zip scenarios
- Updated references aligned to the corrected project boundaries

## Dependencies

- `T-RepoMigrator-035` - `Relocate provider-specific archive and compression files out of core`
- `T-RepoMigrator-036` - `Create archive provider project and move archive-specific models out of core`

## Acceptance Criteria

- Zip-specific implementation files live in a dedicated Zip compression provider project
- `.zip` archives can be inspected and extracted through the intended abstraction boundary
- Tests cover valid and invalid Zip scenarios
- Build and affected tests pass

## Validation Evidence

- Added dedicated compression provider project `RepoMigrator.Providers.Compression.Zip`
- Implemented `ZipArchiveDriver` in `RepoMigrator/RepoMigrator.Providers.Compression.Zip/ZipArchiveDriver.cs`
- Added targeted MSTest coverage in `RepoMigrator/RepoMigrator.Tests/ZipArchiveDriverTests.cs`
- Workspace build passed after integration
