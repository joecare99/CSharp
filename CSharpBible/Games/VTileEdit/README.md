# VTileEdit

Visual Tile Editor for defining and editing multi-line console tile glyphs and color attributes used across several console games in this repository.

## Objectives
- Provide an interactive way to craft tile art (ASCII / extended chars)
- Manage foreground/background color pairs per character cell
- Export tile definitions for embedding into engine libraries

## Core Features (intended)
- Tile selection & preview
- Character grid editing with color picking
- Copy / paste between tiles
- Save / load tile set definitions (custom format or JSON)

## Integration
Edited tiles can be fed into rendering subsystems (e.g. Snake, Tetris, Sokoban variants) for richer visuals without manual array editing.

## Extending
- Add animation frames per tile (for simple sprite animation)
- Palette management & bulk recolor tools
- Undo / redo stack

## Build
```
dotnet build VTileEdit/VTileEdit.csproj
```

## License
Internal tooling sample.
