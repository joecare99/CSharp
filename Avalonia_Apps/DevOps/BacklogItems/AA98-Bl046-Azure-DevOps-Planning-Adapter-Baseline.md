# AA98-Bl046 Azure DevOps Planning Adapter Baseline

## Parent
- Feature: `../Features/AA98-F44-External-DevOps-and-Repository-Providers.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`

## Value
AA98 can later exchange planning information with Azure DevOps through a provider adapter.

## Scope
- Plan a minimal Azure DevOps adapter over provider-neutral contracts.
- Map local planning item types to Azure DevOps work item concepts.
- Keep credentials and tokens behind abstract credential services.
- Avoid implementing full synchronization before import/export rules are chosen.

## Acceptance Criteria
- Azure DevOps integration has a bounded first adapter slice.
- Required field mappings are documented before coding begins.
- No token or credential persistence is introduced in planning files.

## Implementation Tasks
- `AA98-T084 Refine Azure DevOps Field Mapping`
- `AA98-T085 Implement Azure DevOps Adapter Skeleton`
- `AA98-T086 Add Azure DevOps Adapter Contract Tests`

## Assumptions
- Azure DevOps work starts after provider-neutral contracts exist.

## Open Questions
- Which process template should be used for the first mapping assumption?

## Next Refinement Steps
1. Choose a concrete Azure DevOps process template and state mapping.
2. Implement live import or export only after connection consent and error-handling behavior are defined.

## Status
- Completed
