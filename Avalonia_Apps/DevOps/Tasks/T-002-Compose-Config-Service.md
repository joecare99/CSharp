# T-002 Compose Config.Service

## Parent
B-Config-Core | Previous: T-001 | Next: T-003

## Status
Done

## Request
Add a central host-facing DI composition path for the standalone Config.Service component. Hosts supply vendor, application, and section providers; no product model or UI type enters the core.

## Planned tests
- Service provider resolves ConfigService, IConfigStore, and IConfigSectionRegistry once.
- Multiple providers are discoverable in ascending Order.
- Invalid service collection, vendor, application, and provider inputs follow documented failure behavior.
- Dependency test verifies UI- and product-neutral core references.

## Implementation evidence

- Added DI container coverage using `Microsoft.Extensions.DependencyInjection`
  in the dedicated test project.
- Verified that `ConfigService`, `IConfigStore`, and
  `IConfigSectionRegistry` resolve as the same singleton instances on repeated
  requests.
- Verified vendor/application identity, base-key construction, provider
  registration, ascending registry order, and UI description overrides.
- Verified null service collections, null providers, empty vendor/application
  values, and an empty legacy base key fail with the documented argument
  exceptions.
- Corrected two production defects found by the new tests: a legacy base key
  containing only separators previously caused `IndexOutOfRangeException`, and
  the provider-only section overload previously dereferenced a null provider
  before validating it.

## Validation evidence

- `dotnet test Config.Service.Tests.csproj --framework net8.0`: passed,
  13 tests passed, 0 failed, 0 skipped.
- `dotnet build Config.Service.csproj --framework net8.0`: passed.
- SVN revision: 1835.
- English commit message: `Compose Config Service through DI` — harden DI
  composition and add regression coverage for T-002.
- Static dependency audit found no Avalonia, ConsoleLib, CodeStudio,
  Planning, WinForms, WPF, or product-project reference in the production
  component. The only matching text is the illustrative XML documentation
  example `RnzTrauer.Database`.
- Existing nullable warnings remain confined to the test fixtures; no new
  production warning was introduced.

## Gate decision

G-Architecture remains passed. T-002 is approved for completion and T-003 may
start; persistence edge cases remain explicitly out of scope and are assigned
to T-003.

## Gate and completion
Pass G-Architecture. Before Done: implement/document, run tests and net8.0 build, SVN commit in English, record revision/message/results, mark current Done and T-003 In Progress, then update B-Config-Core and feature status.