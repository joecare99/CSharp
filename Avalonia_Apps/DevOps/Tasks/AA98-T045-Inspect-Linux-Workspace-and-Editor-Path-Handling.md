# AA98-T045 Inspect Linux Workspace and Editor Path Handling

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl039-Linux-Editor-and-Workspace-Baseline.md`

## Goal
Inspect workspace, file tree, editor open, edit, and save paths for Linux portability issues.

## Scope
- Review path separator, casing, file watcher, and encoding assumptions.
- Inspect editor handling for `.cs`, `.axaml`, and `.md` files.
- Identify minimal fixes required for self-hosting source editing.

## Execution Notes
1. Trace workspace open and document open/save flows.
2. Prefer symbol-aware searches for key services where available.
3. Record issues before implementation begins.

## Acceptance Criteria
- Linux path risks are listed with affected files or components.
- `AA98-T046` can be implemented from the findings.

## Validation
- No code changes are required for this inspection task.

## Findings
- `AA98_AvlnCodeStudio.Editor/Services/EditorWorkflow.cs` uses `Environment.SpecialFolder.MyDocuments` as the fallback initial directory when the current document has no file path. This is the main Linux portability risk because the resolved directory may vary across desktop environments and may not represent the active workspace.
- The same workflow passes the current directory directly to open/save dialogs. The dialog adapter in `AA98_AvlnCodeStudio.UI/Services/AvaloniaEditorFileDialogService.cs` does not normalize or validate the directory before assigning it to `InitialDirectory`.
- `AA98_AvlnCodeStudio.Model/Documents/FileEditorDocument.cs` uses `Path.GetFileName`, which is separator-neutral and does not currently expose a Windows-only bug by itself.
- Current automated tests in `AA98_AvlnCodeStudio.Editor.Tests/Services/EditorWorkflowTests.cs` and `AA98_AvlnCodeStudio.Tests/Documents/FileEditorDocumentTests.cs` use only Windows-style sample paths such as `C:\Temp\sample.cs`. This leaves Linux path behavior unvalidated.
- No explicit workspace-open or file-tree service was found in the current AA98 slice. The first Linux path fixes can therefore stay focused on editor workflow defaults, dialog initial directories, and cross-platform tests.

## Minimal Fix Candidates for AA98-T046
- Replace the `MyDocuments` fallback with a more platform-neutral initial-directory strategy for unsaved documents.
- Add tests that exercise Linux-style absolute paths and verify separator-neutral behavior.
- Keep file open/save changes localized to the editor workflow and dialog boundary before broader workspace infrastructure exists.

## Status
- Completed
