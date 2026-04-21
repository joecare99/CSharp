# Backlog Item BI-830331 - Define canonical trace exchange model

## Status

Draft

## Parent

- Epic `830302` - `TraceAnalysis and AGV-reverse simulation`
- Feature `830329` - `Filter-based trace intake and export foundation`

## Goal

Define the shared intermediate structure that all input filters produce and all output filters consume.

## Description

The canonical exchange model is the contract between source-specific import logic and target-specific export logic. It should be stable enough to support CSV and Excel export without forcing every format-specific concern into downstream processing.

At the current stage, the only mandatory canonical field is the timestamp. All other imported values remain optional and are initially transferred from input to output without semantic reshaping.

Current baseline granularity: one canonical record represents one imported observation point at a given timestamp, with zero or more optional value fields.

Nested source structures are flattened for the first increment through normalized field names. Grouping information is inferred only from explicit structural separators in field names.

Responsibility boundary for this increment:
- Input filters read and normalize available raw content into canonical records
- Output filters write the canonical content they receive
- Deciding which optional fields become canonical beyond `timestamp` remains intentionally open and outside direct import/export filter scope

## Acceptance Criteria

- The structure defines the timestamp as the only mandatory field
- All other imported values are treated as optional fields
- Metadata and payload content are separated in a documented way
- The model supports general metadata and field-specific metadata such as format and type
- The model supports field groups without making them mandatory for every record
- Field groups can remain absent for flat inputs and should only be created from clear naming structures
- Field-group inference prefers `.` as separator, allows `_`, and does not use prefix-only grouping without separators
- The model can represent records imported from multiple source formats
- Optional values can pass through from input to output unchanged in the first increment
- Mapping gaps and unresolved semantics are explicitly listed
- The model is suitable as input for both CSV and Excel export

## Dependencies

- Confirmation of the first supported source formats
- Agreement on the minimum record granularity

## Candidate Tasks

- `T-830302-002` - `Define canonical fields and optional metadata`

## Open Questions

- Which optional fields should later become canonical fields beyond the timestamp?
