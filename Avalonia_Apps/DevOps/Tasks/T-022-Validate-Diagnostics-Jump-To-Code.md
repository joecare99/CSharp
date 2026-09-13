# T-022 Validate Diagnostics Jump-to-Code

## Parent
B-Diagnostics-Jump-To-Code | Previous: T-021 | Next: T-008

## Status
Done

## Request
Prove the complete diagnostic UI to editor caret workflow in a real
DI-composed host and publish consumption guidance for AXaml parsing and future
compiler diagnostics.

## Planned tests
- End-to-end fixture: diagnostic -> mapping -> navigator -> editor-host open/focus/caret request.
- File path with line/column and file-only diagnostic scenarios.
- Missing file and invalid coordinates expose documented error behavior.
- Existing debug output consumer regression test.
- Execute diagnostics, navigation, and editor test projects on net8.0 and available higher targets.
- A host composition test resolves the diagnostics consumer with an
  `ICodeNavigator` and proves activation reaches the editor document and caret
  services.
- The selected host registers the document host, caret service, and
  `ICodeNavigator` through DI; it must not rely on
  `DiagnosticCollectionViewModel`'s parameterless fallback constructor.

## Gate and completion
The prior fixture proves `DiagnosticCollectionViewModel` ->
`DiagnosticLocationMapper` -> `EditorCodeNavigator` -> `IEditorDocumentHost`
-> `IEditorCaretService`, and it remains valid regression evidence. It did
not prove runtime composition: `AA98.DevOpsPlanning.Host` calls
`AddDiagnosticsUi()` without registering an `ICodeNavigator`, so activation
reports that no navigator is available.

G-Integration is reopened until a production host and host-level integration
test prove the complete DI chain. After successful build and tests, record the
English SVN revision, set this task and B-Diagnostics-Jump-To-Code Done, and
activate T-008.

## Completion evidence
- The full `AA98_AvlnCodeStudio.UI` workbench now registers
  `IEditorDocumentHost`, `IEditorCaretService`, and `ICodeNavigator` before
  registering the diagnostics UI. Its Diagnostics panel consumes the injected
  navigator instead of the parameterless fallback.
- `WorkbenchEditorDocumentHost` opens requested source files through the
  existing editor workflow. `WorkbenchEditorCaretService` positions the
  AvaloniaEdit caret while retaining all UI dependencies in the workbench UI
  project.
- The host composition regression replaces only the UI adapters with
  NSubstitute doubles, resolves the diagnostics consumer from the actual App
  registrations, and proves activation reaches the registered document host
  and caret service.
- `AA98_AvlnCodeStudio.Tests`: 111/111 on net8.0 and net9.0; 125/125 on
  net10.0. `AA98_AvlnCodeStudio.Editor.Tests`: 17/17 on net8.0, net9.0, and
  net10.0. The workbench UI also built successfully on net8.0.
- SVN r1884: `Integrate diagnostics navigation in CodeStudio workbench`.

## Handoff
- `B-Diagnostics-Jump-To-Code` is Done and G-Integration is Passed.
- `T-008 Define Property Contracts` and `B-Property-Editor` are In Progress.
