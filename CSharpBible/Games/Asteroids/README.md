# Asteroids (Classic Console Variant)

Console front end (`Asteroids_net` or similar) that renders the classic Asteroids gameplay using characters / simple shapes based on the underlying engine (`Asteroids_Base`).

## Responsibilities
- Map engine entity positions to console coordinates
- Handle keyboard input (thrust, rotate, fire, hyperspace if present)
- Display score, lives, wave indicators
- Basic frame timing loop

## Differences From Modern Engine Front End
This variant is intentionally minimalistic, focusing on raw gameplay with minimal menus or meta systems.

## Running
```
dotnet build Asteroids/Asteroids_net.csproj
dotnet run --project Asteroids/Asteroids_net.csproj
```

## Controls (example)
| Key | Action |
|-----|--------|
| Left / Right | Rotate ship |
| Up | Thrust |
| Space | Fire |
| H | Hyperspace (if implemented) |
| Esc / Q | Quit |

## Extending
- Add simple text HUD animations
- Integrate sound (system beep or external library wrapper)
- Add color-coded danger indicators near screen edges

## License
Internal arcade sample.
