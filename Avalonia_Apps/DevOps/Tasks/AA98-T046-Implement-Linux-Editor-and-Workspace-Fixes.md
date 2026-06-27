# AA98-T046 Implement Linux Editor and Workspace Fixes

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl039-Linux-Editor-and-Workspace-Baseline.md`

## Goal
Implement Linux-portable workspace and editor fixes needed for self-hosting source editing.

## Scope
- Fix path and file handling issues discovered by `AA98-T045`.
- Preserve component boundaries between workspace, editor, and shell.
- Keep changes focused on `.cs`, `.axaml`, and `.md` self-hosting workflows.

## Execution Notes
1. Use platform-neutral path APIs and abstractions.
2. Avoid hard-coded separators and casing assumptions.
3. Keep editor behavior reusable outside the main shell.

## Acceptance Criteria
- Core source and planning files can be opened, edited, and saved through Linux-compatible paths.
- Platform-specific behavior is isolated or removed.

## Validation
- Run targeted editor/workspace tests.
- Add missing tests in `AA98-T047` if not included here.

## Implementation Notes
- `AA98_AvlnCodeStudio.Editor/Services/EditorWorkflow.cs` now uses a platform-neutral initial-directory strategy for unsaved documents: existing document paths still resolve through `Path.GetDirectoryName`, while new documents prefer `Environment.CurrentDirectory` and fall back to `UserProfile` only when needed.
- Linux-style path behavior is now covered in `AA98_AvlnCodeStudio.Editor.Tests/Services/EditorWorkflowTests.cs` and `AA98_AvlnCodeStudio.Tests/Documents/FileEditorDocumentTests.cs`.

## Validation Result
- Targeted tests were added for Linux-style file paths and for the unsaved-document dialog seed path.
- Relevant editor and document tests should be executed as the task validation step.

## Status
- Completed
