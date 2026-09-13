# T-014 Integrate Config UI Hosts

## Parent
B-Config-UI | Previous: T-013 | Next: T-015

## Status
Done

## Request
Register the shared Config.UI from RnzTrauer and CodeStudio hosts only through DI and provider adapters. Do not duplicate views, ViewModels, storage, or configuration models.

## Planned tests
- Each host sees only registered sections.
- Vendor/application storage roots are isolated.
- Host-localized metadata is displayed and not persisted as core data.
- Provider error in one host does not affect another host fixture.

## Delivered
- RnzTrauer registers the shared stack through `AddConfigurationUi`, retaining
  ownership of the Database, Places, Acquisition, and Announcements provider
  registrations in their documented order.
- CodeStudio registers its host-owned Workbench settings provider while
  consuming the shared Config.Service adapter and Avalonia UI.
- Both hosts inject the shared `ConfigUiViewModel` and host `ConfigUiView`
  rather than copying configuration views or ViewModels.

## Validation
- `RnzTrauer.Avalonia.Tests`: 9/9 passed on net8.0, including real DI
  composition, ordered-section, and shared ViewModel resolution coverage.
- `AA98_AvlnCodeStudio.Tests`: 113/113 on net8.0 and net9.0; 127/127 on
  net10.0. Its host-composition test verifies the single Workbench section,
  shared ViewModel, and CodeStudio identity.
- The shared Config.UI matrix passed serially on net8.0, net9.0, and net10.0:
  `Property.Editor.Tests` 11/11, `Property.Editor.Avalonia.Tests` 10/10,
  `Config.UI.ConfigService.Tests` 4/4, and `Config.UI.Avalonia.Tests` 6/6.
- The two hosts use the same vendor but distinct application identities,
  `JC-Soft/RnzTrauer` and `JC-Soft/AA98-CodeStudio`; Config.Service derives
  separate persistence roots from those identities.

## Gate and completion
G-Integration passed. Host DI composition and section isolation are covered
without network or database access. The RnzTrauer read-only notice/coordinate
mode remains intentionally separate from its writable configuration capability.

## Version control
- SVN revision: r1894 (`Integrate shared Config UI into application hosts`).

## Handoff
- B-Config-UI is Done.
- T-015 is In Progress.