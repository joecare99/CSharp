# AA98-T063 Create DevOps Planning Micro Host

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl044-DevOps-Planning-UI-Baseline.md`
- Feature: `../Features/AA98-F39-Component-Micro-Hosts.md`

## Goal
Create a thin micro host for the DevOps planning component.

## Scope
- Host the planning explorer UI with minimal composition.
- Load the repository's local planning model.
- Keep external provider connections out of scope.

## Execution Notes
1. Follow the shared micro-host pattern.
2. Keep runtime state outside `DevOps` planning files.
3. Provide enough diagnostics for invalid planning files.

## Acceptance Criteria
- Planning host can show local planning hierarchy.
- Host remains thin and provider-neutral.

## Delivered
- Added a new thin Avalonia micro host project `AA98.DevOpsPlanning.Host` dedicated to local planning hierarchy browsing.
- Implemented host startup and DI composition with provider-neutral planning services (`IPlanningReader` via `MarkdownPlanningReader`) and a focused host `MainWindowViewModel`.
- Implemented a minimal planning explorer window with hierarchy tree, selected item metadata panel, diagnostics list, and repository/status footer information.
- Added the new host project to `Avalonia_Apps.slnx` so it can be built and run independently from the full AA98 shell.
- Extended planning item loading with raw markdown document text to support editor and preview scenarios.
- Reworked the planning explorer UI with compact tree density, toggle-based Hierarchy/Category views, a dedicated explorer status line, and a split markdown editor/preview detail surface.
- Introduced a general `IHasProperties` / `IPropertyItem` contract and a reusable properties panel concept with read-only and selectively editable fields.

## Validation
- Build succeeded for `AA98_AvlnCodeStudio.Planning.UI` and `AA98.DevOpsPlanning.Host` in `Release` configuration.
- Build in `Debug` for host can fail while the running host process locks output DLLs (`MSB3021` / `MSB3027`).
- Added `PlanningUiExplorerViewModelTests` for hierarchy/category grouping, explorer status text, and property edit behavior.

## Status
- Completed
