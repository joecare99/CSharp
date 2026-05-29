# AA98-F02 Application Command Surfaces

## Parent
- Epic: `DevOps/Epics/AA98-E01-Workbench-and-Shell.md`
- Vision: `DevOps/Vision.md`

## Goal
Provide the first command surfaces for the workbench so core actions are discoverable and the shell can later accept command contributions from components.

## Scope
- Define the initial menu structure.
- Define a first toolbar or equivalent quick-action surface.
- Prepare command contribution points for later components.
- Keep the first command model small, understandable, and extensible.

## Included
- Menu baseline
- Toolbar or quick-action baseline
- Command grouping rules
- Contribution-oriented command architecture

## Excluded for Now
- Full command customization
- Context-sensitive command routing across all future components
- Keyboard shortcut editor
- Complex command palettes

## Success Indicators
- Users can discover and trigger the most important shell and editor actions.
- The command surfaces support later extension without structural rework.
- Core commands can be exposed consistently across shell regions.

## Candidate Backlog Items
- Define baseline menu groups for file and view workflows
- Define initial toolbar strategy for primary actions
- Introduce command contribution contracts for shell-level actions
- Connect implemented editor actions to visible shell commands

## Assumptions
- A classic IDE-inspired menu model is the safest first step.
- The command model should support later contributions from editor, workspace, and settings components.

## Open Questions
- Should the first increment provide both menu and toolbar, or only one of them?
- How early should keyboard shortcuts become part of the feature scope?

## Status
- Proposed
