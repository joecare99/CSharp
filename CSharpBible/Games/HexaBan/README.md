# HexaBan

Hex-based variation of a Sokoban style puzzle. Player pushes boxes over a hexagonal grid to designated goal cells.

## Distinctive Aspects
- Hex coordinate system (axial or cube coordinates internally)
- Movement & adjacency differ from square grid Sokoban
- Level previews rendered as ASCII hex layouts

## Engine Highlights
- Board representation using `HexCoord`
- Goal / box / player tile distinction
- Validation of legal pushes (one box, free destination)
- Win detection when all goals occupied

## Extensibility
- Weighted tiles (movement cost) for scoring variants
- Multi-box simultaneous push (optional mode)
- Undo stack & solution replay

## Build
```
dotnet build HexaBan/HexaBan.csproj
```

## Running
```
dotnet run --project HexaBan/HexaBan.csproj
```

## License
Internal puzzle engine sample.
