# AA98-T068 Implement Azure DevOps Adapter Skeleton

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl046-Azure-DevOps-Planning-Adapter-Baseline.md`

## Goal
Implement a minimal Azure DevOps adapter skeleton over provider-neutral planning contracts.

## Scope
- Implement adapter shape and capability reporting.
- Wire credential access through abstractions only.
- Keep real synchronization minimal or stubbed according to the refined mapping.

## Execution Notes
1. Do not leak Azure DevOps SDK types into neutral contracts.
2. Keep provider-specific behavior in the adapter layer.
3. Avoid storing runtime provider state in `DevOps` planning files.

## Acceptance Criteria
- Azure DevOps adapter skeleton composes behind neutral contracts.
- Provider-specific dependencies are isolated.

## Validation
- Build changed projects.
- Run adapter tests from `AA98-T069`.

## Status
- Proposed
