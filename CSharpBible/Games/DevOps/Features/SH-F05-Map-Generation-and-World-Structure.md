# SH-F05 - Map Generation and World Structure

## Work Item Type
Feature

## Parent
`SH-E01 - Complete SharpHack as a Playable Roguelike`

## Value
Generated dungeons become reliable, connected, and interesting across many seeds.

## Scope
- Standardize the active default map generator.
- Add generated-map invariant tests.
- Add themed level features.
- Generate room functions as engine/world data.
- Generate decoration as engine/world data that can be used by terminal, console, WPF, and 3D variants.

## Child Backlog Items
- `SH-BI0501 - Standardize Active Map Generator`
- `SH-BI0502 - Add Generated Map Invariants`
- `SH-BI0503 - Add Themed Level Features`

## Acceptance Criteria
- Console and WPF2D use the same default generator through shared setup.
- Generated levels satisfy reachability and door-validity invariants.
- At least two non-basic level features can spawn without breaking connectivity.
- Rooms can receive functions that influence decoration and gameplay expectations.
- Decoration data is available outside the 3D renderer.

## Open Questions
- Should `BSPMapGenerator` or `BSPRoomMazeMapGenerator` become the primary generator?
- Which room functions are required for the first playable content slice?
- Which decoration metadata is gameplay-relevant and which is purely visual?
