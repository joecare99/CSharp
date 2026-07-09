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

## Delivered
- Extended `MarkdownPlanningReaderTests` with additional deterministic fixture scenarios for malformed headings, unknown and empty status sections, and broken cross-link parent references.
- Validated that repository-convention parsing remains stable for `Epic -> Feature -> Backlog Item -> Task`, while diagnostics remain resilient for malformed or incomplete markdown content.
- Kept the implementation provider-neutral by adding coverage only in tests without introducing provider-specific fields.

## Validation
- Run targeted planning model tests.
- `run_tests` for project `AA98_AvlnCodeStudio.Tests`: 200/200 passed, including all `MarkdownPlanningReaderTests` cases.

## Status
- Completed
