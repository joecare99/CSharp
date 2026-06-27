# AA98-Bl047 GitHub Planning Adapter Baseline

## Parent
- Feature: `../Features/AA98-F44-External-DevOps-and-Repository-Providers.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`

## Value
AA98 can later exchange planning information with GitHub issues or projects through a provider adapter.

## Scope
- Plan a minimal GitHub adapter over provider-neutral contracts.
- Map local planning items to GitHub issues or project items where useful.
- Keep authentication behind abstract credential services.
- Avoid GitHub-specific assumptions in the local planning model.

## Acceptance Criteria
- GitHub integration has a bounded first adapter slice.
- Required mapping decisions are documented before coding begins.
- No token or credential persistence is introduced in planning files.

## Implementation Tasks
- `AA98-T070 Refine GitHub Planning Mapping`
- `AA98-T071 Implement GitHub Adapter Skeleton`
- `AA98-T072 Add GitHub Adapter Contract Tests`

## Assumptions
- GitHub work starts after provider-neutral contracts exist.

## Open Questions
- Should GitHub issues or GitHub Projects be the first target surface?

## Next Refinement Steps
1. Decide the first GitHub planning surface.
2. Define minimal mapping before implementation.

## Status
- Proposed
