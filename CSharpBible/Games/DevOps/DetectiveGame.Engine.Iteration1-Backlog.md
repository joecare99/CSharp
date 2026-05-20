# DetectiveGame.Engine Iteration 1 Backlog

## Iteration Goal
Stabilize the current deduction core in `DetectiveGame.Engine` before adding room movement or richer gameplay variants.

## Backlog Overview

### Task 1: Validate acting player
**Description**
Ensure that only the current active player can perform gameplay actions.

**Scope**
- validate `askingPlayerId` in `MakeSuggestion`
- validate `playerId` in `MakeAccusation`
- reject actions from inactive players
- reject actions after game finished

**Acceptance Criteria**
- non-current players cannot suggest
- non-current players cannot accuse
- inactive players cannot act
- finished games reject further actions with explicit failure

---

### Task 2: Correct private knowledge ownership
**Description**
Fix card reveal handling so the asking player, not the refuting player, receives the revealed information.

**Scope**
- move revealed card tracking to the asking player
- keep public history separate from private knowledge
- do not leak revealed card into globally visible state unless explicitly intended

**Acceptance Criteria**
- the asking player stores the seen card
- the refuting player does not gain seen knowledge from their own reveal
- suggestion history can remain public without exposing private card detail globally

---

### Task 3: Remove reflection-based state mutation
**Description**
Replace reflection-based access to the internal player list with an explicit domain-friendly setup path.

**Scope**
- introduce controlled state construction for players
- keep consumer-facing player access read-only
- remove reflection helper from setup logic

**Acceptance Criteria**
- no reflection is used in game setup
- state remains encapsulated
- setup remains simple and deterministic

---

### Task 4: Add basic gameplay validation
**Description**
Add core validation rules for setup and action methods.

**Scope**
- minimum player count
- non-empty player names
- unique player names
- valid player ids
- correct card categories for person, weapon, and room

**Acceptance Criteria**
- invalid setup input fails early
- invalid action input fails early
- category mismatches are rejected explicitly

---

### Task 5: Expand automated tests
**Description**
Add MSTest coverage for the corrected gameplay behavior.

**Scope**
- current player enforcement
- refutation order
- private revealed knowledge
- wrong accusation deactivates player
- next turn skips inactive players
- finished game blocks actions

**Acceptance Criteria**
- tests cover all iteration 1 rules
- existing setup tests continue to pass
- tests remain focused on engine behavior, not UI concerns

## Suggested Work Order
1. remove reflection-based setup path
2. add validation for current player and inputs
3. fix revealed knowledge ownership
4. add or update MSTest coverage
5. verify build and tests

## Out of Scope
The following items stay out of iteration 1:

- board movement
- room reachability rules
- advanced turn phase state machine
- configurable game variants
- AI behavior
- UI-facing summaries

## Definition of Done
- engine logic matches the iteration 1 acceptance criteria
- relevant tests pass
- no reflection remains in the current setup implementation
- public and private knowledge handling is no longer mixed