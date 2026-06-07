# Task T-RepoMigrator-050 - Abstract app-state credential protection for platform neutrality

## Status

In Progress

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`

## Goal

Remove the Windows-specific credential protection dependency from `RepoMigrator.App.State` by introducing DI-friendly abstractions for state file location and secret protection so the non-UI app layers stay platform-neutral.

## Scope

- Introduce interfaces for app-state persistence and secret protection
- Refactor the app-state store to use injected abstractions instead of `ProtectedData`
- Move Windows-specific DPAPI protection and storage-path resolution to the WPF composition layer
- Retarget `RepoMigrator.App.State` away from `net9.0-windows`
- Preserve compatibility for already persisted Windows-protected credentials in the current WPF app
- Add regression tests for the refactored store and WPF-facing wiring

## Acceptance Criteria

- `RepoMigrator.App.State` no longer directly references Windows-only cryptography APIs
- `RepoMigrator.App.State` builds as a platform-neutral project target
- `MainViewModel` depends on an app-state persistence abstraction instead of the concrete store type
- WPF registers Windows-specific credential protection through DI
- Existing persisted protected credentials remain readable in the Windows UI path
- Relevant build and test validation succeeds

## Validation Evidence

- Pending implementation
