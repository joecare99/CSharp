# SH-T020103 - Map Save DTOs To Domain State

## Work Item Type
Task

## Parent
`SH-BI0201 - Implement Save Game Model`

## Goal
Reconstruct runtime domain state from save DTOs.

## Scope
- Restore map tiles, items, creatures, player, enemies, equipment, and level metadata.
- Reconnect tile creature references and inventory/equipment references correctly.
- Handle unsupported DTO versions explicitly.

## Done
- DTOs can be restored into playable domain objects.
- Restored maps and creatures have consistent references.

## Status
Completed
