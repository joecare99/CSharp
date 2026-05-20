# SH-F04 - Combat AI and Magic Depth

## Work Item Type
Feature

## Parent
`SH-E01 - Complete SharpHack as a Playable Roguelike`

## Value
Encounters become tactically interesting rather than simple bump combat.

## Scope
- Replace string-only combat feedback with structured combat results.
- Upgrade enemy AI with pathfinding and perception.
- Implement the first gameplay slice in `SharpHack.Magic`.

## Child Backlog Items
- `SH-BI0401 - Replace Simple Combat With Structured Combat Results`
- `SH-BI0402 - Upgrade Enemy AI`
- `SH-BI0403 - Implement First Magic Slice`

## Acceptance Criteria
- Combat behavior can be tested without parsing UI messages.
- Enemy movement handles obstacles and basic perception rules.
- At least one spell can be used through core game logic and surfaced by ViewModels.

## Open Questions
- Which combat randomization rules should be used for the first complete version?
- Which initial spell best supports the current roguelike loop?
