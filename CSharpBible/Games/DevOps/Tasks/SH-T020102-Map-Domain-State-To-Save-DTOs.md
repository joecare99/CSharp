# SH-T020102 - Map Domain State To Save DTOs

## Work Item Type
Task

## Parent
`SH-BI0201 - Implement Save Game Model`

## Goal
Map runtime domain objects into save DTOs.

## Scope
- Convert map tiles, explored state, items, creatures, player, enemies, equipment, and level data.
- Preserve enough type information to restore known item and creature kinds.
- Avoid serializing transient UI state.

## Done
- Representative session state can be converted to DTOs.
- Mapping handles empty inventories and null equipment.

## Status
Completed
