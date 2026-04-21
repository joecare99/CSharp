# Task T-830302-012 - Integrate TraceCsv2realCsv with shared TraceAnalysis intake filters

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Refactor `TraceCsv2realCsv` to use the shared `TraceAnalysis` input-filter selection path instead of direct format-specific parsing.

## Scope

- Replace direct `TraceCsv` parsing with selector-based canonical import
- Support `.csv`, `.txt`, and `.trace` inputs through the shared intake path
- Reuse the canonical CSV output filter for converter output
- Keep tool behavior deterministic when multiple filters match the same source

## Implementation Notes

- Use `IAnalyzableInputFilter` and `InputFilterSelector`
- Adapt existing CSV-oriented input filters so they participate in deterministic selection
- Keep console entry behavior minimal while moving conversion logic into a testable class
- Preserve the converter's role as a file-to-file transformation tool

## Dependencies

- `T-830302-003` - `Specify input filter interface and selection strategy`
- `T-830302-008` - `Implement MOVIRUN trace text input filter`
- `T-830302-010` - `Implement MOVIRUN XML trace input filter`

## Done Criteria

- `TraceCsv2realCsv` uses shared TraceAnalysis intake filters
- Existing CSV inputs still convert successfully
- `.txt` MOVIRUN traces convert through the same intake path
- `.trace` MOVIRUN XML traces convert through the same intake path
- Output writing uses the canonical CSV export path
