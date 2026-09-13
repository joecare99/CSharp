# E-Shared-CodeStudio-Components

## Status
Done

## Goal
Deliver a component-based shared foundation for RnzTrauer, AA98_AvlnCodeStudio, and later ConsoleLib.Cxaml.Designer.

## Scope
- Product-neutral configuration core and Avalonia configuration UI.
- Project explorer, property editor, and diagnostics jump-to-code capabilities.
- Separate production and MSTest projects for every component.

## Out of scope
- ConsoleLib AXaml preview rendering, control mapping, toolbox, hit-testing, and AXaml write-back.
- Planning/DevOps explorer domain, Git workflow, and product-specific configuration models.

## Architecture constraints
- Non-UI components are UI- and OS-agnostic.
- Components expose capabilities through interfaces, factories, and DI.
- `AA98_AvlnCodeStudio.Base` has no Model dependency.
- Cross-application diagnostics stay in `AppKomponentBaseLib`.
- UI text remains localized at the UI or registration boundary.

## Children
- F-Shared-CodeStudio-Components

## Completion
T-001 through T-022 are complete. The delivered shared components have passed
their architecture, quality, integration, and release-readiness gates.

## Completion gate
All child backlog items have passed their quality and integration gates, have
recorded test evidence and English SVN commits, and have consistent final
work-item status.