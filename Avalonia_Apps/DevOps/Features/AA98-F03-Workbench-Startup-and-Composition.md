# AA98-F03 Workbench Startup and Composition

## Parent
- Epic: `DevOps/Epics/AA98-E01-Workbench-and-Shell.md`
- Vision: `DevOps/Vision.md`

## Goal
Establish the startup composition path for the shell so the application opens reliably, wires the first services through dependency injection, and can evolve toward a component-based workbench.

## Scope
- Define shell startup responsibilities.
- Wire the first shell-related services into dependency injection.
- Define how the main window and workbench parts are composed.
- Prepare a clean path for later component registration.

## Included
- Application startup flow
- Shell service registration
- Main window creation and initialization
- Composition boundaries between shell, editor, and future components

## Excluded for Now
- Dynamic plugin discovery
- Advanced module lifecycle management
- External provider loading
- Full component marketplace or package scenarios

## Success Indicators
- The application starts consistently into the intended workbench.
- Shell composition remains understandable and testable.
- Later components can be integrated through defined composition seams.

## Candidate Backlog Items
- Define startup sequence for shell initialization
- Register shell services and view models with dependency injection
- Separate shell composition from editor-specific composition
- Prepare extension points for future component registration

## Assumptions
- Early composition should remain explicit and local to the application startup path.
- Component extensibility should begin with internal extension points before external plugins are considered.

## Open Questions
- Which shell services belong in the application layer versus the UI layer?
- When should component discovery move from explicit registration to a more flexible model?

## Status
- Proposed
