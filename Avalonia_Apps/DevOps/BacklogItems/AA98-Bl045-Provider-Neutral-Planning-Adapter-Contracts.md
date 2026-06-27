# AA98-Bl045 Provider-Neutral Planning Adapter Contracts

## Parent
- Feature: `../Features/AA98-F44-External-DevOps-and-Repository-Providers.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`

## Value
External planning providers can later be added without reshaping the local planning model.

## Scope
- Define provider-neutral planning adapter contracts.
- Separate local planning identity from provider identity.
- Define import/export or synchronization boundaries without choosing a provider first.
- Keep credential handling outside planning entities.

## Acceptance Criteria
- Adapter contracts can represent provider capabilities without Azure DevOps or GitHub dependencies.
- Provider IDs and local IDs remain clearly separated.
- Credential access is represented only through abstract services.

## Implementation Tasks
- `AA98-T065 Define Provider Neutral Planning Adapter Contracts`
- `AA98-T066 Add Planning Adapter Contract Tests`

## Assumptions
- Provider adapters are not required for minimal Linux self-hosting.

## Open Questions
- Should initial synchronization be pull-only, push-only, or import/export?

## Next Refinement Steps
1. Implement neutral contracts before provider-specific adapters.
2. Decide provider order after local planning validation is complete.

## Status
- Proposed
