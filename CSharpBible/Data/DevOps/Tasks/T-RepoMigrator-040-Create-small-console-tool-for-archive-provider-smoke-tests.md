# Task T-RepoMigrator-040 - Create small console tool for archive provider smoke tests

## Status

Done

## Parent

- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`
- Task `T-RepoMigrator-036` - `Create archive provider project and move archive-specific models out of core`

## Goal

Create a small standalone console tool that exercises the archive provider against a local archive directory and prints a readable inspection and plan preview.

## Scope

- Create a dedicated small console project for archive smoke tests
- Reference the archive provider and the available compression provider projects
- Support local archive directory input with optional recursion
- Inspect the matching archives and prepare an archive import plan preview
- Persist the prepared plan under the existing DevOps archive-import state store layout

## Detailed Work Packages

1. Project creation
   - create a new net9.0 executable project under `RepoMigrator.Tools`
   - reference `RepoMigrator.Core`, `RepoMigrator.Providers.Archive`, `RepoMigrator.Providers.Compression.Zip`, and `RepoMigrator.Providers.Compression.TarGz`
2. Tool implementation
   - add simple option parsing for source directory, recursion, and extension selection
   - inspect archives through the registered archive drivers
   - prepare an archive import plan preview using the archive planner
   - print a concise readable console summary
3. Validation
   - add the tool project to the solution
   - build the workspace successfully

## Deliverables

- New small archive-test console tool project
- Console output for archive inspection and plan preview
- Solution updated to include the new tool project

## Dependencies

- `T-RepoMigrator-036` - `Create archive provider project and move archive-specific models out of core`
- `T-RepoMigrator-039` - `Create TarGz compression provider project and implement tar.gz driver there`

## Acceptance Criteria

- The tool can scan a local archive directory and recognize supported archives
- The tool can prepare an archive import plan preview using the archive provider services
- The workspace build succeeds after the tool is added
