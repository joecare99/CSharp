# Task T-830302-010 - Implement MOVIRUN XML trace input filter

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Implement a robust input filter for the XML-based `MOVIRUN` `.trace` export format.

## Scope

- Detect the `.trace` XML signature reliably
- Parse `TraceData/TraceRecord/TraceVariable` measurement series
- Correlate `Values` and `Timestamps` into canonical trace records by timestamp
- Keep XML configuration data secondary unless it directly affects measurement interpretation
- Preserve successful records when malformed variable series are encountered
- Emit parse errors with variable or series context where available

## Format Baseline

- Root element `Trace`
- Configuration section `TraceConfiguration`
- Data section `TraceData`
- One or more `TraceRecord` elements
- `TraceVariable` entries with `VarName`, `Values`, and `Timestamps`

## Implementation Notes

- Place the filter in a dedicated input-filter project because the XML parsing behavior diverges from CSV- and text-oriented filters
- Use deterministic XML structure analysis for `CanHandle` or `Analyze`
- Parse numeric values using invariant culture where applicable
- Correlate records by timestamp rather than assuming one flat row block in the source
- Continue collecting partial results for recoverable variable or row issues

## Dependencies

- `T-830302-001` - `Identify first supported source formats`
- `T-830353` - `Specify input filter interface and selection strategy`

## Done Criteria

- A dedicated MOVIRUN XML trace input filter exists
- Format detection works for the known `.trace` XML structure
- Trace-variable series are merged into canonical records by timestamp
- Row- or series-level parse errors are collected without discarding successful records
- The implementation works for the known `ms` and `µs` sample variants
