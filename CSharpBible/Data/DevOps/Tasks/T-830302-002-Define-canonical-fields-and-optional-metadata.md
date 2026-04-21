# Task T-830302-002 - Define canonical fields and optional metadata

## Status

Draft

## Parent

- Backlog Item `BI-830331` - `Define canonical trace exchange model`

## Goal

Describe the mandatory fields, optional metadata, and extension points of the canonical exchange model.

## Current Domain Decision

- The only mandatory canonical field is the timestamp
- The model represents that some variables had some value at a given point in time
- All other fields are optional
- Optional values are initially passed through from input to output unchanged
- One canonical record represents one timestamped observation point
- Nested structures are flattened through normalized field names in the first increment
- Field groups are optional and inferred only from explicit separators (`.` preferred, `_` allowed)

## Done Criteria

- The timestamp is defined as the only mandatory canonical field
- Optional values and metadata are identified as pass-through fields
- Extension points for additional canonical fields are documented
- Record granularity is documented
- Flattening and field-group inference rules are documented
- Known ambiguities in input-derived values are listed
