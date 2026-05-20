# SH-BI0604 - Implement Initial 3D View

## Work Item Type
Backlog Item

## Parent
`SH-F06 - UI and Player Experience`

## Description
Create the first 3D SharpHack view while keeping grid-based gameplay authoritative and UI-neutral.

## Scope
- Prefer OpenGL initially, but select the concrete .NET-compatible 3D stack after comparing practical integration options.
- Use a hybrid approach where `SharpHack.Engine` and the grid map remain authoritative.
- Implement tile-to-3D extrusion for the first 3D slice.
- Render engine-generated room function and decoration metadata.
- Treat decoration as passive visual content in the first slice.
- Preserve decoration metadata so later iterations can add NetHack-like interaction with many or most decoration objects.

## Product Decision
- 3D is the preferred first polished UI direction if practical.
- OpenGL is the initial rendering preference, but better practical alternatives may be selected if they fit .NET/WPF integration better.
- Gameplay logic must remain outside the 3D renderer.
- Decoration starts visually decorative and becomes interactable in later slices.

## Acceptance Criteria
- A first 3D view can render a generated map from engine data.
- Walls, floors, doors, stairs, items, creatures, room functions, and decoration metadata can be mapped to 3D representations.
- The 3D renderer does not own gameplay rules.
- The initial 3D mapping is testable through renderer-independent mapping contracts.

## Child Tasks
- `SH-T060401 - Select 3D Rendering Stack`
- `SH-T060402 - Create Tile To 3D Prototype`
- `SH-T060403 - Render Engine Decoration In 3D`
- `SH-T060404 - Test 3D Mapping Contracts`
