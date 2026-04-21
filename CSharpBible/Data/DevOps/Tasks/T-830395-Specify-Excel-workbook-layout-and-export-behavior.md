# Task T-830395 - Specify Excel workbook layout and export behavior

## Status

Draft

## Parent

- Backlog Item `BI-830337` - `Create Excel output filter`

## Goal

Define the workbook layout and cell mapping for Excel export.

## Current Domain Context

- The canonical model requires `timestamp` as the only mandatory field
- All other fields are optional and initially passed through unchanged
- Technical metadata must stay separated from data values
- Field groups are optional
- CSV contract decisions should be mirrored where practical

## Proposed Excel Contract Baseline

### Workbook structure

- Default workbook contains one primary worksheet named `Data`
- The `Data` worksheet contains canonical record export
- Additional metadata worksheet is optional and only created when metadata exists beyond first-line inference
- Exception: when source CSV has no header row, metadata worksheet becomes mandatory

### Worksheet layout (`Data`)

- Row 1: column headers
- Row 2+: data records
- Column order follows CSV contract baseline:
  1. `timestamp`
  2. data columns in stable lexical order

### Header behavior

- Preserve canonical field names
- Keep header style minimal and deterministic
- Do not rely on localized header text

## Cell Mapping Rules

- `timestamp` is always exported
- Missing optional values are exported as empty cells
- No localized number/date conversion in the first baseline
- Multi-value fields are not split in this export step; downstream general filters may transform them first

## Metadata Handling in Excel

- Do not mix technical metadata into regular data columns in `Data`
- If extended metadata is required, write it into a dedicated worksheet (for example `Header`)
- Metadata worksheet can follow the same row semantics used for the CSV companion header file:
  1. column name
  2. optional format/type
  3. optional group name
  4. optional group description

## Formatting Scope for First Increment

- Keep formatting intentionally minimal
- Freeze top row is allowed if implemented deterministically
- Auto-filter on header row is allowed if implemented deterministically
- No advanced styling requirements in first increment

## Large Dataset Behavior

- Prefer memory-conscious writing patterns
- Keep workbook creation deterministic for large row counts
- Report partial export failures with row-level context where possible
- No fixed hard limits for first increment, but implementation should keep a path open for future async pipelining

## Finalized Decisions

- Metadata worksheet is optional, except mandatory when CSV input has no header row
- No strict standardized worksheet naming is required for the first increment
- No fixed performance/size limits are set in this increment

## Done Criteria

- Worksheet layout is defined
- Header behavior is defined
- Cell mapping is defined
- Size and formatting assumptions are recorded
- Metadata separation strategy is defined
- Large-dataset behavior is documented
- Future async-pipelining compatibility is noted
