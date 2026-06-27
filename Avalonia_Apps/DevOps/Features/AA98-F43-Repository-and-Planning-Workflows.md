# AA98-F43 Repository and Planning Workflows

## Parent
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`

## Goal
Introduce local repository and DevOps planning workflows that support AA98 self-hosting before external provider synchronization is added.

## Scope
- Add local Git-oriented repository workflow planning.
- Add local DevOps planning hierarchy browsing and validation.
- Keep planning files as repository source artifacts.
- Prepare planning and repository actions for later AI/tool invocation.

## User or Process Value
- A developer can use AA98 to navigate implementation planning and repository state.
- Self-hosting work can be driven from local planning artifacts.
- External provider integration is delayed until the local model is stable.

## Candidate Backlog Items
- `AA98-Bl042 Local Repository Workflow Baseline`
- `AA98-Bl043 DevOps Planning Local Model Baseline`
- `AA98-Bl044 DevOps Planning UI Baseline`

## Assumptions
- The local markdown planning structure is the first source of truth for planning.
- Repository workflows start with local Git context before remote provider workflows.
- Runtime or provider-owned state is not stored as planning state under `DevOps`.

## Open Questions
- Is local repository inspection enough for the first self-hosting milestone, or is commit support required?
- Should the planning component first optimize reading/navigation or editing/validation?
- Which local planning validation rules should block a self-hosting milestone?

## Next Refinement Steps
1. Create a local repository baseline implementation task.
2. Create local planning model and planning UI tasks.
3. Add dedicated validation tasks for planning convention checks and repository workflow tests.

## Status
- Proposed
