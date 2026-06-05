# Task T-RepoMigrator-050 - Fix convert_patches behavior with regression tests before refactor

## Status

Draft

## Parent

- Task `T-RepoMigrator-010` - `Use differential patches for SVN sync`
- Task `T-RepoMigrator-011` - `Close priority test coverage gaps`

## Goal

Capture the current observable behavior of `C:\Projekte\Cmd\convert_patches.ps1` in automated regression tests before refactoring the script into a unified single-path processing flow.

## Scope

- Add black-box automated tests that execute `convert_patches.ps1` with temporary input patches
- Verify key observable conversion behavior for small and split outputs
- Verify SVN metadata cleanup and `svn:ignore` to `.gitignore` conversion behavior
- Preserve current output naming and `new file mode` index insertion behavior during later refactoring

## Deliverables

- Automated regression tests for `convert_patches.ps1`
- Temporary patch fixtures created within tests
- Validation evidence showing the script behavior is fixed before refactoring

## Detailed Work Packages

1. Add a black-box MSTest class for `convert_patches.ps1`
2. Cover single-output conversion for small patch files
3. Cover split-output conversion with a low threshold for deterministic tests
4. Cover SVN metadata cleanup and `.gitignore` diff generation
5. Validate the test suite and workspace build after adding the regression coverage

## Acceptance Criteria

- The tests execute `C:\Projekte\Cmd\convert_patches.ps1` as an external black-box workflow
- The tests verify the expected `.gitpatch` output for a small patch input
- The tests verify `.part_XX.gitpatch` output for a forced split scenario
- The tests verify cleanup of SVN-only metadata and conversion of `svn:ignore` into `.gitignore`
- The tests verify `new file mode` handling inserts the zero hash `index` line
- The targeted tests pass before the later script refactor starts

## Risks

- The script under test lives outside the repository root and therefore depends on the local machine path remaining stable
- PowerShell process execution may fail if the environment lacks script execution permissions
- Refactoring may intentionally change some currently implicit edge-case behavior that needs an explicit test decision first

## Open Questions

- Should the later refactor keep the exact current split naming behavior, or should naming be revisited after the logic is unified?
- Should the script eventually move into the repository so its tests and implementation version together?
