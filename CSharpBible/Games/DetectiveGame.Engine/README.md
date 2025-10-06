# DetectiveGame.Engine

Core engine for a Cluedo / deduction style detective game. Manages secret solution selection, card distribution, suggestion & accusation resolution and turn order.

## Mechanics
- Random selection of person, weapon, room cards as hidden solution
- Even distribution of remaining cards to players
- Suggestion flow: clockwise refutation search, first matching card revealed (simplified global visibility currently)
- Accusation: correct => game ends, incorrect => player eliminated (inactive)
- Automatic win if only one active player remains

## Data Structures
- `GameState`: players, solution, history, current index
- `Player`: id, name, hand, seen cards, active flag
- `CaseSolution`: person, weapon, room
- `Suggestion`: asking player id, chosen triplet, refuting player id, revealed card

## Services
- `GameService` implements creation & action APIs (`CreateNew`, `MakeSuggestion`, `MakeAccusation`, `NextTurn`)

## Extending
- Add movement on a board vs abstract turns
- Track which player actually saw the revealed card (current simplification is global add)
- Support notes / deduction sheets
- Add AI players with probability reasoning

## Testing
Add deterministic tests by supplying a fixed seed to `CreateNew`. Validate distribution, refutation logic and elimination rules.

## Build
```
dotnet build DetectiveGame.Engine/DetectiveGame.Engine.csproj
```

## License
Internal game engine sample.
