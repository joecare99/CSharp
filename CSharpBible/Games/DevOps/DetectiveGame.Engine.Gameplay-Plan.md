# DetectiveGame.Engine Gameplay Plan

## Goal
Refine the gameplay flow for `DetectiveGame.Engine` before further implementation changes.

## Current State
The current engine already supports a minimal round flow:

- create a new game with players
- choose a hidden solution
- distribute remaining cards fairly
- make a suggestion
- let the first matching player refute
- make an accusation
- eliminate a player after a wrong accusation
- advance to the next active player

## Observed Gaps
The current implementation is intentionally simple, but the gameplay model is still incomplete:

1. `MakeSuggestion` uses `CurrentPlayerIndex` instead of reliably validating `askingPlayerId`.
2. Revealed knowledge is stored on the refuting player (`SeenCards`) although the asking player should receive that information.
3. There is no explicit turn phase model.
4. There is no room or movement model yet.
5. There is no distinction between public information and private player knowledge.
6. Suggestions and accusations are not yet constrained by turn rules.
7. There is no structured game result or round event log beyond raw suggestion history.
8. Setup uses reflection to mutate the player list, which should be replaced by explicit state construction.

## Planned Gameplay Scope
The engine should model a clear detective round loop with explicit responsibilities.

### Backlog Item 1: Define turn lifecycle
A turn should be modeled in ordered phases:

1. turn starts for current active player
2. optional movement or room selection happens
3. player may make a suggestion if allowed
4. refutation is resolved clockwise
5. player may optionally make an accusation
6. turn ends
7. next active player becomes current player

Acceptance ideas:

- only the current active player can act
- a finished game rejects further actions
- illegal actions fail with explicit validation

### Backlog Item 2: Separate private and public knowledge
The engine should distinguish:

- cards in a player hand
- cards revealed privately to one player
- public history visible to all players

Acceptance ideas:

- only the asking player receives the revealed card
- all players can still know who refuted a suggestion if rules require that
- history stores public outcome without leaking hidden card data globally

### Backlog Item 3: Model suggestion resolution explicitly
Suggestion handling should become a structured engine operation.

Suggested result model:

- asking player id
- suggested person, weapon, room
- checked players in order
- refuting player id if any
- revealed card if any and only in private result context
- whether suggestion was unrefuted

Acceptance ideas:

- refutation starts with the next active player clockwise
- inactive players are skipped
- the first eligible refuter stops the search

### Backlog Item 4: Model accusation resolution explicitly
Accusations should be first-class game actions.

Acceptance ideas:

- a correct accusation finishes the game and sets the winner
- a wrong accusation removes the player from active turn rotation
- if one active player remains, that player wins automatically only if that rule is intended for this variant
- the automatic-win rule should be configurable or documented

### Backlog Item 5: Decide room and movement rules
This is the main open design decision for the engine.

Options:

- **Minimal mode**: room is just a selected context value for suggestions
- **Board-aware mode**: movement and reachable rooms are modeled in the engine

Recommendation for now:

Start with **Minimal mode** so the deduction loop is stable before introducing pathfinding or board movement.

Acceptance ideas:

- the current player has a current room or selected room context
- suggestions require a valid room context
- movement rules can be added later without breaking core deduction services

### Backlog Item 6: Add explicit validation layer
The engine should validate inputs before changing state.

Validation examples:

- minimum player count
- unique non-empty player names
- player id must exist
- only cards of correct categories can be passed
- only current active player may suggest or accuse
- no actions after game finished

### Backlog Item 7: Replace reflection-based state mutation
`GameState` setup currently uses reflection to access `_players`.

Recommendation:

- expose controlled internal mutation through constructor or internal methods
- keep read-only access for consumers
- avoid reflection in domain setup

### Backlog Item 8: Expand tests around gameplay rules
Tests should cover the intended game loop.

Priority tests:

1. current player validation
2. refutation order with skipped inactive players
3. revealed card goes only to asking player knowledge
4. wrong accusation deactivates player
5. next turn skips inactive players
6. finished game rejects new actions
7. fair card distribution and hidden solution remain correct

## Proposed Iteration Order

### Iteration 1
Stabilize the current deduction core.

- define turn ownership rules
- fix private knowledge handling
- remove reflection-based state mutation
- add validation and tests

### Iteration 2
Introduce explicit action/result models.

- structured suggestion result
- structured accusation result
- public vs private history separation

### Iteration 3
Introduce room context.

- current room in state
- suggestion preconditions based on room
- optional movement abstraction

### Iteration 4
Optional advanced rules.

- board movement
- configurable variant rules
- richer AI or UI-facing summaries

## Recommended Immediate Next Step
Focus next on **Iteration 1**.

Concrete first implementation target:

1. validate current player identity in `MakeSuggestion` and `MakeAccusation`
2. move revealed knowledge from the refuting player to the asking player
3. replace reflection-based player mutation with an explicit state construction approach
4. add MSTest coverage for the corrected behavior
