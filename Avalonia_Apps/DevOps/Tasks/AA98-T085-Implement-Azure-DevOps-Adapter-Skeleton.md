# AA98-T085 Implement Azure DevOps Adapter Skeleton

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl046-Azure-DevOps-Planning-Adapter-Baseline.md`

## Goal
Implement a minimal Azure DevOps adapter skeleton over provider-neutral planning contracts.

## Scope
- Implement adapter shape, capability reporting, and the mapping boundary.
- Resolve credentials only through the abstract credential service.
- Keep synchronization non-networked and diagnostic until a concrete integration slice is approved.

## Acceptance Criteria
- The Azure DevOps adapter composes behind neutral contracts.
- Provider-specific dependencies remain isolated.
- No token or credential persistence is introduced in planning files.

## Validation
- Build changed projects.
- Run adapter tests from `AA98-T086`.

## Delivered
- Created `AA98_AvlnCodeStudio.Planning.AzureDevOps` with only Planning.Core and dependency-injection abstractions as dependencies.
- Added an adapter descriptor with import/export capabilities and a diagnostic skeleton synchronization path.
- Resolved credentials only through `IPlanningCredentialService`.

## Status
- Completed
