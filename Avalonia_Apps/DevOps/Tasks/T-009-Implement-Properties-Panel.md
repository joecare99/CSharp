# T-009 Implement Properties Panel

## Parent
B-Property-Editor | Previous: T-008 | Next: T-010

## Status
Planned

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