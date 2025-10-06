# Snake_Console

Classic Snake game implementation for the terminal / console. Multi-targeted for .NET Framework and modern .NET to showcase cross?TFM console rendering, pluggable game loop timing and MVVM style view model separation found in `Snake_Base`.

## Purpose
Demonstrates how to build a simple real?time console game using:
- A reusable game engine (`Snake_Base`)
- A thin console UI layer (this project)
- Observable state changes for rendering

## Main Features
- Deterministic snake movement with configurable tick speed
- Direction input and graceful quit handling
- Growing tail & collision detection (self + walls)
- Score / level progression hooks (extend in engine)
- Multi target frameworks (legacy + modern)

## Architecture
```
Snake_Console (UI)
  ? depends on Snake_Base (engine logic, models)
  ? uses Game_Base (shared abstractions) where applicable
```
The console project only concerns itself with:
- Translating key presses into engine `UserAction`
- Rendering the tile buffer (via ConsoleDisplay / custom drawing)
- Presenting score / status lines

## Build & Run
```
dotnet build Snake_Console/Snake_Console.csproj -c Release
dotnet run   --project Snake_Console/Snake_Console.csproj
```
On Windows you may alternatively start the framework specific build artifact.

## Controls (default)
| Key | Action |
|-----|--------|
| Arrow Keys / WASD | Move snake |
| Esc / Q | Quit |
| P | (optional) pause extension point |

## Extending
- Add new food types: extend enum + rendering map in engine, adapt scoring.
- Add obstacles: inject obstacle layout before game start.
- Add high score persistence: implement a simple repository and hook into game over event.

## Testing
Logic is unit tested inside engine libraries; UI layer is intentionally minimal. Add integration tests by simulating key sequences against the game loop.

## License
Internal sample (no explicit OSS license provided here). Adjust as needed.
