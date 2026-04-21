# Task T-830895 - Implement Excel output filter from canonical exchange records

## Status

Draft

## Parent

- Backlog Item `BI-830337` - `Create Excel output filter`

## Goal

Implement the first production-ready Excel output filter that exports canonical exchange records into a deterministic workbook layout.

## Scope

- Implement workbook creation with a primary `Data` worksheet
- Implement deterministic column projection and ordering aligned with CSV baseline:
  1. `timestamp`
  2. remaining fields in stable lexical order
- Implement row mapping from canonical exchange records to worksheet cells
- Implement optional metadata worksheet creation when metadata exists
- Implement mandatory metadata worksheet creation when source CSV had no header row
- Keep technical metadata out of data columns
- Keep first increment formatting minimal and deterministic

## Out of Scope

- Advanced worksheet styling beyond deterministic baseline behavior
- Domain-specific derived columns or value transformations
- Localization-specific number/date formatting
- Async execution pipeline implementation (compatibility only)

## Implementation Notes

- Reuse existing output-filter abstractions and dependency injection patterns in `TraceAnalysis`
- Keep one class/interface/struct per file when adding or changing implementation artifacts
- Preserve nullability rules and avoid implicit null assumptions
- Keep worksheet naming flexible in this increment (no strict normalization requirement)
- Ensure the implementation is deterministic for equivalent inputs

## Data Mapping Baseline

- Header row is row 1 in worksheet `Data`
- Data starts at row 2
- Missing optional values are written as empty cells
- `timestamp` is always present in output
- Do not split multi-value fields in this step; write canonical values as provided

## Metadata Worksheet Behavior

Create a dedicated metadata worksheet using CSV companion-header-compatible row semantics:
1. column name
2. optional format/type
3. optional group name
4. optional group description

Rule:
- Optional when metadata exists
- Mandatory when source CSV had no header row

## Large Dataset and Reliability Considerations

- Use memory-conscious writing patterns
- Keep behavior stable for large exports
- Surface export failures with meaningful row/record context where available
- Keep code structure compatible with future async pipelining

## Test Strategy

- Add unit tests for:
  - worksheet existence and structure
  - deterministic column order
  - empty-value mapping
  - metadata worksheet optional/mandatory behavior
- Add integration-style export test for a representative canonical dataset
- Use `MSTest` and `NSubstitute` where mocking is required

## Dependencies

- `BI-830331` - `Define canonical trace exchange model`
- `T-830395` - `Specify Excel workbook layout and export behavior`

## Done Criteria

- Excel output filter implementation is available through the output-filter layer
- Workbook output for canonical records is generated as defined
- Metadata is separated from data columns
- Optional/mandatory metadata worksheet rule is implemented
- Deterministic column ordering is implemented
- Relevant tests pass for workbook layout and mapping behavior
- Implementation notes future async-pipelining compatibility
