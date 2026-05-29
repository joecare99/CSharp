# AA98-F01 Main Window and Shell Regions

## Parent
- Epic: `DevOps/Epics/AA98-E01-Workbench-and-Shell.md`
- Vision: `DevOps/Vision.md`

## Goal
Define the first stable main window structure for `AA98_AvlnCodeStudio` so users start in a clear, usable workbench with predictable regions for commands, editors, and later tool windows.

## Scope
- Define the main application window structure.
- Establish the primary shell regions.
- Reserve clear hosting areas for editor content and future tool components.
- Ensure the shell remains simple enough for the first usable increments.

## Included
- Main window composition
- Primary visual regions
- Editor host region
- Reserved tool region placeholders
- Region responsibilities and boundaries

## Excluded for Now
- Full docking behavior
- Persisted layout customization
- Advanced window management scenarios
- Rich workspace navigation behavior

## Success Indicators
- The application opens into a stable and understandable shell.
- The main regions are clear enough for users and for follow-up implementation work.
- Later editor and tool-window features can attach without redesigning the main window.

## Candidate Backlog Items
- Define shell region model and responsibilities
- Implement first main window layout with editor host
- Add placeholder regions for future tool windows
- Align shell layout with MVVM-friendly composition

## Assumptions
- The first shell should prioritize clarity over flexibility.
- Early layout decisions should prepare for later docking support without forcing it immediately.

## Open Questions
- Which regions must already be visible in the first increment?
- Should the first version include a dedicated navigation/sidebar region or only reserve space for it?

## Status
- Proposed
