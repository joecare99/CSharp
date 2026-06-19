# AA98-Bl036 Add Builder V1.1 Formatter and Thin Host

## Parent
- Feature: `DevOps/Features/AA98-F38-Roslyn-Builder-V1-Inspection-and-Compilation-Baseline.md`
- Epic: `DevOps/Epics/AA98-E10-Workbench-Builder-and-Roslyn-Execution.md`
- Vision: `DevOps/Vision.md`

## Scope
Add the first reusable formatter for `ProjectInspectionResult` plus a dedicated thin console host that can inspect a project and emit either plain-text or JSON output.

## Goals
- Keep formatting reusable and host-neutral inside the builder core.
- Add a dedicated thin host project for command-line inspection execution.
- Support selectable plain-text and JSON output for the first host-facing slice.
- Cover formatter and host behavior with stable validation.

## Assumptions
- The formatter should not replace the structured result contracts; it should only project them into host-friendly output.
- The thin host can stay intentionally minimal and focus on argument parsing, DI composition, and output emission.
- `V1.2` compilation and emit remain deferred until after this host-facing inspection slice.

## Open Questions
- Should later host slices add Markdown output, or should Markdown remain a separate reporting concern?
- How much command-line surface should be added before a more general command infrastructure exists?

## Status
- In Progress
