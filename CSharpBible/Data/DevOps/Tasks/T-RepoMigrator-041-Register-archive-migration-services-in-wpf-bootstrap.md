# Task T-RepoMigrator-041 - Register archive migration services in WPF bootstrap

## Status

Done

## Parent

- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`
- Task `T-RepoMigrator-040` - `Create small console tool for archive provider smoke tests`

## Goal

Register the archive migration services and required provider bridges in the WPF application bootstrap so the desktop application can resolve the archive workflow components.

## Scope

- Add WPF-local DI bridge classes for archive source and destination provider resolution
- Register archive planner, archive migration service, archive state store, and archive driver registry
- Register Zip and TarGz archive drivers in the WPF application container
- Keep the existing repository migration registrations intact

## Detailed Work Packages

1. DI bridge registration
   - add an application-local `IMigrationSourceProviderFactory` implementation for archive collection inputs
   - add an application-local `IMigrationDestinationProviderFactory` implementation for Git-backed archive migration targets
2. Archive service registration
   - register archive planner and execution services
   - register archive state persistence and extraction-root helpers
   - register Zip and TarGz drivers through the archive driver registry
3. Validation
   - build the workspace successfully after registration changes

## Deliverables

- New WPF-local DI bridge classes for archive migration
- Updated `Bootstrap` registration for archive services
- Successful build validation

## Dependencies

- `T-RepoMigrator-036` - `Create archive provider project and move archive-specific models out of core`
- `T-RepoMigrator-039` - `Create TarGz compression provider project and implement tar.gz driver there`

## Acceptance Criteria

- The WPF application container can resolve archive migration services
- Zip and TarGz drivers are registered for archive inspection and extraction
- Existing Git and SVN repository migration registration remains intact
- The workspace build succeeds
