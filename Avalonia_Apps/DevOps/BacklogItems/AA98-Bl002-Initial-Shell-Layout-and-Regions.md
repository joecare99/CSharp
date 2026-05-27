# AA98-Bl002 Initial Shell Layout and Regions

## Parent
- Feature: `DevOps/Features/AA98-F01-Main-Window-and-Shell-Regions.md`
- Epic: `DevOps/Epics/AA98-E01-Workbench-and-Shell.md`
- Vision: `DevOps/Vision.md`

## Scope
Create the first stable workbench shell layout for `AA98_AvlnCodeStudio` with clearly defined regions for commands, editor content, status information, and future tool hosting.

## Goals
- Introduce a main window structure that is understandable and usable as an IDE-style shell.
- Define explicit regions for menu/toolbar, editor hosting, and status display.
- Reserve extension-friendly placeholder areas for later tool windows or navigation surfaces.
- Keep the first shell layout lightweight and compatible with MVVM-oriented composition.

## Assumptions
- The first usable shell should focus on a single main window.
- Reserved regions may initially be placeholders rather than fully interactive tool windows.
- Full docking and layout persistence are later increments and should not block this step.

## Open Questions
- Should a navigation/sidebar placeholder already be visible in the first implementation?
- Should the first shell prioritize vertical space for the editor area over explicit side regions?

## Status
- Proposed
