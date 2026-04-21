# Task T-830302-008 - Implement MOVIRUN trace text input filter

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Implement a robust input filter for the whitespace-separated `MOVIRUN` trace text export format.

## Scope

- Detect the `MOVIRUN` text trace signature reliably
- Parse the fixed preamble and `Zeitstempel(...)` header row
- Parse whitespace-separated data rows into canonical trace records
- Keep the timestamp as the canonical mandatory record key
- Preserve successful rows when malformed rows are encountered
- Emit parse errors with line-level context where available

## Format Baseline

- Line 1: `MOVIRUN` trace signature
- Line 2: source path or trace-origin text
- Line 3: whitespace-separated header row beginning with `Zeitstempel(...)`
- Line 4+: whitespace-separated data rows

## Implementation Notes

- Place the filter in a dedicated input-filter project because the parsing behavior diverges from the CSV-oriented filters
- Use deterministic header and sample-row analysis for `CanHandle` or `Analyze`
- Parse numeric values using invariant culture
- Support at least `Zeitstempel(ms)` and `Zeitstempel(µs)` header variants in the first increment
- Continue collecting partial results for recoverable row issues

## Dependencies

- `T-830302-001` - `Identify first supported source formats`
- `T-830353` - `Specify input filter interface and selection strategy`

## Done Criteria

- A dedicated MOVIRUN text trace input filter exists
- Format detection works for the known text trace signature
- Header and row parsing produce canonical records
- Row-level parse errors are collected without discarding successful rows
- The implementation supports the known `ms` and `µs` timestamp header variants
