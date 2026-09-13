# T-010 Validate Property Editor

## Parent
B-Property-Editor | Previous: T-009 | Next: T-011

## Status
Done

## Request
Validate the component's host independence and publish adapter guidance for configuration and future AXaml designer consumers.

## Planned tests
- Consumer fixture supplies properties without RnzTrauer or ConsoleLib types.
- Error and cancellation behavior.
- Complete dedicated test project execution and available framework builds.

## Gate and completion
Pass G-Component-Quality. Before Done: test/commit/status evidence, set B-Property-Editor Done, T-011 In Progress, and update feature status.

## Validation
- The NSubstitute-based consumer fixture supplies only
  `Property.Editor.IPropertyItem`; it proves a scalar host can apply a valid
  change and surface the contract's localized validation message without
  RnzTrauer or ConsoleLib dependencies.
- `dotnet test Libraries\Property.Editor.Avalonia.Tests\Property.Editor.Avalonia.Tests.csproj`:
  9/9 passed on net8.0, net9.0, and net10.0.
- Property editing has no asynchronous or cancellable operation. The component
  rejects invalid values synchronously through `TrySetValue` and leaves the
  last valid value unchanged; this failure behavior is covered by the
  dedicated view-model and fixture tests.
- The Planning host uses the public DI registration and panel; its
  cross-framework integration suite passed under T-009.

## Version control
- SVN r1890: `Validate shared property editor consumers`.

## Handoff
- B-Property-Editor is Done.
- T-011 is In Progress and inventories the actual RnzTrauer configuration
  baseline before Config.UI design begins.