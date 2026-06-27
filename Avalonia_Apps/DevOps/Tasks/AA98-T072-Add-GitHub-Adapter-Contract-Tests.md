# AA98-T072 Add GitHub Adapter Contract Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl047-GitHub-Planning-Adapter-Baseline.md`

## Goal
Add tests for the GitHub planning adapter skeleton.

## Scope
- Test capability reporting and mapping behavior.
- Test credential abstraction usage with fakes.
- Avoid live GitHub network dependency in unit tests.

## Execution Notes
1. Use fake provider responses or adapter seams.
2. Separate live integration validation into later tasks if needed.

## Acceptance Criteria
- Adapter skeleton has repeatable tests without live credentials.
- Provider isolation is verified.

## Validation
- Run targeted GitHub adapter tests.

## Status
- Proposed
