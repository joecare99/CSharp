# T-014 Integrate Config UI Hosts

## Parent
B-Config-UI | Previous: T-013 | Next: T-015

## Status
Planned

## Request
Register the shared Config.UI from RnzTrauer and CodeStudio hosts only through DI and provider adapters. Do not duplicate views, ViewModels, storage, or configuration models.

## Planned tests
- Each host sees only registered sections.
- Vendor/application storage roots are isolated.
- Host-localized metadata is displayed and not persisted as core data.
- Provider error in one host does not affect another host fixture.

## Gate and completion
Pass G-Integration. Before Done: integration tests/build, English SVN commit/revision, B-Config-UI Done, T-015 In Progress, feature status update.