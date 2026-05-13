# SH-BI0201 - Implement Save Game Model

## Work Item Type
Backlog Item

## Parent
`SH-F02 - Durable Persistence`

## Description
Define versioned serializable save-game DTOs for complete run state.

## Scope
- Capture run state, player, map, tiles, items, enemies, equipment, explored tiles, and level metadata.
- Avoid UI-specific types.
- Include save-version metadata.
- Design the save payload for compressed JSON storage.
- Include metadata needed for recovery, validation, and compatibility checks.

## Product Decision
- The save format should be compressed JSON plus recovery support.
- The uncompressed logical representation should remain JSON-compatible for debugging and future migration.

## Acceptance Criteria
- Save DTOs capture all gameplay state needed to resume a run.
- DTOs are independent from WPF and console concerns.
- DTOs contain version and recovery metadata.
- Round-trip serialization is covered by tests.

## Child Tasks
- `SH-T020101 - Define Save DTOs`
- `SH-T020102 - Map Domain State To Save DTOs`
- `SH-T020103 - Map Save DTOs To Domain State`
- `SH-T020104 - Test Save Model Round Trip`

## Status
Completed

## Status
In Progress
