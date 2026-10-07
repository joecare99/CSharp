# OFB-T001 Consolidate OFBCreator Project Configuration and Tests

## Parent
- Local planning mirror: `DevOps/BacklogItems/OFB-BL001-Consolidate-Project-Configuration-and-Tests.md`
- Canonical Avalonia backlog: `C:\Projekte\CSharp\DevOps\Backlogs\BLK-055-avalonia-host.md`
- Canonical implementation task: `C:\Projekte\CSharp\DevOps\Tasks\TASK-055.3-implement-avalonia-selection-progress.md`
- Canonical test task: `C:\Projekte\CSharp\DevOps\Tasks\TASK-055.5-test-avalonia-host.md`

## Scope
Consolidate the OFBCreator solution folder and active test-framework matrix; resolve the OFBProjectStore ambiguity; add Avalonia project/GEDCOM/DOCX path dialogs and standard-path watermarks. Linked external projects remain dependency-only and are not modified.

## Expected Behavior
- OFBCreator projects and tests follow the active net8.0/net9.0/net10.0 target matrix where appropriate.
- Project open/save-as and folder selection, GEDCOM open, and DOCX save-as/folder selection use injectable Avalonia StorageProvider dialogs.
- Empty project/output path fields show a standard path under Documents/OFBCreator; selecting a project path moves the output default to that project's directory.
- The solution groups its OFBCreator test projects coherently and builds without compiler errors.

## Planned Tests
- Success: full restore/build/test on every active target framework.
- Boundary: verify default paths before and after project-path selection and ensure dialog cancellation leaves selected paths unchanged.
- Error: distinguish shared-package NU1506 warnings from OFBCreator build/test failures.
- Regression: verify file-dialog command results and default project/DOCX filenames with MSTest and NSubstitute.

## Validation
- Baseline: all 617 discovered test executions passed, but the IDE-reported build had stale CS0104/CS1061 diagnostics and downstream CS0006 errors. A sequential project build confirmed the DOCX `AddSection` API exists and compiled.
- Final restore and serial full solution build succeeded for the active net8.0/net9.0/net10.0 framework set.
- Final full solution test run: 744 passed, 0 failed, 0 skipped. Avalonia tests: 33 passed across all three frameworks.
- Restore reports non-blocking NU1506 duplicate `Microsoft.Extensions.Logging.Abstractions` versions in shared package metadata; the shared package file was not changed because it is outside the selected project-code scope.
- Canonical BLK-055 and TASK-055.3/TASK-055.5 records are updated. Repository commit/CI gate remains pending.

## Status
Implemented and validated; awaiting repository commit/CI completion gate
