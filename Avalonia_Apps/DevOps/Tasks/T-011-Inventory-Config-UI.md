# T-011 Inventory Config UI Baseline

## Parent
B-Config-UI | Previous: T-010 | Next: T-012

## Status
Done

## Request
Inventory the current RnzTrauer configuration presentation baseline before
changing code. `RnzTrauer.Avalonia.Config` does not exist; inspect
`RnzTrauer.Avalonia`, Config.Service, its provider projects, and their tests
to determine whether an existing UI can be adapted or Config.UI must begin as
a greenfield shared component. Record project paths, public API, views,
ViewModels, DI, models, UI strings, dependencies, and test coverage; classify
each element as shared, host adapter, or product-specific.

## Planned tests
- Run the relevant RnzTrauer and Config.Service test suites on net8.0.
- Reference audit identifies every RnzTrauer-specific dependency.
- Document the absence or presence of UI commands and persistence behavior as
  baseline scenarios.

## Gate and completion
A documented dependency matrix is mandatory before extraction. Before Done: test evidence, English SVN commit/revision if artifacts change, current/next/backlog/feature status update.

## Inventory and dependency matrix

| Area | Actual baseline | Classification |
|---|---|---|
| `Libraries\Config.Service` | Standalone multi-target `net8.0`/`net9.0`/`net10.0` component. `IConfigSectionProvider`, `IConfigSectionRegistry`, `IConfigStore`, `ConfigService`, section registrations, JSON store, and reflection metadata inspection are public or internal service behavior. | Shared configuration core |
| `Config.Service` DI | `AddConfigService(vendor, application)` owns the JSON store root; `AddConfigSection` preserves provider order and UI display/description metadata. | Host registration boundary |
| Persistence | `JsonConfigStore` writes below `%LocalAppData%\<vendor>\<application>\config`, or beneath test-only `CONFIG_ROOT`. Section names are file keys. | Shared service behavior; host supplies isolated vendor/application identity |
| RnzTrauer providers | `DatabaseConfigProvider` (order 0), `PlaceConfigProvider` (1), `AcquisitionConfigProvider` (2), and `AnnouncementsConfigProvider` (3) own localized German display metadata and product model types. | Product-specific adapters |
| `RnzTrauer.Avalonia` | A net8.0 application containing App, MainWindow, and MainWindowViewModel only. It references product modules but neither Config.Service nor a configuration UI. | Product host |
| RnzTrauer settings tab | Displays `RNZ_DB_*` environment values read-only and deliberately omits the password. It has no load/save/reset commands, section selection, or provider registration. | Product-specific migration placeholder; not reusable |
| Existing tests | `Config.Service.Tests`: 19/19; `RnzTrauer.Avalonia.Tests`: 8/8 on net8.0. Neither tests a config UI. | Baseline coverage |

## Decision
No configuration UI exists to adapt. T-012 therefore creates `Config.UI` as a
greenfield reusable Avalonia component. It may consume Config.Service and
`Property.Editor` through host-owned DI registrations, but must not reference
RnzTrauer provider/model types, expose environment variables as a persistence
substitute, or carry RnzTrauer's German strings. Hosts supply their vendor and
application identity plus localized section metadata.

## Validation
- `dotnet test Libraries\Config.Service.Tests\Config.Service.Tests.csproj -f net8.0`:
  19/19 passed.
- `dotnet test RnzTrauer\RnzTrauer.Avalonia.Tests\RnzTrauer.Avalonia.Tests.csproj`:
  8/8 passed on net8.0.

## Version control
- SVN r1891: `Inventory Config UI baseline`.

## Handoff
- T-012 is ready to define the neutral Config.UI adapter contracts and project
  boundaries from this confirmed greenfield baseline.