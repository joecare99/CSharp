# T-013 Implement Shared Config UI

## Parent
B-Config-UI | Previous: T-012 | Next: T-014

## Status
Done

## Request
Extract or adapt the existing UI into a reusable Avalonia component with View, CommunityToolkit.Mvvm ViewModels, commands, DI registration, Config.Service integration, and shared PropertiesPanel hosting.

## Planned tests
- First provider selection and section switching.
- Load, save, reset, reload, busy, and error commands.
- Unsaved-change policy during section changes.
- Property validation blocks persistence by documented policy.
- ViewModel substitutes and Avalonia binding/control behavior.

## Gate and completion
Before Done: tests on success/error/regression paths pass, net8.0 build passes, English SVN commit/revision recorded, T-014 In Progress, all statuses updated.

## Delivered
- Added `Config.UI.ConfigService`, a dedicated Config.Service adapter that
  always calls the public `ConfigService` facade. This preserves each host's
  vendor/application-prefixed persistence root instead of bypassing it through
  direct store calls.
- The adapter reflects only writable public model properties, excludes
  `ConfigIgnore` properties, maps enum choices to `PropertyOption`, blocks
  persistence while any neutral property is invalid, and maps
  `SensitiveConfigProperty` to the shared masking metadata.
- Added `Config.UI.Avalonia` with `ConfigUiViewModel`, CommunityToolkit relay
  commands, DI registration, and `ConfigUiView`. It hosts the shared
  `PropertyEditorPanel` rather than duplicating editor templates.
- Section changes retain an already-loaded section's in-memory draft. Explicit
  Load and Reset discard that draft; a successful Save marks it persisted.

## Validation
- `dotnet build Libraries\Libraries.sln -f net8.0 --no-restore`: succeeded.
- `Property.Editor.Tests`: 11/11; `Property.Editor.Avalonia.Tests`: 10/10;
  `Config.UI.ConfigService.Tests`: 4/4; and `Config.UI.Avalonia.Tests`: 6/6
  passed on each of net8.0, net9.0, and net10.0.
- The Config.Service adapter tests use NSubstitute for the store and cover
  typed projection, prefixed save, read-only reset rejection, invalid-value
  persistence blocking, ignored properties, enums, and sensitive metadata.

## Version control
- SVN revision: r1893 (`Implement shared Config UI`).

## Handoff
- T-014 may register this component through RnzTrauer and CodeStudio host
  composition roots with their own provider registrations and storage
  identities.