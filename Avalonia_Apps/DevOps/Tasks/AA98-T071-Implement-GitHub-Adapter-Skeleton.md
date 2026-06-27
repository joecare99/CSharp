# AA98-T071 Implement GitHub Adapter Skeleton

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl047-GitHub-Planning-Adapter-Baseline.md`

## Goal
Implement a minimal GitHub planning adapter skeleton over provider-neutral planning contracts.

## Scope
- Implement adapter shape and capability reporting.
- Wire authentication through abstractions only.
- Keep real synchronization minimal or stubbed according to the refined mapping.

## Execution Notes
1. Do not leak GitHub SDK types into neutral contracts.
2. Keep provider-specific behavior in the adapter layer.
3. Avoid storing runtime provider state in `DevOps` planning files.

## Acceptance Criteria
- GitHub adapter skeleton composes behind neutral contracts.
- Provider-specific dependencies are isolated.

## Validation
- Build changed projects.
- Run adapter tests from `AA98-T072`.

## Status
- Proposed
