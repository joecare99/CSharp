# T-012 Define Config UI Contracts

## Parent
B-Config-UI | Previous: T-011 | Next: T-013

## Status
Done

## Request
Define product-neutral Avalonia-facing section navigation, section metadata, loading, editing, saving, reset, busy, and error contracts. Host registration owns localizable display names and descriptions.

## Planned tests
- Provider order and selected default section.
- Missing description and invalid metadata.
- Read-only, unavailable, loading, and failure states.
- UI contract does not reference RnzTrauer, ConsoleLib, or CodeStudio models.

## Gate and completion
Pass G-Architecture. Before Done: all tests/build/documentation, English SVN commit and revision, T-013 In Progress, and parent statuses updated.

## Delivered
- Added standalone `Config.UI` and `Config.UI.Tests` projects with only a
  forward dependency on `Property.Editor`.
- Defined localized `ConfigUiSection` metadata, explicit asynchronous section
  states, and `IConfigUiSection`/`IConfigUiSectionRegistry` capability
  contracts. The host adapter owns all Config.Service model/store/reflection
  details, which remain implementation work for T-013.

## Validation
- `dotnet test Libraries\Config.UI.Tests\Config.UI.Tests.csproj`: 3/3 passed
  on net8.0, net9.0, and net10.0.
- Reference audit confirms no RnzTrauer, ConsoleLib, CodeStudio, Avalonia, or
  operating-system dependency in the contracts.

## Version control
- SVN r1892: `Define neutral Config UI contracts`.