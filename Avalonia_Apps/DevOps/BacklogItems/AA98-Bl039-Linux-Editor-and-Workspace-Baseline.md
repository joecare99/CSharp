# AA98-Bl039 Linux Editor and Workspace Baseline

## Parent
- Feature: `../Features/AA98-F41-Linux-Workbench-Base.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
A developer can open the AA98 workspace on Linux and edit core source and planning files.

## Scope
- Validate workspace open and close behavior on Linux.
- Validate file tree and file path behavior with Linux paths.
- Stabilize open/edit/save flows for `.cs`, `.axaml`, and `.md` files.
- Keep editor behavior componentized and reusable.

## Acceptance Criteria
- The AA98 workspace can be opened from a Linux path.
- Core source and planning files can be edited and saved.
- Path handling avoids Windows-only separators and casing assumptions.

## Implementation Tasks
- `AA98-T045 Inspect Linux Workspace and Editor Path Handling`
- `AA98-T046 Implement Linux Editor and Workspace Fixes`
- `AA98-T047 Add Editor and Workspace Validation`

## Assumptions
- `.resx` and `json` support can remain later unless needed for self-hosting.

## Open Questions
- Should markdown planning files be part of the first editor validation set?

## Next Refinement Steps
1. Start with path and workspace inspection.
2. Add focused tests for the first fixed path behaviors.

## Discovery Notes
- The current editor workflow fallback directory for unsaved documents is implemented in `AA98_AvlnCodeStudio.Editor/Services/EditorWorkflow.cs` through `Environment.SpecialFolder.MyDocuments`, which is a weak Linux baseline because it may not align with the active workspace or with desktop-specific folder mappings.
- `AA98_AvlnCodeStudio.UI/Services/AvaloniaEditorFileDialogService.cs` forwards the provided initial directory directly into the Avalonia dialogs and currently does not guard or normalize the value.
- `AA98_AvlnCodeStudio.Model/Documents/FileEditorDocument.cs` uses `Path.GetFileName`, which is already separator-neutral and should remain stable for Linux-style paths.
- Current tests for editor workflow and file documents use only Windows-style sample paths, leaving Linux path expectations unverified.
- No dedicated workspace-open or file-tree path service is visible in the current AA98 slice, so the first implementation work can stay tightly scoped to editor open/save path defaults and cross-platform tests.

## Implementation Progress
- `AA98-T045` inspected the current editor and document path handling and identified the `MyDocuments` fallback plus Windows-only test data as the first Linux portability risks.
- `AA98-T046` replaced the `MyDocuments` fallback with a platform-neutral current-directory strategy plus `UserProfile` reserve and added Linux-style path coverage in editor and document tests.
- `AA98-T047` completed the repeatable validation path with additional Linux file workflow tests and a manual smoke checklist for the current shell-driven editor baseline.

## Task Status Snapshot
- `AA98-T045 Inspect Linux Workspace and Editor Path Handling` - Completed
- `AA98-T046 Implement Linux Editor and Workspace Fixes` - Completed
- `AA98-T047 Add Editor and Workspace Validation` - Completed

## Status
- Completed
