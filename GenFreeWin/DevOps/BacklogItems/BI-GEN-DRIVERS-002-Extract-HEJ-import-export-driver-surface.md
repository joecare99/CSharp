# Backlog Item BI-GEN-DRIVERS-002: Extract HEJ Import/Export Driver Surface

## Parent

- Feature `F-GEN-DRIVERS-001` - Refactor WinAhnenCls into HEJ Import/Export Driver

## Objective

Define and implement a focused `.hej` driver surface in `WinAhnenCls` that imports and exports genealogy data through shared model abstractions.

## Value

- Makes `WinAhnenCls` a clear file-format adapter instead of a mixed model and parser project.
- Provides a stable API for consuming `.hej` files from UI, console, and automation workflows.
- Enables targeted HEJ compatibility tests.

## Scope

- HEJ import from stream or file abstraction.
- HEJ export to stream or file abstraction.
- Mapping between HEJ fields and `BaseGenClasses`/`GenInterfaces` model objects.
- Diagnostics for unsupported or malformed HEJ sections.

## Deliverables

- Public HEJ import/export API proposal.
- HEJ mapping matrix for individuals, marriages, adoptions, places, and sources.
- Driver tests using existing `.hej` resources.
- Export fixture tests for representative genealogies.

## Acceptance Criteria

1. The driver API is small and format-specific.
2. Import maps known HEJ sections into the central genealogy model.
3. Export writes HEJ-compatible content for supported model data.
4. Unsupported sections are reported predictably and do not crash valid partial imports.

## Assumptions

- The existing separator and encoding behavior must be preserved unless tests prove a safer alternative.
- Export formatting needs explicit fixtures because round-trip equality may not be byte-identical for legacy files.
- The driver surface should be asynchronous and return a shared result object from `GenInterfaces`.
- Standard HEJ export should target semantic equivalence first; legacy-format output should be treated as an optional later mode.

## Open Questions

- Which HEJ export details must be covered by the later legacy-format mode because semantic output is not sufficient for real consumers?
