# Snake_Base

Core engine library for the console Snake game. Provides the domain model, update loop hooks and tile abstraction independent from any specific rendering front end.

## Goals
- Clean separation between game logic and presentation
- Deterministic, easily testable update flow
- Support for various front ends (console, potential GUI)

## Key Components
- `ISnakeGame` interface & implementation encapsulating state
- Direction & user action handling
- Snake body management (linked collection of segments)
- Food spawning and growth rules
- Collision detection (self + boundaries)
- Events for partial vs full redraw (`visUpdate`, `visFullRedraw`)

## Update Cycle
```
Input -> Validate -> Apply Direction -> Advance Head -> Handle Growth -> Detect Collision -> Emit Events
```

## Extensibility Points
- Replace random food generator (inject an RNG or deterministic sequence)
- Add special items: extend tile / enum set and handle consumption rule
- Persist or serialize game state for replays

## Testing Strategy
Unit tests target movement logic, collision detection and growth. Visual tests are handled in higher level console test harnesses.

## Consuming Projects
- `Snake_Console` provides the UI
- Other sample consoles can reuse this engine

## Build
```
dotnet build Snake_Base/Snake_Base.csproj
```

## License
Internal educational sample.
