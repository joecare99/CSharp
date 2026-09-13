# F-Shared-CodeStudio-Components

## Parent
E-Shared-CodeStudio-Components

## Status
Done

## Current handoff
- `B-Config-Core`: Done.
- `B-Code-Navigation`: Done after T-007 and G-Component-Quality.
- `B-Diagnostics-Jump-To-Code`: Done after T-022 and G-Integration.
- `B-Property-Editor`: Done after T-008 through T-010 and
  G-Component-Quality; the canonical contracts, reusable Avalonia UI, and
  host-independent fixture validation are complete.
- `B-Config-UI`: Done. T-011 through T-014 delivered the greenfield
  boundary, Config.Service adapter, reusable Avalonia UI, and isolated
  RnzTrauer/CodeStudio DI integration.
- `B-Project-Explorer`: Done. T-015 through T-017 delivered neutral
  contracts, a filesystem source, Avalonia UI, and CodeStudio navigation
  integration.
- `B-Designer-Consumption`: Done. T-018 established linked canonical
  Property.Editor consumption in the CXAML designer; T-019 passed release
  readiness.
- `AppKomponentBaseLib`: shared base for component interfaces and base classes.

## Value
Applications consume shared, independently testable components instead of duplicating configuration, explorer, property editing, or source-navigation logic.

## Components
1. `Config.Service` configuration core.
2. `Config.UI` reusable Avalonia configuration UI.
3. Project Explorer.
4. Property Editor.
5. Diagnostics and Jump-to-Code.

## Dependency order
1. Configuration core discovery and integration.
2. Code-location contracts and editor navigation.
3. Diagnostics jump-to-code integration.
4. Property contracts and UI.
5. Configuration UI.
6. Project Explorer.
7. AXaml designer consumption preparation.

## Feature gates
- G-Architecture: boundaries and public contracts approved.
- G-Component-Quality: build, MSTest, edge/error/regression tests and documentation evidence.
- G-Integration: hosts consume components through DI without copied logic.
- G-Release-Readiness: every task records tests, SVN commit, and current/next status.

## Children
B-Config-Core; B-Code-Navigation; B-Diagnostics-Jump-To-Code; B-Property-Editor; B-Config-UI; B-Project-Explorer; B-Designer-Consumption

## Completion
Done. Every child backlog item is Done and all required gate evidence is linked.