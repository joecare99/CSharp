# SH-BI0401 - Replace Simple Combat With Structured Combat Results

## Work Item Type
Backlog Item

## Parent
`SH-F04 - Combat AI and Magic Depth`

## Description
Make combat outcomes explicit and testable without parsing message strings.

## Scope
- Add a combat result model with hit, miss, damage, death, and optional effects.
- Support random damage ranges through injected randomness.
- Keep UI message formatting outside low-level combat where practical.

## Acceptance Criteria
- Combat results are testable without parsing strings.
- Death handling remains consistent for player and enemies.
- Existing combat tests are updated to structured assertions.

## Child Tasks
- `SH-T040101 - Define Combat Result Model`
- `SH-T040102 - Update Combat System Contract`
- `SH-T040103 - Move Combat Message Formatting`
- `SH-T040104 - Test Structured Combat Results`
