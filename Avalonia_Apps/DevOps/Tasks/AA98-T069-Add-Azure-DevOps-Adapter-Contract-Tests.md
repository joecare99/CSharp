# AA98-T069 Add Azure DevOps Adapter Contract Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl046-Azure-DevOps-Planning-Adapter-Baseline.md`

## Goal
Add tests for the Azure DevOps planning adapter skeleton.

## Scope
- Test capability reporting and mapping behavior.
- Test credential abstraction usage with fakes.
- Avoid live Azure DevOps network dependency in unit tests.

## Execution Notes
1. Use fake provider responses or adapter seams.
2. Separate live integration validation into later tasks if needed.

## Acceptance Criteria
- Adapter skeleton has repeatable tests without live credentials.
- Provider isolation is verified.

## Validation
- Run targeted Azure DevOps adapter tests.

## Status
- Proposed
