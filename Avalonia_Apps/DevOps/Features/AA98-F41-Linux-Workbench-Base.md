# AA98-F41 Linux Workbench Base

## Parent
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`

## Goal
Make the AA98 workbench shell, editor baseline, and workspace opening path reliable enough on Linux to form the first self-hosting foundation.

## Scope
- Validate Linux startup of the main AA98 shell.
- Identify and remove Windows-only assumptions from the initial workbench path.
- Stabilize file open, edit, and save workflows for self-hosting source files.
- Ensure the workbench can load command, settings, and component contribution baselines on Linux.

## User or Process Value
- A developer can start AA98 on Linux and use it for basic source inspection and editing.
- Linux defects are discovered early in the workbench base instead of after higher-level components are built.
- The first self-hosting milestone has a concrete start condition.

## Candidate Backlog Items
- `AA98-Bl038 Linux Shell Startup Baseline`
- `AA98-Bl039 Linux Editor and Workspace Baseline`

## Assumptions
- Avalonia remains the shell UI technology.
- The first target is development usability, not polished distribution packaging.
- Existing shell/editor features should be stabilized before new Linux-only features are introduced.

## Open Questions
- Which Linux distribution is the first validation baseline?
- Which file formats are mandatory for the first Linux editing milestone?
- Which startup diagnostics should be visible to the developer?

## Next Refinement Steps
1. Implement the shell startup baseline before adding more self-hosting components.
2. Add targeted test and smoke validation tasks for Linux startup and editor workflows.
3. Feed discovered platform assumptions back into architecture guardrail planning.

## Progress Notes
- Shell startup inspection is complete. The first concrete AA98 shell path is `AA98_AvlnCodeStudio.UI` with Avalonia desktop lifetime startup and composition in `App.axaml.cs`.
- Phase 1 blockers are now better scoped: shell startup mainly needs diagnostics and validation seams, while Linux path normalization issues continue in the editor/workspace baseline.
- Shell startup fixes are now in place: desktop initialization has an explicit composition seam, DI validation is enabled, and startup errors surface with focused diagnostics.
- Linux shell startup now has a repeatable validation path: automated startup guardrail tests cover the composition seam, and the remaining full-UI Linux smoke path is documented in `../Validation/AA98-Linux-Shell-Startup-Smoke-Checklist.md`.
- The editor/workspace baseline is now inspected as well: the first concrete Linux risks are the `MyDocuments` fallback in `AA98_AvlnCodeStudio.Editor/Services/EditorWorkflow.cs`, direct dialog initial-directory forwarding, and Windows-only sample paths in current tests.
- The first editor/workspace Linux fixes are now in place: unsaved documents seed dialogs from a platform-neutral working directory strategy, and automated tests cover Linux-style source and planning file paths.
- The Linux editor/workspace baseline now has repeatable validation too: automated tests cover representative `.cs`, `.axaml`, and `.md` flows, and the remaining full-shell workflow is documented in `../Validation/AA98-Linux-Editor-Workspace-Smoke-Checklist.md`.
- Feature remains active until backlog items `AA98-Bl038` and `AA98-Bl039` are fully validated.

## Status
- In Progress
