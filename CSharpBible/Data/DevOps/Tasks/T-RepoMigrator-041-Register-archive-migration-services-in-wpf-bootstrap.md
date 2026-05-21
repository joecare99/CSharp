# Task T-RepoMigrator-041 - Register archive migration services in WPF bootstrap

## Status

Completed

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

## Validation Evidence

- Updated WPF DI registration in `RepoMigrator/RepoMigrator.App.Wpf/DI/BootStrap.cs`
- Registered archive planner, migration service, runtime-defined archive state store, extraction-root helpers, and Zip/TarGz drivers
- Added regression coverage in `RepoMigrator/RepoMigrator.Tests/MainViewModelStartWorkflowTests.cs`
- Workspace build passed after the registration changes

## Follow-up Maintenance Note

- A later WPF validation against `C:\Projekte\Cpp\xpdf` exposed that the app-local archive workflow still constrained `AllowedExtensions` to `.zip` and `.tar.gz`, even after the archive provider and TarGz driver were extended to support `.tgz`.
- `MainViewModel` now uses one shared archive extension list for both source probing and archive workflow execution: `.zip`, `.tar.gz`, and `.tgz`.
- Added regression coverage in `RepoMigrator.Tests.MainViewModelStartWorkflowTests` to verify that the WPF source test path counts both `.tar.gz` and `.tgz` archive snapshots.
- Validation after the follow-up fix:
  - workspace build passed,
  - full `RepoMigrator.Tests` execution passed with `237/237` tests successful and `0` skipped.
