# Backlog Item BI-830332 - Create pluggable input filters for initial source formats

## Status

Draft

## Parent

- Epic `830302` - `TraceAnalysis and AGV-reverse simulation`
- Feature `830329` - `Filter-based trace intake and export foundation`

## Goal

Support one or more source formats through interchangeable input filters that map imported content into the canonical exchange model.

## Description

Input filters should isolate parsing and source-specific transformation logic. New source formats should be addable without redesigning the export path.

The first design should prefer stream-based input with an additional source identifier instead of a hard file-only contract. The filter contract should support general metadata, field-specific metadata such as format and type, and field groups while keeping technical metadata clearly separated from actual data values.

Field groups are optional. For flat CSV files, no field groups may exist. Field groups should only be derived when field names show clear shared prefixes or structural paths, for example `AGV1.X` and `AGV1.Y` or `Diag1.Axis1.Speed` and `Diag1.Axis2.Speed`.

For inferred field groups, prefer `.` as separator, also allow `_`, and do not infer groups from prefix-only naming without a separator.

The first supported source format is a flat `CSV` file with a header row in the first line. The second supported format is the `CSV` variant from the `TraceCsv2realCsv` project. A third supported source format is the whitespace-separated `MOVIRUN` trace text export with a fixed preamble and a `Zeitstempel(...)` header line. A fourth supported source format is the XML-based `MOVIRUN` `.trace` export that contains the same measurement series in structured `TraceVariable` elements.

## Acceptance Criteria

- A documented input filter contract exists
- The selection strategy for input filters is defined
- Deterministic ranking rules are defined when multiple filters match the same source
- At least one initial source format is covered by the design
- Unsupported formats produce a defined failure path
- Parsed records are emitted as canonical exchange records
- The contract supports general metadata, field-specific metadata, and field groups
- Partial results and logged parse errors are part of the design for large files
- Filters are registered via interface plus Dependency Injection
- Project structure direction for base interfaces and input/output separation is documented
- Each major coding task has a dedicated test task item

## Dependencies

- `BI-830331` - `Define canonical trace exchange model`

## Candidate Tasks

- `T-830302-001` - `Identify first supported source formats`
- `T-830302-003` - `Specify input filter interface and selection strategy`
- `T-830302-006` - `Define base filter interface project structure`
- `T-830302-007` - `Test T-830302-003 input filter selection and analysis`
- `T-830302-008` - `Implement MOVIRUN trace text input filter`
- `T-830302-009` - `Test MOVIRUN trace text input filter`
- `T-830302-010` - `Implement MOVIRUN XML trace input filter`
- `T-830302-011` - `Test MOVIRUN XML trace input filter`
- `T-830302-012` - `Integrate TraceCsv2realCsv with shared TraceAnalysis intake filters`
- `T-830302-013` - `Test TraceCsv2realCsv shared intake integration`

## Current Source Format Direction

- Detect flat `CSV` files primarily by file extension and then confirm them through file inspection (`header + n lines`)
- Detect the `TraceCsv2realCsv` project format by extension plus format-specific decision lines or `n KB` analysis block output
- Detect the `MOVIRUN` trace text export through its signature line, `Zeitstempel(...)` header row, and whitespace-separated sample rows
- Detect the XML-based `MOVIRUN` `.trace` export through the `Trace` root plus `TraceData/TraceRecord/TraceVariable` structure and paired `Values`/`Timestamps` series
- Keep manual override available later when automatic detection cannot distinguish similar `CSV` variants reliably

## Architecture Direction

- Provide one or more base interface projects used by input and output filter implementations
- Keep input and output filter implementations in separate projects
- Combine multiple input filters in one project only when this is clearly justified by shared parsing behavior
