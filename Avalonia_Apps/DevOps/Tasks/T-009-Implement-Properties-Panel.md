# T-009 Implement Properties Panel

## Parent
B-Property-Editor | Previous: T-008 | Next: T-010

## Status
Done

## Request
Implement reusable Avalonia PropertiesPanel ViewModels and control templates using CommunityToolkit.Mvvm. Support basic scalar, nullable scalar, Boolean, and enum editing; specialized editors are supplied by adapters later.

## Planned tests
- Correct template selection by property type.
- Read-only items disable editing.
- Valid changes raise the neutral change callback once.
- Validation errors are visible, localizable, and preserve prior value.
- Category ordering and binding behavior.

## Gate and completion
Before Done: test ViewModels with NSubstitute and UI bindings where available, build net8.0, English SVN commit/revision, advance to T-010, update parent statuses.

## Delivered
- Added the standalone `Property.Editor.Avalonia` component with an
  `AddPropertyEditorAvalonia` DI registration and a reusable
  `PropertyEditorPanel`.
- Added scalar, nullable scalar, Boolean, and enum templates. Values are
  applied only through `IPropertyItem.TrySetValue`, so invalid input keeps the
  last valid value and displays the consumer-supplied validation text.
- Added `Property.Editor.Avalonia.Tests`, covering scalar conversion,
  nullable values, Boolean changes, enum options, read-only rejection, invalid
  input preservation, and neutral ordering.
- Replaced the Planning-specific editor layout with a thin host that binds the
  reusable panel to a `PropertyEditorViewModel`. Planning-specific typed
  conversion and model mutation remain in its adapter.

## Validation
- `dotnet test Libraries\Property.Editor.Avalonia.Tests\Property.Editor.Avalonia.Tests.csproj`:
  7/7 passed on net8.0, net9.0, and net10.0.
- `dotnet test AA98_AvlnCodeStudio\AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj`:
  112/112 passed on net8.0 and net9.0; 126/126 passed on net10.0.

## Version control
- SVN r1888: `Add reusable Avalonia property editor`.

## Handoff
- T-010 may now validate host-independent fixture consumption and component
  quality before closing B-Property-Editor.