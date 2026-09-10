# T-011 Inventory RnzTrauer Config UI

## Parent
B-Config-UI | Previous: T-010 | Next: T-012

## Status
Planned

## Request
Locate and analyze `RnzTrauer.Avalonia.Config` and its tests before changing code. Record project path, public API, views, ViewModels, DI, models, UI strings, dependencies, and test coverage; classify each element as shared, host adapter, or product-specific.

## Planned tests
- Run existing component test suite on net8.0.
- Reference audit identifies every RnzTrauer-specific dependency.
- Existing UI commands and persistence behavior receive baseline scenarios.

## Gate and completion
A documented dependency matrix is mandatory before extraction. Before Done: test evidence, English SVN commit/revision if artifacts change, current/next/backlog/feature status update.