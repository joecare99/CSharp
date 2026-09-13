# B-Config-UI

## Parent
F-Shared-CodeStudio-Components

## Status
Done

## Request
Create a product-neutral, reusable Avalonia configuration UI consuming
Config.Service and the shared property editor. T-011 first establishes whether
any existing RnzTrauer UI behavior can be adapted; no nonexistent project is
assumed as an extraction source.

## Open question
`RnzTrauer.Avalonia.Config` does not exist in the current working copy.
T-011 confirmed that `RnzTrauer.Avalonia` and the configuration providers
contain no reusable configuration UI. T-012 is defining a greenfield
component contract from the documented baseline.

## Delivered
- T-012 and T-013 are Done. Config.UI has neutral contracts, a Config.Service
  adapter, a reusable Avalonia view, and dedicated tests.
- T-014 integrated RnzTrauer and CodeStudio through host-owned registrations.
  The hosts consume the shared UI through DI, retain their own provider
  metadata/models, and use isolated Config.Service application identities.

## Acceptance criteria
- Displays registered sections in provider order with localizable display text.
- Supports load, edit, save, reset, busy state, and failure feedback.
- Hosts provide their own registration and localized metadata through DI.

## Tasks
T-011, T-012, T-013, T-014

## Gates
G-Architecture passed after T-012; G-Integration passed after T-014.