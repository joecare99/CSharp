# T-022 Validate Diagnostics Jump-to-Code

## Parent
B-Diagnostics-Jump-To-Code | Previous: T-021 | Next: T-008

## Status
Done

## Request
Prove the complete diagnostic UI to editor caret workflow and publish consumption guidance for AXaml parsing and future compiler diagnostics.

## Planned tests
- End-to-end fixture: diagnostic -> mapping -> navigator -> editor-host open/focus/caret request.
- File path with line/column and file-only diagnostic scenarios.
- Missing file and invalid coordinates expose documented error behavior.
- Existing debug output consumer regression test.
- Execute diagnostics, navigation, and editor test projects on net8.0 and available higher targets.

## Gate and completion
Completed: G-Integration passed. The end-to-end fixture proves `DiagnosticCollectionViewModel` -> `DiagnosticLocationMapper` -> `EditorCodeNavigator` -> `IEditorDocumentHost` -> `IEditorCaretService`. File-only locations, invalid coordinates, missing documents, and the existing debug consumer regression are covered.

Test matrix: AA98 diagnostics/integration tests 33/33 on net8.0, net9.0, and net10.0; editor navigation tests 48/48 on net8.0, net9.0, and net10.0; Code.Navigation tests 27/27 on net8.0, net9.0, and net10.0; Diagnostics.Navigation tests passed in the same validation run. SVN revision: 1851.

Handoff: `B-Diagnostics-Jump-To-Code` is Done. `T-008 Define Property Contracts` is In Progress.
