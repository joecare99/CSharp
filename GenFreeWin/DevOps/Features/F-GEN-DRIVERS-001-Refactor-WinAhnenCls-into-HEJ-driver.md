# Feature F-GEN-DRIVERS-001: Refactor WinAhnenCls into HEJ Import/Export Driver

## Parent

- Epic `E-GEN-DRIVERS-001` - Genealogy Import/Export Driver Separation

## Objective

Refactor `..\WinAhnenNew\WinAhnenCls\WinAhnenCls.csproj` so it only contains `.hej` file-format import/export behavior and maps that format to and from the shared genealogy model in `BaseGenClasses`.

## Value

- Preserves existing HEJ compatibility while removing generic genealogy ownership from the HEJ project.
- Makes the HEJ implementation easier to test with resource-based fixtures.
- Enables other applications and tools to consume `.hej` import/export through a clean driver API.

## Scope

- Inventory current HEJ-specific and generic classes in `WinAhnenCls`.
- Move reusable model/helper logic to `BaseGenClasses` where it is not format-specific.
- Keep HEJ-specific parsing, field mapping, separators, encoding behavior, and export formatting in `WinAhnenCls`.
- Add driver-level tests for import, export, and round-trip behavior.

## Non-Goals

- No GEDCOM implementation in `WinAhnenCls` after the split.
- No UI-facing strings or UI workflows in the driver.
- No broad `ArtifactsPath` or storage migration.

## Proposed Boundaries

### BaseGenClasses

- Core genealogy entity model.
- Fact, date, connection, place, source, repository, media, and transaction concepts.
- Shared builders/converters that are independent from `.hej` syntax.
- No driver-specific import/export abstractions; those belong in `GenInterfaces`.

### WinAhnenCls HEJ driver

- `.hej` stream reader and writer.
- HEJ section handling such as individuals, marriages, adoptions, places, and sources.
- HEJ field enumerations and compatibility mapping.
- Encoding, delimiter, and legacy format tolerance.

## Acceptance Criteria

1. `WinAhnenCls` no longer owns generic genealogy model classes that belong in `BaseGenClasses`.
2. The project exposes a small HEJ driver surface for import and export.
3. Existing `.hej` resource tests are migrated or retained as regression coverage.
4. Export behavior is covered by fixture or snapshot-style tests where practical.

## Assumptions

- Current `.hej` tests only cover part of the real format and must be expanded before broad refactoring.
- Existing `CHejGenealogy` behavior may be treated as compatibility behavior, not as the target domain model.
- HEJ driver APIs should be asynchronous and use the shared result/diagnostics model from `GenInterfaces`.
- HEJ export is a secondary concern compared to import and should default to semantic equivalence.

## Open Questions

- Which existing `WinAhnenCls` classes are purely HEJ-specific and which should move to `BaseGenClasses`?
- Which concrete parts of HEJ export need a later opt-in legacy-format mode beyond the default semantic export?
