# AA98-Bl003 Baseline Menu and Toolbar Commands

## Parent
- Feature: `DevOps/Features/AA98-F02-Application-Command-Surfaces.md`
- Epic: `DevOps/Epics/AA98-E01-Workbench-and-Shell.md`
- Vision: `DevOps/Vision.md`

## Scope
Add the first visible command surfaces for the workbench so users can discover and invoke core shell and editor actions through a baseline menu and an initial toolbar or equivalent quick-action area.

## Goals
- Define a small classic IDE-inspired menu structure for early workflows.
- Expose the most important existing actions through visible command surfaces.
- Create a command organization approach that can later accept contributions from components.
- Keep the first command set small, coherent, and easy to extend.

## Assumptions
- File-related commands are the most important initial actions to expose.
- A minimal toolbar is useful if it reinforces discoverability without adding UI noise.
- The command architecture should support later extension by editor, workspace, and settings components.

## Open Questions
- Should the first implementation include both menu and toolbar, or start with menu only?
- Which commands belong in the first visible set beyond file actions and basic view actions?

## Status
- Proposed
