# Task T-830302-009 - Test MOVIRUN trace text input filter

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Verify detection, parsing, and partial-result behavior of the MOVIRUN trace text input filter.

## Test Scope

- Detect valid `MOVIRUN` text traces
- Reject non-matching content safely
- Parse `Zeitstempel(ms)` input
- Parse `Zeitstempel(µs)` input
- Preserve valid rows when malformed rows are present
- Verify canonical field metadata and record mapping

## Test Notes

- Use `MSTest`
- Prefer compact representative text samples for unit tests
- Include parse-error assertions for malformed row scenarios
- Keep assertions deterministic across supported target frameworks

## Done Criteria

- Detection tests cover the known format signature
- Parsing tests cover `ms` and `µs` timestamp header variants
- Partial-result behavior is validated with malformed-row input
- Tests pass in the TraceAnalysis test project
