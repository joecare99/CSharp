# AA98-T065 Define Provider Neutral Planning Adapter Contracts

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl045-Provider-Neutral-Planning-Adapter-Contracts.md`

## Goal
Define contracts for external planning provider adapters without provider-specific dependencies.

## Scope
- Represent provider capabilities and mapping boundaries.
- Separate local planning item identity from provider identity.
- Define credential access through abstract services only.

## Execution Notes
1. Build on the local planning model.
2. Avoid Azure DevOps or GitHub SDK references in neutral contracts.
3. Keep synchronization mode explicit but not overdesigned.

## Acceptance Criteria
- Provider-neutral adapter contracts can support later Azure DevOps and GitHub adapters.
- Credential handling is abstracted.

## Validation
- Build changed projects.
- Add contract tests in `AA98-T066`.

## Status
- Proposed
