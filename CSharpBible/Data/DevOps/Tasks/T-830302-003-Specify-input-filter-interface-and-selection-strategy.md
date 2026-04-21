# Task T-830353 - Specify input filter interface and selection strategy

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Define how input filters are discovered, selected, and invoked.

## Current Domain Decision

- The interface should prefer stream-based input plus an additional source identifier instead of a hard file-only contract
- Filter selection should first use the source extension, then inspect the file header or content signature
- Later, the selection should be overridable through a manually chosen prefiltered list
- Filters should be registered through interface-based Dependency Injection
- The base contract should support general metadata, field-specific metadata such as format and type, and field groups
- Field groups are optional and should only be derived when field names show clear structural prefixes
- Flat CSV input may not contain any field groups at all
- Technical metadata must stay clearly separated from the actual data values
- Processing should support large files and return partial results together with a logged error list

## Proposed Contract Baseline

### Input source descriptor

The source descriptor should be passed together with the input stream and provide detection hints without forcing file-system usage.

Required baseline hints:
- Source identifier
- Suggested extension
- Optional content type hint
- Optional display name

### Input filter responsibilities

Each input filter should:
- Decide whether it can process the source based on descriptor and optional header probe
- Parse records from stream input into canonical exchange records
- Emit technical metadata separately from data values
- Emit field-level metadata (`type`, `format`, optional units)
- Optionally emit field groups
- Report parse issues without losing successful records

### Result shape expectations

The filter result should contain:
- Parsed records
- General metadata
- Field metadata
- Optional field groups
- Logged parse errors
- Partial-result indicator

## Selection Strategy

1. Build candidate filter set from DI registration
2. Prefilter by source extension where available
3. Validate candidates by file inspection (`header + n lines`) and/or an `n KB` format-analysis block returned by each filter
4. If one filter matches, select it
5. If multiple filters match, apply deterministic ranking and log ambiguity
6. If no filter matches, return unsupported-format outcome
7. Future: allow manual override from the prefiltered candidate set

## Deterministic Ranking Rules

When multiple filters match the same source, ranking should be resolved in this order:

1. Manual override selection (when provided)
2. Highest confidence from format inspection result (`header + n lines` decision lines or `n KB` analysis block)
3. Exact extension match
4. Configured filter priority
5. Stable tie-breaker by filter identifier

## Field Group Inference Rules

- Field groups are optional and can be absent
- Prefer `.` as the structural separator for group inference
- Also allow `_` as secondary separator
- Do not infer groups from prefix-only naming without a separator
- Examples:
  - `AGV1.X`, `AGV1.Y`
  - `Diag1.Axis1.Speed`, `Diag1.Axis2.Speed`

## Failure and Partial-Result Behavior

- Continue parsing where feasible for recoverable row-level issues
- Collect errors in a structured error list
- Return partial results when at least one record was parsed successfully
- Separate fatal format errors from row-level conversion errors
- Ensure logged issues include source position references where available

## Project Structure Direction

- Introduce one or more base interface projects for shared filter contracts
- Keep input and output filter implementations in separate projects
- Combine multiple input filters into one project only when parsing rules are genuinely close
- `CSV` and `TraceCsv2realCsv` CSV may start together but can be split if rule divergence increases

## Done Criteria

- Input filter responsibilities are documented
- The stream-based input contract and source descriptor are documented
- Selection strategy is documented, including extension, header detection, and later manual override
- Failure behavior is documented, including partial results and logged error lists
- Metadata handling is documented for general metadata, field-specific metadata, and field groups
- Optional field-group detection rules are documented with naming examples and separator precedence (`.` before `_`)
- Extension path for new formats through interface plus DI is documented
- Project-structure guidance is documented for base interfaces and input/output filter separation
