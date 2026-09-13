# T-008 Define Property Contracts

## Parent
B-Property-Editor | Previous: T-022 | Next: T-009

## Status
Done

## Request
Consolidate existing property item patterns into a neutral contract for category, display name, type, value, editability, options, validation, and change notification. No ConsoleLib reflection or product model type may enter the contract.

## Planned tests
- Scalar, nullable, Boolean, and enum values.
- Read-only values and empty option sets.
- Category/property deterministic ordering.
- Invalid update does not replace the last valid value.
- Contract reference audit.

## Gate and completion
Pass G-Architecture. Before Done: document contract, run MSTest/build, make English SVN commit, record revision/results, set T-009 In Progress, and update B-Property-Editor and feature status.

## Delivered
- Added the standalone `Property.Editor` component and its dedicated
  `Property.Editor.Tests` MSTest project.
- Defined neutral category, item, option, editor-kind, validation, ordering,
  and value-change contracts without UI, OS, model, planning, or product
  dependencies.
- The default `PropertyItem` implementation validates type, nullability,
  closed options, and consumer validators before retaining a new value.
  Invalid or read-only updates preserve the prior valid value and expose the
  corresponding validation result.
- Deterministic ordering uses category order/name followed by property
  order/name; consumers remain responsible for supplying localized display
  text at their registration boundary.

## Architecture review
- `Property.Editor` contains no project or package references, so the
  contract remains independent of Avalonia, RnzTrauer, CodeStudio,
  ConsoleLib, planning, and operating-system concerns.
- `IPropertyItem` exposes the capability required by a future UI adapter,
  while `PropertyItem` is an optional default implementation. This keeps
  product adapters free to provide their own data sources.
- The obsolete string-only `IPropertyItem` and `PropertyItem` types were
  removed from `AA98_AvlnCodeStudio.Base.UI.Properties`. Its
  `IHasProperties` capability now exposes `Property.Editor.IPropertyItem`,
  making `Property.Editor` the only public property-item contract.
- Planning preserves its product-specific mutation and text-to-typed-value
  conversion in `PlanningPropertyItemViewModel`, which composes the canonical
  `Property.Editor.PropertyItem`. It does not introduce an alternate contract.

## Validation
- `dotnet test Libraries\Property.Editor.Tests\Property.Editor.Tests.csproj -f net8.0`: 10/10 passed.
- `dotnet test Libraries\Property.Editor.Tests\Property.Editor.Tests.csproj -f net9.0`: 10/10 passed.
- `dotnet test Libraries\Property.Editor.Tests\Property.Editor.Tests.csproj -f net10.0`: 10/10 passed.
- `dotnet test AA98_AvlnCodeStudio\AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj`:
  112/112 passed on net8.0 and net9.0; 126/126 passed on net10.0. The
  Planning adapter test verifies typed enum options, rejected invalid values,
  and application of a valid update.

## Version control
- SVN r1879: `Add neutral property editor contracts`.
- SVN r1886: `Consolidate CodeStudio property contracts`.

## Handoff
- `B-Property-Editor` remains In Progress.
- T-009 may now start its reusable Avalonia control extraction; it must replace
  the temporary Planning properties layout instead of recreating a contract.