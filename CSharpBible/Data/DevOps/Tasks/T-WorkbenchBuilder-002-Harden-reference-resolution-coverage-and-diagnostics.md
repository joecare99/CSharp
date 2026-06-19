# Task T-WorkbenchBuilder-002 - Harden reference resolution coverage and diagnostics

## Status

Draft

## Parent

- Backlog Item `BI-WorkbenchBuilder-001` - `Complete V1.1 inspection pipeline`

## Goal

Strengthen confidence in reference resolution behavior and its resulting diagnostics for supported sample projects.

## Scope

- Expand tests around framework, project, metadata, package, and analyzer references where practical
- Verify diagnostics for missing or unresolved references
- Refine severity and categorization rules if current output is too coarse

## Acceptance Criteria

- High-value reference categories have direct regression coverage
- Missing-reference situations are visible and testable
- The current resolver behavior is documented where limitations remain

## Dependencies

- `ReferenceResolverTests`
- `ProjectInspectionServiceTests`
- Sample test projects and restore helper infrastructure
