# AA98-T047 Add Editor and Workspace Validation

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl039-Linux-Editor-and-Workspace-Baseline.md`

## Goal
Add validation for Linux-compatible workspace and editor file workflows.

## Scope
- Add tests for path normalization and file open/save behavior.
- Include representative `.cs`, `.axaml`, and `.md` cases where practical.
- Document any remaining manual validation.

## Execution Notes
1. Focus on services and view models before UI-only tests.
2. Use explicit file paths and test data.
3. Avoid brittle OS-specific expectations.

## Acceptance Criteria
- Relevant editor/workspace behavior has repeatable validation.
- Known remaining Linux risks are documented.

## Validation
- Run targeted tests and record results.

## Implementation Notes
- Automated validation now covers Linux-style `.cs`, `.axaml`, and `.md` path handling through `AA98_AvlnCodeStudio.Editor.Tests/Services/EditorWorkflowTests.cs` and `AA98_AvlnCodeStudio.Tests/Documents/FileEditorDocumentTests.cs`.
- Manual Linux editor/workspace smoke validation is documented in `../Validation/AA98-Linux-Editor-Workspace-Smoke-Checklist.md` for the remaining full-shell workflow.

## Remaining Risks
- A dedicated workspace-open or file-tree service is still outside the current AA98 slice, so Linux validation remains focused on editor-centric file workflows.

## Validation Result
- A repeatable automated and manual validation path now exists for the current Linux editor/workspace baseline.

## Status
- Completed
