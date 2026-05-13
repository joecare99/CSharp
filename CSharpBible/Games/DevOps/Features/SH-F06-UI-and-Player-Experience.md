# SH-F06 - UI and Player Experience

## Work Item Type
Feature

## Parent
`SH-E01 - Complete SharpHack as a Playable Roguelike`

## Value
Players understand controls, status, choices, and outcomes.

## Scope
- Add shared application orchestration.
- Improve HUD and message surfaces.
- Complete inventory UI flows.
- Keep the UI target range open across terminal, console, WPF top view, WPF/3D first-person, and fully immersive 3D.
- Prepare future 3D views for procedurally generated mappings.
- Prefer 3D as the first polished UI direction if practical.
- Use a hybrid approach for the first 3D slice: grid-based gameplay remains authoritative, while the 3D view maps engine state through tile-to-3D extrusion.
- Plan later evolution from tile extrusion to prefab and optimized mesh mapping.
- Use OpenGL as the initial rendering preference unless a better practical .NET-compatible stack is selected during refinement.

## Child Backlog Items
- `SH-BI0601 - Add Shared Application Orchestration`
- `SH-BI0602 - Improve HUD and Message Surfaces`
- `SH-BI0603 - Complete Inventory UI`
- `SH-BI0604 - Implement Initial 3D View`

## Acceptance Criteria
- Core game setup is shared instead of duplicated across UI projects.
- Players can discover controls and understand death, victory, and objective state.
- Inventory can be managed manually in console and selected WPF UI.
- UI-specific polish can be prioritized without blocking the shared engine and ViewModel contracts.
- The first 3D UI can render existing engine map, room-function, decoration, item, and creature metadata without owning gameplay rules.
- The first 3D UI can treat decoration as passive visuals while preserving metadata for later interaction.

## Open Questions
- Which concrete .NET-compatible OpenGL or alternative 3D stack should be used first?
- Which parts of room decoration need interactive UI affordances versus passive visual display?
