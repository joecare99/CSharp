# Tetris_Base

Core logic for a Tetris style falling block puzzle game. Supplies piece definitions, board state management and line clearing logic independent from presentation.

## Features
- Tetromino (or custom block) definitions with rotation logic
- Well / playfield representation and collision tests
- Line detection & clearing with scoring hook
- Gravity tick & soft drop extensibility
- Events to differentiate incremental vs full redraw

## Architecture
```
UI (Console / Future GUI)
  -> Tetris_Base (engine)
    -> Game_Base (shared abstractions)
```
Engine exposes pure operations; rendering is performed externally.

## Rotation System
Implements a basic rotation (no advanced wall kicks by default). You can extend by injecting a rotation policy service.

## Extensibility
- Add hold / next queue: extend state, emit events
- Ghost piece: compute lowest non-colliding y for active block
- Fancy scoring: implement combo / T-spin detection in a scoring service

## Testing
Include tests in corresponding test project for rotation correctness, line clearing, and collision edge cases.

## Build
```
dotnet build Tetris_Base/Tetris_Base.csproj
```

## License
Internal educational module.
