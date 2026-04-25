# Task T-831181 - Implement JSON trace cache filter

## Status

Draft

## Parent

- Feature `830329` - `Filter-based trace intake and export foundation`

## Goal

Provide a JSON-based input and output filter pair for the canonical trace exchange model so trace data can be reloaded deterministically and used as an intermediate cache format.

## Scope

- Add a dedicated JSON filter project within `TraceAnalysis`
- Implement deterministic JSON export from canonical trace datasets
- Implement JSON import back into the canonical trace dataset model
- Support source metadata, field metadata, records, and parse errors in the serialized payload
- Keep the JSON format suitable for intermediate storage between import and export steps
- Integrate the JSON input filter into the existing analyzable filter selection model
- Add automated tests for JSON roundtrip behavior and selection heuristics

## Out of Scope

- Compression or chunked persistence for very large files
- Schema version negotiation beyond a first explicit format marker
- UI-specific format selection workflows
- Domain-specific data transformation during JSON read or write

## Implementation Notes

- Reuse `IAnalyzableInputFilter` and `IOutputFilter` contracts
- Prefer a deterministic JSON structure with stable property names and field ordering
- Treat JSON as a technical exchange and cache format, not as a UI-facing report format
- Keep compatibility with the repository multi-targeting setup, including older .NET Framework targets
- Preserve nullability and partial parse error reporting behavior

## Proposed Payload Shape

- Root object contains:
  - `format`
  - `sourceId`
  - `fields`
  - `records`
  - `parseErrors`
- Each field contains:
  - `name`
  - `type`
  - `group`
  - `format`
- Each record contains:
  - `timestamp`
  - `values`

## Test Strategy

- Add unit tests for JSON export payload content
- Add unit tests for JSON import of a representative payload
- Add roundtrip tests for metadata, optional values, and parse errors
- Add input-filter analysis tests for `.json` sources and invalid payloads

## Assumptions

- JSON should be human-readable and deterministic enough for cache reuse and diagnostics
- Canonical field metadata is sufficient for first-increment JSON reconstruction
- Timestamp values may be restored using JSON value kinds plus declared metadata without requiring domain-specific converters

## Open Questions

- Should future versions preserve original CLR type names exactly or normalize to a smaller supported set?
- Is a dedicated schema version property required once external tools start producing the JSON payload?

## Done Criteria

- JSON filter implementation is available in the workspace
- Canonical datasets can be exported to JSON and re-imported
- JSON can be selected as input by extension and content analysis
- Relevant tests pass for roundtrip and detection behavior
- Build validation completes for the modified solution scope
