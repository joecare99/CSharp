# AA98-E01 Workbench and Shell

## Parent
- Vision: `DevOps/Vision.md`

## Goal
Establish the application shell as the stable workbench foundation for `AA98_AvlnCodeStudio`, including the main window structure, command surfaces, navigation regions, and the baseline user experience for an extensible IDE-style application.

## Scope
- Define the main shell layout and workbench structure.
- Provide menu, toolbar, status, and editor hosting areas.
- Prepare shell-level extension points for components.
- Support a first user-usable application frame for later editor and tool-window increments.

## Included Themes
- Main window composition
- Shell navigation and command surfaces
- Application startup composition
- Basic shell state and workbench hosting

## Excluded for Now
- Advanced project system behavior
- Full LLM interaction workflows
- Rich debugging or build orchestration

## Success Indicators
- The application starts into a stable and understandable workbench shell.
- Core UI regions are clearly defined for later incremental expansion.
- The shell can host editor and tool components without redesign.

## Candidate Child Features
- Main window layout and shell regions
- Application command framework
- Status and notification surface
- Shell composition and startup wiring

## Assumptions
- The shell should remain lightweight in the first iterations.
- Later docking and layout persistence may extend, but not replace, the shell foundation.

## Open Questions
- Should the first shell expose a classic IDE menu model, a minimal shell model, or both?
- Which shell regions should be mandatory from the first increment onward?

## Status
- Proposed
