# Task T-830302-004 - Specify CSV column mapping and export behavior

## Status

Draft

## Parent

- Backlog Item `BI-830334` - `Create CSV output filter`

## Goal

Define the CSV projection of canonical exchange records.

## Current Domain Context

- The canonical model requires `timestamp` as the only mandatory field
- All other values are optional and initially passed through from input to output
- Technical metadata must remain clearly separated from actual data values
- Field groups are optional and may be absent for flat CSV input

## Decisions from Current Refinement

1. Delimiter strategy:
   - Automatic detection between `;` and `<tab>`
   - Optional user-defined delimiter override
2. Detection approach:
   - Probe the first lines
   - Apply rules from general international assumptions toward country-specific assumptions
3. Multi-value handling:
   - No split logic in the direct import filter
   - Raw format is read first
   - Multi-value split into separate columns is handled later by a downstream general filter
4. Metadata handling for CSV:
   - Flat CSV does not contain dedicated metadata, except what can be inferred from the first lines
   - Extended per-column metadata is provided via a separate header file
   - Export may also write supplemental per-column metadata into a header file

## Proposed CSV Contract Baseline

### Column families

The CSV output should use explicit column families:
- `timestamp` column (always present)
- Data value columns (dynamic, based on available canonical fields)

Technical metadata should not be mixed into regular data columns. Metadata that cannot be inferred from first lines should be handled through a companion header file.

### Deterministic column order

1. `timestamp`
2. Data value columns in stable lexical order

### Header naming

- Preserve canonical field names for data columns
- Keep group-related output optional and avoid forcing group columns when groups are absent
- If a companion header file is present, it follows the same column order and separator as the data CSV

## Value Formatting Rules

- `timestamp` is exported in invariant ISO 8601 UTC format: `yyyy-MM-ddTHH:mm:ss.fffZ`
- Empty optional values must be exported as empty cells
- No localized formatting for numbers or dates in the first contract draft
- Multi-value fields are not split in direct import; downstream general filters may transform them later

## CSV Escaping and Delimiter Rules

- Try delimiter auto-detection using first-line probing
- Supported auto-detection baseline: `;` and `<tab>`
- Allow explicit user override of delimiter
- Values containing delimiter, quotes, or line breaks must be escaped consistently
- Quote escaping must follow one deterministic rule
- Header and value serialization must use the same escaping behavior

## Output Filter Selection Responsibility

- Output format selection is handled by the output-filter layer
- Manual output-filter selection by the user has priority
- If no manual selection exists, choose by file extension using a fixed entropy baseline without additional analysis
- For `.csv`, the default expected output is flat CSV
- Specialized CSV variants are only selected manually or through explicit non-generic extension mapping

## Metadata Companion Header File

The companion header file uses the same delimiter as the data CSV and the same column order.

Row schema:
1. Column names
   - Allowed characters: alphanumeric plus `.`, `-`, `_`
   - Spaces are normalized to `_`
   - Delimiter characters are not allowed in column names
2. Optional per-column format or type
3. Group name
   - Empty value means no group (single column without grouping)
   - A non-empty identifier means the column belongs to that group, even if the group currently contains only one column
4. Optional group description
   - Used when row 3 contains a group name
   - Can continue in following rows when needed

Multiline handling rules:
- Row 4 is the first description line
- Row 5+ are continuation lines in original order
- Continuation ends at end-of-file
- Empty cells in continuation rows are treated as "no additional text" for that column
- Export writes one description line per physical row, preserving line order

## Large-File Behavior

- Export should be stream-friendly and avoid full in-memory materialization where possible
- Export failures should report row/record position where available
- Partial export outcomes should be detectable and logged

## Open Decisions for Finalization

- No blocking open decisions for this task

## Done Criteria

- Column order is defined
- Header names are defined
- Escaping behavior is defined
- Missing value handling is defined
- Metadata separation from data columns is documented
- Companion header file behavior for import/export is documented
- Large-file and partial-result behavior is documented
- Delimiter auto-detection and user override behavior are documented
