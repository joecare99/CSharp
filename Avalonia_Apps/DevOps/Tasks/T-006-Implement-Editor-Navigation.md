# T-006 Implement Editor Navigation

## Parent
B-Code-Navigation | Previous: T-005 | Next: T-007

## Status
Done

## Request
Implement the CodeStudio editor-host adapter for the code-location navigator. It opens or reuses a document, focuses it, and positions the caret while keeping AvaloniaEdit access in UI infrastructure.

## Implementation
- Added `EditorCodeNavigator` to `AA98_AvlnCodeStudio.Editor`.
- The shared `Code.Navigation` contract is hosted canonically under
  `Avalonia_Apps\Libraries` and is consumed by the editor from that location.
- A parallel `CSharpBible\Libraries` project is provided with linked source
  files, avoiding a second independently maintained implementation.
- Added `IEditorDocumentHost`, `IEditorDocument`, and `IEditorCaretService` as
  small host/UI boundaries.
- Missing documents produce `FileNotFoundException`.
- Cancellation is checked before contacting the host and is forwarded to both
  host operations.
- Coordinate clamping/rejection remains the responsibility of the
  AvaloniaEdit-facing caret service.

## Planned tests
- [x] Navigate an already open document.
- [x] Open then navigate a closed document through the host contract.
- [x] Forward coordinates and offsets without changing the documented policy.
- [x] Return a defined exception for a missing file.
- [x] Repeated navigation requests preserve a consistent host/caret contract.
- [x] Cancel before host access.

## Validation
- `get_errors`: no diagnostics in the new adapter or test file.
- Direct net8.0 build completed successfully after the Strong Name Key became
  available at `Avalonia_Apps\Libraries\sgLib.snk`.
- `AA98_AvlnCodeStudio.Editor.Tests`: 15 tests passed, 0 failed.
- The build reports four pre-existing unused-event warnings in
  `NullTerminalSession`; no adapter-related warnings or errors were reported.

## Handoff
- Current task: Done.
- Next task: T-007 Validate Code Navigation.
- T-006 is ready for integration validation once the shared build prerequisites
  are restored.

## Gate and completion
Before Done: use substitutes for document/editor services, run relevant MSTests and net8.0 build, create English SVN commit, record revision/results, advance status to T-007 and update parent items.