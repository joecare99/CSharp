# Werner_Flaschbier

A classic style platform / arcade puzzle hybrid game implementation. Consists of a base engine (`Werner_Flaschbier_Base`) and one or more console front ends.

## Concept
Navigate the character through levels, avoid hazards, interact with the tile based world and reach objectives. Emphasis on timing and tile interaction.

## Modules
- `Werner_Flaschbier_Base`: Game rules, level data, state transitions
- Console UI: Renders tiles, handles keyboard controls

## Features (Engine Level)
- Tile map with multiple tile categories (solid, hazard, collectible, etc.)
- Player state (position, lives, score)
- Basic physics / gravity tick (if applicable) or stepwise movement
- Event hooks for rendering updates

## Possible Extensions
- Add enemy AI behaviors
- Expand tile types (moving platforms, switches)
- Add save / load of progress
- Implement scripting for level events

## Build & Run
```
dotnet build Werner_Flaschbier/Werner_Flaschbier_Console.csproj
```
Run the produced executable or:
```
dotnet run --project Werner_Flaschbier/Werner_Flaschbier_Console.csproj
```

## Controls (example)
| Key | Action |
|-----|--------|
| Arrow keys | Move |
| Space | Jump / action |
| Q / Esc | Quit |

Adjust actual mapping according to implemented input layer.

## Testing
Unit tests reside in `Werner_Flaschbier_BaseTests` covering logic (collision, scoring). Rendering tests rely on snapshot comparisons in console stubs.

## License
Internal sample.
