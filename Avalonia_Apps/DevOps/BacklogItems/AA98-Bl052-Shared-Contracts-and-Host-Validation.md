# AA98-Bl052 Shared Contracts and Host Validation

## Parent
- Feature: `../Features/AA98-F39-Component-Micro-Hosts.md`
- Feature: `../Features/AA98-F40-Copilot-Assisted-Workflow.md`
- Feature: `../Features/AA98-F43-Repository-and-Planning-Workflows.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`

## Value
Create a thin but validated AA98 self-hosting slice that proves the component architecture works end to end for local planning, repository context, and a first AI-assisted workflow.

## Scope
- Complete the shared component-contract layer so planning, shell, and future component hosts speak the same neutral vocabulary.
- Use that shared layer in the DevOps planning micro host and the first repository/planning workflow surface.
- Add one explicit, consent-aware tool workflow that can be exercised without the full workbench.
- Add smoke validation for Linux and the new host path.

## Acceptance Criteria
- Shared component contracts are reused by at least the planning workflow and one other host-facing component.
- The DevOps planning micro host can load local planning content, expose diagnostics, and remain usable as a standalone host.
- A first tool-capable workflow can be invoked through deterministic contracts and is visible as a narrow, testable slice.
- A Linux-oriented smoke checklist exists for the new host/workflow path and can be executed with minimal manual setup.

## Implementation Tasks
- `AA98-T063 Create DevOps Planning Micro Host`
- `AA98-T064 Add Planning UI Tests`
- `AA98-T076 Define Tool-Capable Command Metadata`
- `AA98-T077 Implement Tool-Command-Descriptor Contracts`
- `AA98-T079 Select First Copilot-Assisted Workflow`
- `AA98-T080 Implement First AI Tool Workflow Skeleton`
- `AA98-T073 Define Micro-Host Project Pattern`
- `AA98-T074 Create Shell and Editor Micro-Hosts`

## Assumptions
- The local markdown planning model remains the first source of truth.
- Repository and planning workflows should stay provider-neutral until the local slice is stable.
- The first assistant workflow should be narrow and explicit rather than broad and opaque.

## Open Questions
- Which host should be the primary proof point for the next iteration: planning, builder, or terminal?
- Which validation scenario should be considered the minimum Linux self-hosting milestone?
- How much consent and disclosure detail should be surfaced in the first workflow slice?

## Next Refinement Steps
1. Pick the first host/workflow pair to implement as the next milestone.
2. Lock the shared component-contract slice to the smallest set of reusable abstractions.
3. Add a dedicated test task for the selected host before implementation starts.

## Status
- Proposed
