# AA98-T086 Add Azure DevOps Adapter Contract Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl046-Azure-DevOps-Planning-Adapter-Baseline.md`

## Goal
Add repeatable tests for the Azure DevOps planning adapter skeleton.

## Scope
- Test capability reporting and mapping-boundary behavior.
- Test credential abstraction usage with fakes.
- Avoid live Azure DevOps network dependencies.

## Acceptance Criteria
- The adapter skeleton has repeatable tests without live credentials.
- Provider isolation is verified.

## Validation
- Run targeted Azure DevOps adapter tests.

## Delivered
- Added repeatable tests for capability reporting, credential resolution, adapter-target diagnostics, and neutral DI registration.
- Kept all tests offline and free of Azure DevOps SDK dependencies.

## Status
- Completed
