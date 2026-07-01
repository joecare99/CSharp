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

## Validation
- Run host build and planning host smoke scenario.

## Status
- Completed
