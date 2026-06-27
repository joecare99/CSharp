# Feature F-GEN-DRIVERS-002: Create Separate GEDCOM Import/Export Driver

## Parent

- Epic `E-GEN-DRIVERS-001` - Genealogy Import/Export Driver Separation

## Objective

Create a separate GEDCOM driver project that imports and exports GEDCOM files by mapping GEDCOM records to and from the central genealogy model.

## Value

- Keeps GEDCOM-specific parsing and writing separate from HEJ compatibility code.
- Allows GEDCOM dialect/version decisions to be tested and evolved independently.
- Provides a reusable driver for UI, console, migration, and automation scenarios.

## Scope

- Define a new GEDCOM driver class library project.
- Move or rework existing GEDCOM reader concepts from `WinAhnenCls\Model\GenBase` only after tests document required behavior.
- Add GEDCOM writer/export support.
- Add resource-based import, export, and round-trip tests.
- Review legacy `GedLes` only as a negative/comparison reference without directly coupling to its UI or legacy runtime assumptions.

## Non-Goals

- No WinForms or UI dependencies in the GEDCOM driver.
- No forced reuse of legacy `GedLes` code before dependency review.
- No immediate support promise for every GEDCOM dialect.

## Proposed Project Shape

- Production project candidate in a neutral solution area: `Genealogy.GedCom\Genealogy.GedCom.csproj`
- Test project candidate in a neutral solution area: `Genealogy.GedCom.Tests\Genealogy.GedCom.Tests.csproj`
- References:
  - `..\WinAhnenNew\BaseGenClasses\BaseGenClasses.csproj`
  - `..\CSharpBible\Libraries\GenInterfaces\GenInterfaces.csproj`
  - `..\CSharpBible\Libraries\BaseLib\BaseLib.csproj` only if file abstractions are needed directly

## Acceptance Criteria

1. The GEDCOM driver project has a documented public import/export surface.
2. GEDCOM parsing handles at least the baseline records needed by existing resources.
3. GEDCOM export can produce a standards-oriented file from the central model.
4. Dialect/version support and unsupported GEDCOM tags are documented through diagnostics.

## Assumptions

- The driver should read GEDCOM `5.5`, `5.5.1`, and `7`, with a slight implementation preference for `5.5.1`.
- Existing sample `Care_exp.ged` and other GEDCOM resources can seed regression tests.
- Export should select the target GEDCOM version through user choice or configuration.
- Unsupported or foreign GEDCOM tags should be handled by a configurable mode that either preserves them or reports them diagnostically.

## Open Questions

- Which concrete extension-data shape should the shared model use when GEDCOM special or foreign tags are preserved?
