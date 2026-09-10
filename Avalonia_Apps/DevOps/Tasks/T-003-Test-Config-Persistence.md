# T-003 Test Config Persistence

## Parent
B-Config-Core | Previous: T-002 | Next: T-004

## Status
Done

## Request
Define and secure persistence semantics for load, save, reset, defaults, validation, vendor/application root, and CONFIG_ROOT override using isolated test storage.

## Planned tests
- Save/load round trip for a registered model.
- Missing document returns provider defaults.
- Reset affects only the requested section.
- CONFIG_ROOT overrides the default local application-data root.
- Invalid JSON, absent property, unsupported enum value, and inaccessible directory return defined results.
- String and enum lookup fallback behavior is preserved.

## Persistence semantics

- The default storage path is `%LOCALAPPDATA%/<Vendor>/<Application>/config`.
- A non-empty `CONFIG_ROOT` replaces the local-application-data root while
  preserving the vendor, application, and `config` path segments.
- Each section is stored as `<SectionName>.json`; nested section names are
  represented by the platform path separator through `Path.Combine`.
- Saves use indented JSON and camel-case property names.
- Missing files, empty reset files, malformed JSON, and JSON documents that
  deserialize to `null` return the caller-provided fallback value.
- Missing JSON properties retain the defaults already present in the fallback
  model instance.
- Reset affects only the requested section by replacing its document with an
  empty file. The next load therefore returns the fallback; other section
  documents remain unchanged.
- Unsupported enum text is handled by the `ConfigService` lookup layer and
  returns the requested enum fallback. Missing properties likewise return the
  lookup fallback.
- Storage-root creation failures and save I/O failures are not converted into
  fallback results; they propagate to the caller. The current test suite
  documents deterministic read/persistence behavior without relying on
  platform-specific permission manipulation for inaccessible directories.

## Test evidence

- Added per-test temporary `CONFIG_ROOT` setup and cleanup.
- Added tests for reset isolation, root override path construction, malformed
  JSON, missing-property defaults, unsupported enum fallback, and missing
  section lookup fallbacks.
- Complete suite: 19 tests passed, 0 failed, 0 skipped on `net8.0`.
- `Config.Service` build passed on `net8.0`.
- `Config.Service` build passed on available `net9.0`.
- Existing nullable warnings remain in test fixtures and are not production
  warnings.

## Gate decision

T-003 is approved for completion. No production correction was required after
the persistence tests were added; the existing reset and failure semantics are
now explicit and regression-tested. T-004 may start the release/documentation
validation.

## Gate and completion
Before Done: all listed MSTests pass, failures are diagnostically described, net8.0 build passes, changes receive English SVN commit with recorded revision, this/next/backlog/feature statuses are updated.

## SVN and status handover

- SVN revision: 1837.
- English commit message: `Secure Config Service persistence` — add isolated
  persistence tests and document T-003 semantics.
- Next task: T-004 Release Config Core is In Progress.