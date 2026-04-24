# Task T-830352 - Identify first supported source formats

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

List the first source formats that must be supported by the initial input filter design.

## Current Domain Decision

- The first supported format is a flat `CSV` file with a header row in the first line
- The second supported format is the `CSV` shape produced or consumed by the `TraceCsv2realCsv` project
- Both formats may share one input-filter project if that stays simple and justified, but they may also be separated if their parsing rules diverge too much
- The `MOVIRUN` trace text export (`.txt`) is now included as a third supported baseline source format because it is already present in repository test data
- The XML-based `MOVIRUN` `.trace` export is now included as a fourth supported baseline source format because repository test data shows it carries the same measurement series as the related `.trace.csv` exports
- Further source formats are explicitly deferred beyond these four baseline formats

## Initial Format List

### 1. Flat CSV with header row

- Priority: first
- Rationale: simplest useful baseline for the import pipeline and easiest format for validating canonical pass-through behavior
- Expected characteristics: delimited text file, first row contains column names, flat field structure, no mandatory field groups
- Known risks: delimiter variants, quoting and escaping behavior, encoding differences, inconsistent timestamp column naming

### 2. TraceCsv2realCsv project CSV format

- Priority: second
- Rationale: directly relevant to the existing repository context and likely to contain trace-specific conventions needed by the epic
- Expected characteristics: CSV-based input with project-specific naming and trace-related field semantics
- Known risks: format rules may differ from flat CSV enough to require separate parsing rules, additional metadata conventions may exist, field grouping may need to be inferred from structured field names

### 3. MOVIRUN trace text export

- Priority: third
- Rationale: repository test data already contains real trace exports in this text format, and the fixed preamble plus whitespace-separated table structure justify a dedicated input filter
- Expected characteristics: text file with a fixed signature line, source-path preamble line, `Zeitstempel(...)` header row, and whitespace-separated data rows
- Known risks: timestamp unit is embedded in the first header token, row separators are whitespace rather than explicit CSV delimiters, malformed rows should not block partial imports

### 4. MOVIRUN XML trace export

- Priority: fourth
- Rationale: repository test data already contains `.trace` XML files that hold the same measurement series as the exported `.trace.csv` files, making them a high-value direct import source
- Expected characteristics: XML document with `Trace` root, configuration metadata, and `TraceData/TraceRecord/TraceVariable` elements containing `Values` and `Timestamps` series
- Known risks: per-variable series must be correlated by timestamp, malformed or unequal `Values`/`Timestamps` lengths should not discard all imported data, and technical XML configuration should not be mixed into regular data columns

## Deferred Formats

- Generic `JSON`
- Generic `XML`
- Proprietary binary or container formats
- Direct database or service-based imports

## Done Criteria

- The first flat `CSV` format with header row is documented
- The `TraceCsv2realCsv` project `CSV` format is documented as the second supported format
- The `MOVIRUN` trace text export is documented as the third supported format
- The XML-based `MOVIRUN` `.trace` export is documented as the fourth supported format
- Each selected format includes a short rationale and known parsing risks
- Deferred formats are marked explicitly
