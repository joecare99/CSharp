# AA98-T059 Define Local Planning Model Contracts

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md`

## Goal
Define provider-neutral contracts for local markdown planning items.

## Scope
- Represent epics, features, backlog items, and tasks.
- Represent stable IDs, titles, source paths, parent links, status, and diagnostics.
- Keep provider-specific fields out of the first local model.

## Execution Notes
1. Start from `DevOps/.info.md` hierarchy rules.
2. Design for later UI and AI/tool usage.
3. Keep runtime state separate from source planning files.

## Acceptance Criteria
- Local planning items can be modeled without Azure DevOps or GitHub dependencies.
- Diagnostics can represent duplicate IDs and missing links.

## Delivered
- Added provider-neutral local planning contracts in `AA98_AvlnCodeStudio.Base` for planning items, item kinds, item statuses, parent or child links, diagnostics, and diagnostic severities.
- Kept the first local planning model focused on stable IDs, titles, source paths, parent links, statuses, and diagnostics without introducing provider-specific fields or reader behavior.
- Added deterministic engineering model tests that pin the default values of the new planning contracts for later markdown-reader and UI work.

## Validation
- Build changed projects.
- Add tests for normalization or parsing helpers in later tasks.
- `dotnet test AA98_AvlnCodeStudio/AA98_AvlnCodeStudio.Tests/AA98_AvlnCodeStudio.Tests.csproj --no-restore --filter "FullyQualifiedName~EngineeringFoundationModelTests"`

## Status
- Completed
