# Task T-WorkbenchBuilder-005 - Add coverage for reference categories and missing-input diagnostics

## Status

Draft

## Parent

- Backlog Item `BI-WorkbenchBuilder-003` - `Expand builder regression coverage`

## Goal

Increase test confidence for reference classification and diagnostic emission in edge or failure-oriented inspection scenarios.

## Scope

- Add test scenarios that exercise missing compile items or missing project references
- Validate that diagnostic codes and severities remain observable
- Cover additional reference categories where the current test set is too shallow

## Acceptance Criteria

- New tests protect important diagnostic and classification behavior
- The most visible V1.1 warning paths are covered
- The test intent remains readable through focused sample cases

## Dependencies

- `ProjectInspectionServiceTests`
- `ReferenceResolverTests`
- Current test-data project set or targeted additions to it
