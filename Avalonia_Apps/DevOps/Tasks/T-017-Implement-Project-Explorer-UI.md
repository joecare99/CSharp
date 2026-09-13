# T-017 Implement Project Explorer UI

## Parent
B-Project-Explorer | Previous: T-016 | Next: T-018

## Status
Done

## Request
Create the reusable Avalonia TreeView component, CommunityToolkit.Mvvm ViewModels, commands, and DI composition. File activation delegates to the shared code-location/document navigation capability.

## Planned tests
- Children, expansion, and selection bindings.
- File selection invokes one open request.
- Folder selection does not invoke document opening.
- Unavailable nodes expose a defined UI state.
- Explorer has no direct AvaloniaEdit access.

## Delivered
- Added `Project.Explorer.Avalonia` with a reusable TreeView, observable item
  and explorer ViewModels, refresh command, DI registration, selection state,
  expansion-state restoration, unavailable-node representation, and visible
  error feedback.
- Added a CodeStudio-owned opener adapter that maps only available files to
  `CodeLocation` and delegates activation to the registered `ICodeNavigator`.
  The shared explorer projects have no AvaloniaEdit dependency.
- The CodeStudio workbench hosts the reusable explorer alongside its
  unchanged Planning Explorer, whose domain remains separate.

## Validation
- `Project.Explorer.Avalonia.Tests`: 3/3 passed on net8.0, net9.0, and
  net10.0. Coverage includes hierarchy selection/expansion restoration, file
  activation, folder/unavailable-item suppression, and source errors.
- `AA98_AvlnCodeStudio.Tests`: 114/114 on net8.0 and net9.0; 128/128 on
  net10.0. The host composition test verifies file opening invokes the
  registered `ICodeNavigator`.
- `dotnet build Libraries\Libraries.sln -f net8.0 --no-restore`: succeeded.

## Version control
- SVN revision: recorded with the implementation commit.

## Handoff
- B-Project-Explorer is Done.
- T-018 is In Progress.