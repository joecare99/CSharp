# Backlog Item BI-830334 - Create CSV output filter

## Status

Draft

## Parent

- Epic `830302` - `TraceAnalysis and AGV-reverse simulation`
- Feature `830329` - `Filter-based trace intake and export foundation`

## Goal

Export canonical exchange records to CSV in a deterministic and documented format.

## Description

The CSV output filter should serialize the shared intermediate structure with stable column order and predictable formatting rules so that users can inspect and reuse exported data in common tools.

The contract must keep technical metadata separate from data value columns. `timestamp` is mandatory and must always be exported. Other fields are optional and exported when present.

`timestamp` uses invariant ISO 8601 UTC format: `yyyy-MM-ddTHH:mm:ss.fffZ`.

Delimiter handling should support auto-detection (`;` and `<tab>`) by probing first lines, with optional user override. Detection heuristics should start with international/general assumptions and continue with country-specific assumptions.

Output format selection belongs to the output-filter layer: either explicit manual user selection, or extension-based selection with a fixed entropy baseline and no extra analysis. For `.csv`, the default expected output is flat CSV.

Flat CSV has no dedicated metadata except what can be inferred from first lines. Extended per-column metadata is handled in a separate companion header file. The companion header file uses the same delimiter and column order as the data CSV.

Companion header file rows:

1. Column names (alphanumeric plus `.`, `-`, `_`; spaces normalized to `_`)
2. Optional format/type per column
3. Group name per column (optional)
   - Empty value means no group (single column)
   - Non-empty identifier means group membership, including valid one-column groups
4. Group description, optionally continued in following rows when needed
   - Row 4 is the first description line
   - Row 5+ are continuation lines in original order
   - Continuation ends at end-of-file
   - Empty cells in continuation rows add no text for that column

Multi-value split logic is not part of the direct import filter. Import reads raw format first; any splitting into multiple columns is handled by downstream general filters.

## Acceptance Criteria

- Column order is documented
- Headers are defined
- Escaping and delimiter rules are defined
- Missing optional values are handled consistently
- Exported CSV can be produced directly from canonical exchange records
- Metadata is not mixed into data columns
- Companion header file behavior for import and export is documented
- Companion header row schema is documented
- Large-file export behavior and partial-failure reporting are documented
- Delimiter auto-detection and user override behavior are documented

## Dependencies

- `BI-830331` - `Define canonical trace exchange model`

## Candidate Tasks

- `T-830348` - `Specify CSV column mapping and export behavior`

## Open Questions

- No blocking open questions for the first increment
