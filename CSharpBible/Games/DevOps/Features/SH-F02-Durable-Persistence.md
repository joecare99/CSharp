# SH-F02 - Durable Persistence

## Work Item Type
Feature

## Parent
`SH-E01 - Complete SharpHack as a Playable Roguelike`

## Value
Players can save and resume a run reliably.

## Scope
- Define versioned save-game DTOs.
- Implement durable file-backed persistence.
- Add save/load UI flows.

## Child Backlog Items
- `SH-BI0201 - Implement Save Game Model`
- `SH-BI0202 - Implement File Backed GamePersist`
- `SH-BI0203 - Add Save Load UI Flows`

## Status
In Progress

## Acceptance Criteria
- Gameplay state can be saved and loaded across process restarts.
- Save files have version metadata.
- Corrupt or unsupported saves fail with clear diagnostics.
- Console and WPF-facing flows can trigger save/load without leaking UI details into core persistence.

## Open Questions
- Should the first durable save format be JSON for debugging or binary for compactness?
