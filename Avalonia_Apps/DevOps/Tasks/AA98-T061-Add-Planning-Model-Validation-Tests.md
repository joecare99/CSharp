# AA98-T061 Add Planning Model Validation Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md`

## Goal
Add tests for local planning model parsing and convention validation.

## Scope
- Test valid epic, feature, backlog item, and task parsing.
- Test duplicate ID and missing parent diagnostics.
- Test broken or absent cross-link handling.

## Execution Notes
1. Use small markdown fixtures where possible.
2. Include current repository conventions as examples.
3. Keep provider-specific fields out of tests.

## Acceptance Criteria
- Planning model reader behavior is covered by targeted tests.
- Validation diagnostics are deterministic.

## Validation
- Run targeted planning model tests.

## Status
- Proposed
