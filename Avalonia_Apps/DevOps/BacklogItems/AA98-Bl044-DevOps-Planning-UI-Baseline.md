# AA98-Bl044 DevOps Planning UI Baseline

## Parent
- Feature: `../Features/AA98-F43-Repository-and-Planning-Workflows.md`
- Feature: `../Features/AA98-F39-Component-Micro-Hosts.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`

## Value
A developer can browse and inspect local planning items inside a dedicated planning component or micro host.

## Scope
- Provide a first planning explorer UI over the local planning model.
- Support item selection and basic detail display.
- Prepare editing and validation surfaces without requiring external providers.
- Create a thin planning micro host for isolated development.

## Acceptance Criteria
- The planning UI can show local epics, features, backlog items, and tasks.
- A selected planning item can display its key metadata and source path.
- A planning micro host can run without the full AA98 shell.

## Implementation Tasks
- `AA98-T062 Implement Planning Explorer ViewModel`
- `AA98-T063 Create DevOps Planning Micro Host`
- `AA98-T064 Add Planning UI Tests`

## Assumptions
- Editing can be introduced after read/navigation behavior is stable.

## Open Questions
- Should the first UI include markdown preview, source editing, or metadata-only detail display?

## Next Refinement Steps
1. Build on the local planning model baseline.
2. Add editing backlog items after browsing and validation work.

## Progress Notes
- A first planning explorer view model is now implemented in `AA98_AvlnCodeStudio.UI`, exposing a deterministic hierarchy for local epics, features, backlog items, and tasks.
- Selection now exposes key metadata fields (ID, title, status, parent, source path) plus diagnostics from local planning reads.
- Startup composition now provides the planning explorer in the main shell view model, and deterministic tests cover hierarchy and selection behavior.
- A dedicated thin micro host `AA98.DevOpsPlanning.Host` now exists for isolated planning exploration, loading local planning hierarchy and diagnostics without external provider coupling.

## Status
- In Progress
