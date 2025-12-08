# HexaBan_Console

Console front end for the HexaBan hexagonal Sokoban variant (`HexaBan` engine). Renders hex cells using indented ASCII rows and translates keyboard input into engine actions.

## Responsibilities
- Draw current board state & player location
- Accept directional inputs mapped to hex neighbor directions
- Present level preview and status (moves, boxes placed)
- Provide help / instructions dialog

## Direction Mapping (example)
Because hex grids have 6 neighbors, choose a key layout (e.g. numpad, or QWE/ASD) for intuitive movement.

## Extensibility
- Colorize output for better readability
- Add coordinate overlay for debugging
- Implement undo / redo when engine supports it

## Build & Run
```
dotnet build HexaBan_Console/HexaBan_Console.csproj
```

## License
Internal console UI sample.
