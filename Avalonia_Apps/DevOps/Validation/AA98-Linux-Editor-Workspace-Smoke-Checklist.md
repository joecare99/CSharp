# AA98 Linux Editor and Workspace Smoke Checklist

## Purpose
Provide a repeatable manual smoke validation path for Linux-compatible editor and workspace file workflows until broader workspace automation is available.

## Scope
- Validate editor open, edit, and save behavior on Linux.
- Cover representative `.cs`, `.axaml`, and `.md` files.
- Record remaining workspace limitations that are outside the current AA98 slice.

## Preconditions
- Build the AA98 editor and shell projects for the target runtime.
- Run inside a Linux desktop session with the AA98 workbench startup baseline already validated.
- Prepare one `.cs`, one `.axaml`, and one `.md` file under a Linux workspace path.

## Smoke Steps
1. Start the AA98 shell on Linux and wait for the main window to appear.
2. Open a `.cs` file from a Linux workspace path and confirm that its content appears in the editor.
3. Modify the `.cs` file, save it, and confirm that the file path and dirty state update correctly.
4. Open a `.axaml` file from a Linux workspace path and confirm that it loads without path-related errors.
5. Use Save As for a `.md` planning file under a Linux workspace path and confirm that the new file is created at the selected path.
6. Re-open the saved `.md` file and confirm that the content and display name remain stable.
7. Record any remaining issues related to workspace tree, folder opening, or Linux desktop integration that are not yet covered by the current AA98 slice.

## Expected Results
- `.cs`, `.axaml`, and `.md` files can be opened from Linux-style paths.
- Save and Save As keep the expected file path and clear the dirty state.
- Remaining issues are recorded as later workspace-infrastructure tasks rather than hidden inside the editor workflow.

## Automation Link
- Automated validation lives in `AA98_AvlnCodeStudio.Editor.Tests/Services/EditorWorkflowTests.cs` and `AA98_AvlnCodeStudio.Tests/Documents/FileEditorDocumentTests.cs`.
- This checklist covers the remaining full-shell Linux editor/workspace smoke path that is not yet modeled by dedicated workspace services.
