# T-017 Implement Project Explorer UI

## Parent
B-Project-Explorer | Previous: T-016 | Next: T-018

## Status
Planned

## Request
Create the reusable Avalonia TreeView component, CommunityToolkit.Mvvm ViewModels, commands, and DI composition. File activation delegates to the shared code-location/document navigation capability.

## Planned tests
- Children, expansion, and selection bindings.
- File selection invokes one open request.
- Folder selection does not invoke document opening.
- Unavailable nodes expose a defined UI state.
- Explorer has no direct AvaloniaEdit access.

## Gate and completion
Pass G-Integration. Before Done: test evidence, English SVN commit/revision, B-Project-Explorer Done, T-018 In Progress, feature status update.