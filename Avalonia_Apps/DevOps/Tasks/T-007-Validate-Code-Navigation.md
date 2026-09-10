# T-007 Validate Code Navigation

## Parent
B-Code-Navigation | Previous: T-006 | Next: T-020

## Status
Done

## Request
Validate the public navigation capability and document consumer integration rules.

## Consumer integration rules
- Consumers reference `ICodeNavigator`, never AvaloniaEdit controls or editor tabs.
- `CodeLocation` is the only source-location input and normalizes paths while
  validating optional coordinates.
- The editor host owns document activation, focus, and coordinate clamping or
  rejection through `IEditorDocumentHost` and `IEditorCaretService`.
- Missing documents produce `FileNotFoundException`; caret-service failures
  are propagated to the caller.
- The canonical library is under `Avalonia_Apps\Libraries`; the parallel
  `CSharpBible\Libraries` project links the same source files.

## Planned tests
- [x] Full navigator-to-editor-host fixture path.
- [x] Navigator cancellation and failure propagation.
- [x] No direct AvaloniaEdit dependency in consumer contracts.
- [x] Build and test each available target framework.

## Validation evidence
- `AA98_AvlnCodeStudio.Editor.Tests`: 16/16 passed on net8.0.
- `Avalonia_Apps\Libraries\Code.Navigation.Tests`: 9/9 passed on net8.0.
- `CSharpBible\Libraries\Code.Navigation.Tests`: 9/9 passed on net8.0.
- Both production libraries built successfully on net8.0, net9.0, and net10.0.
- G-Component-Quality: Passed for B-Code-Navigation.

## Handoff
- Current task: Done.
- Current backlog: B-Code-Navigation Done.
- Next task: T-020 Map Diagnostics to Code Locations In Progress.

## Gate and completion
G-Component-Quality passed. Evidence, English SVN commit/revision, backlog
completion, next-task activation, and feature status update are recorded.
