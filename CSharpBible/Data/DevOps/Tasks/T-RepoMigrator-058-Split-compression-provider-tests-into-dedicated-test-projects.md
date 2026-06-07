# T-RepoMigrator-058 Split compression provider tests into dedicated test projects

## Summary
Create dedicated MSTest projects for `RepoMigrator.Providers.Compression.TarGz` and `RepoMigrator.Providers.Compression.Zip` coverage and remove the migrated compression-focused tests from the aggregate `RepoMigrator.Tests` project without changing the solution file.

## Scope
- Create `RepoMigrator.Providers.Compression.TarGz.Tests`
- Move `TarGzArchiveDriverTests`
- Create `RepoMigrator.Providers.Compression.Zip.Tests`
- Move `ZipArchiveDriverTests`
- Remove the direct compression provider references from `RepoMigrator.Tests`
- Keep the solution file untouched

## Validation
- Run the dedicated `RepoMigrator.Providers.Compression.TarGz.Tests` project
- Run the dedicated `RepoMigrator.Providers.Compression.Zip.Tests` project
- Run a workspace build

## Outcome
- Created `RepoMigrator.Providers.Compression.TarGz.Tests` as a dedicated MSTest project.
- Moved `TarGzArchiveDriverTests` out of `RepoMigrator.Tests`.
- Created `RepoMigrator.Providers.Compression.Zip.Tests` as a dedicated MSTest project.
- Moved `ZipArchiveDriverTests` out of `RepoMigrator.Tests`.
- Removed the direct compression provider references from `RepoMigrator.Tests`.
- Left the solution file untouched.

## Verification
- `RepoMigrator.Providers.Compression.TarGz.Tests`: 6/6 tests passed, 0 failed, 0 skipped.
- `RepoMigrator.Providers.Compression.Zip.Tests`: 5/5 tests passed, 0 failed, 0 skipped.
- Workspace build: succeeded.

## Status
Done
