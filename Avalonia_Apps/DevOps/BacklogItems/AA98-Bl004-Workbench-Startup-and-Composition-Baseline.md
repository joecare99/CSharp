# AA98-Bl004 Workbench Startup and Composition Baseline

## Parent
- Feature: `DevOps/Features/AA98-F03-Workbench-Startup-and-Composition.md`
- Epic: `DevOps/Epics/AA98-E01-Workbench-and-Shell.md`
- Vision: `DevOps/Vision.md`

## Scope
Establish the first reliable startup and composition baseline for the `AA98_AvlnCodeStudio` workbench so the shell starts consistently, wires its initial services through dependency injection, and remains ready for later component growth.

## Goals
- Define the startup sequence for the first workbench shell.
- Register the initial shell-related services, view models, and views in a clear composition path.
- Separate shell composition concerns from editor-specific composition as far as practical in the current increment.
- Prepare explicit extension seams for later component registration without introducing plugin complexity too early.

## Assumptions
- Early startup and composition should stay explicit and easy to understand.
- Internal composition seams are sufficient before external plugin or dynamic discovery scenarios are introduced.
- Dependency injection should be used consistently to support testability and later extension.

## Open Questions
- Which composition responsibilities belong in the application bootstrap versus the shell-specific layer?
- How early should component registration be abstracted beyond explicit startup wiring?

## Status
- Proposed
