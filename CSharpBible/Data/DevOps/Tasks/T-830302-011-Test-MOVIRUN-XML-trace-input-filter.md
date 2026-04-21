# Task T-830302-011 - Test MOVIRUN XML trace input filter

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Verify detection, timestamp correlation, and partial-result behavior of the MOVIRUN XML trace input filter.

## Test Scope

- Detect valid `.trace` XML files
- Reject non-matching XML safely
- Parse canonical records from `ms`-style samples
- Parse canonical records from `µs`-style samples
- Verify timestamp-based merging across multiple variables
- Preserve valid records when one variable series is malformed

## Test Notes

- Use `MSTest`
- Prefer compact representative XML samples for unit tests
- Include parse-error assertions for mismatched `Values` and `Timestamps`
- Keep assertions deterministic across supported target frameworks

## Done Criteria

- Detection tests cover the known `.trace` XML structure
- Parsing tests cover timestamp correlation and canonical record mapping
- Partial-result behavior is validated with malformed variable series
- Tests pass in the TraceAnalysis test project
