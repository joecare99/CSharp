# SH-BI0103 - Add Restart and New Run Flow

## Work Item Type
Backlog Item

## Parent
`SH-F01 - Core Game Loop Completion`

## Description
Allow players to start a new run without restarting the application or leaking previous session state.

## Scope
- Add a clean session factory abstraction.
- Support creating a new run from console and WPF-facing flows.
- Share setup across UI entry points.
- Support both non-permadeath/reloadable and permadeath run variants.
- In permadeath mode, support starting a new character in the same dungeon layout.
- In same-dungeon permadeath replay, place a gravestone at the previous death location.
- Preserve part of the previous character's items at the gravestone when they have not rotted or been stolen.

## Product Decision
- Both play variants are desired: reloadable/non-permadeath and permadeath.
- Permadeath should still support continuity through same-dungeon replay with a new character.
- Grave item recovery should be partial and time-sensitive through rot or theft rules.

## Acceptance Criteria
- Console and WPF2D can start a new run after death or victory.
- New runs do not leak state from previous runs.
- Permadeath restart can reuse the same dungeon layout with a new character.
- A gravestone can mark the previous character's death location.
- Recoverable previous-character items can be represented separately from rotted or stolen items.
- Tests cover factory-created run defaults.

## Child Tasks
- `SH-T010301 - Create Shared Session Factory`
- `SH-T010302 - Add New Run Commands`
- `SH-T010303 - Reset UI State On New Run`
- `SH-T010304 - Test New Run Flow`

## Status
Completed
