# AA98-F44 External DevOps and Repository Providers

## Parent
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`

## Goal
Prepare external Azure DevOps and GitHub provider adapters without letting provider-specific behavior reshape the local planning and repository core.

## Scope
- Define provider-neutral planning and repository synchronization boundaries.
- Plan Azure DevOps adapter work after the local planning model is stable.
- Plan GitHub adapter work after the local planning model is stable.
- Keep credential and token handling behind abstractions.

## User or Process Value
- AA98 can later work with external planning and repository providers without losing offline-first local planning.
- Provider work can be scheduled after the self-hosting core is useful.
- Future synchronization is visible to planning agents without prematurely committing implementation details.

## Candidate Backlog Items
- `AA98-Bl045 Provider-Neutral Planning Adapter Contracts`
- `AA98-Bl046 Azure DevOps Planning Adapter Baseline`
- `AA98-Bl047 GitHub Planning Adapter Baseline`

## Assumptions
- Provider adapters are not required for the first minimal Linux self-hosting milestone.
- Azure DevOps and GitHub integrations should share neutral contracts where practical.
- Credentials are managed by dedicated services, not by planning model entities.

## Open Questions
- Should Azure DevOps or GitHub be integrated first?
- Should the first adapter mode be import/export or connected synchronization?
- Which work-item fields must the neutral model support before provider adapters begin?

## Next Refinement Steps
1. Define provider-neutral planning adapter contracts.
2. Decide first provider adapter order after local planning validation exists.
3. Pair each provider implementation slice with dedicated contract and integration test tasks.

## Status
- Proposed
