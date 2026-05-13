# SH-T010201 - Document Victory Rule Decision

## Work Item Type
Task

## Parent
`SH-BI0102 - Define Victory and Dungeon Depth Rules`

## Goal
Record the first complete-game victory rule before implementation.

## Scope
- Decide target depth and objective type.
- Document assumptions and alternatives.
- Link the decision to related implementation tasks.

## Clarified Decision
- The final victory condition is obtaining the Amulet of JoCarneer.
- Dungeon depth progression, finale structure, and exit behavior must support the Amulet objective instead of replacing it.
- The objective should remain engine-owned so UI layers only display the result.

## Assumptions
- The first complete-game flow can still use the current prototype dungeon structure while the deeper finale is refined later.
- The objective should be reachable through normal gameplay without relying on UI-only triggers.

## Alternatives Considered
- Boss defeat as the sole win condition: rejected because the Amulet objective is the chosen product direction.
- Simple exit-at-depth victory: rejected because it does not preserve the artifact-driven end goal.

## Related Tasks
- `SH-T010202 - Implement Victory Trigger`
- `SH-T010203 - Surface Victory Summary Data`
- `SH-T010204 - Test Victory Rules`

## Done
- Victory decision is documented in DevOps.
- The Amulet of JoCarneer is recorded as the complete-game objective.
- Open questions are updated or closed.

## Status
Completed
