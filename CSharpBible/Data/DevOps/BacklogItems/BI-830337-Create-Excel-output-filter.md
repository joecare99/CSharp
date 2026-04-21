# Backlog Item BI-830337 - Create Excel output filter

## Status

Draft

## Parent

- Epic `830302` - `TraceAnalysis and AGV-reverse simulation`
- Feature `830329` - `Filter-based trace intake and export foundation`

## Goal

Export canonical exchange records to Excel in a structure suitable for inspection and stakeholder review.

## Description

The Excel output filter should create a workbook representation of the canonical exchange records while keeping the export contract aligned with the CSV export where practical.

First baseline:

- Primary worksheet `Data` with row 1 as header and row 2+ as values
- Column order aligned with CSV baseline (`timestamp` first, then stable lexical order)
- Technical metadata stays separate from data columns
- Metadata worksheet is optional when metadata exists, but mandatory when CSV input has no header row
- No strict worksheet-name standardization is required in the first increment

The metadata worksheet can follow the same semantics as the CSV companion header file:

1. column name
2. optional format/type
3. optional group name
4. optional group description

## Acceptance Criteria

- Workbook and worksheet structure are defined
- Header formatting rules are defined
- Cell data mapping from canonical records is documented
- Large dataset assumptions are recorded
- Exported Excel output can be produced directly from canonical exchange records
- Metadata handling is documented without mixing technical metadata into data columns
- Metadata worksheet optional/mandatory rule is documented
- Future async-pipelining compatibility is noted

## Dependencies

- `BI-830331` - `Define canonical trace exchange model`

## Candidate Tasks

- `T-830395` - `Specify Excel workbook layout and export behavior`
- `T-830895` - `Implement Excel output filter from canonical exchange records`

## Open Questions

- No blocking open questions for the first increment
