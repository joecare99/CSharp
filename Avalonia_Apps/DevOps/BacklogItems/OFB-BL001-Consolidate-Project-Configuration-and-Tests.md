# OFB-BL001 Consolidate Project Configuration and Tests

## Scope
Align OFBCreator solution organization and active target frameworks, resolve verified compilation issues, and add the Avalonia path-selection dialogs and default-path watermarks. Linked external projects remain dependency-only and are not modified.

## Value
A coherent solution and usable project/source/output selection workflow make the Avalonia host buildable, testable, and practical for project export.

## Assumptions
- OFBCreator projects and their tests are the implementation scope.
- The current Avalonia host supports GEDCOM input and DOCX output; ODT remains unavailable.
- Empty project/output paths use `Documents/OFBCreator` until a project location is chosen.
- Canonical Avalonia tracking is `C:\Projekte\CSharp\DevOps\Backlogs\BLK-055-avalonia-host.md` with implementation/test tasks TASK-055.3 and TASK-055.5.

## Acceptance Criteria
- OFBCreator project settings and test matrices are consistent for net8.0/net9.0/net10.0.
- All OFBCreator projects and tests are coherently grouped in the solution.
- Native file and directory selection supports project files, GEDCOM source, DOCX output and their folders.
- Empty project/output path fields display appropriate dynamic default-path watermarks.
- Full restore/build and tests pass for active target frameworks; remaining external/shared warnings are documented.

## Tasks
- [x] Implement the project graph/configuration and Avalonia dialog changes (OFB-T001; canonical TASK-055.3).
- [x] Add and run dialog/path regressions (canonical TASK-055.5).
- [ ] Record the repository commit/CI completion gate in the canonical tracking items.

## Validation
- Full solution build succeeded.
- 744 tests passed, 0 failed, 0 skipped across net8.0/net9.0/net10.0; Avalonia tests contributed 33 passing executions.
- Restore still reports NU1506 duplicate Microsoft.Extensions.Logging.Abstractions package versions in shared package metadata.

## Status
Implementation and validation complete; commit/CI gate pending.
