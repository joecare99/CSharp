# AA98-Bl050 First Copilot Assisted Self-Hosting Workflow

## Parent
- Feature: `../Features/AA98-F40-Copilot-Assisted-Workflow.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
A developer gets a first narrow, explicit AI-assisted workflow that helps with AA98 self-hosting without bypassing consent or provider boundaries.

## Scope
- Select one narrow workflow with clear user value.
- Use tool-capable commands rather than provider-specific shortcuts.
- Require explicit context selection and consent.
- Produce traceable inputs and outputs for diagnostics.

## Acceptance Criteria
- The first workflow can run with a provider-neutral AI boundary.
- Context sharing is explicit and minimal.
- The workflow is narrow enough to validate safely before broader automation.

## Implementation Tasks
- `AA98-T079 Select First Copilot Assisted Workflow`
- `AA98-T080 Implement First AI Tool Workflow Skeleton`
- `AA98-T081 Add AI Workflow Consent and Boundary Tests`

## Assumptions
- Tool-capable command contracts exist before this workflow is implemented.

## Open Questions
- Should the first workflow target planning refinement, build diagnostics, or editor selection assistance?

## Next Refinement Steps
1. Choose the first workflow after command metadata is stable.
2. Keep the first provider path replaceable.

## Status
- Proposed
