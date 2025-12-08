# Galaxia_Base

Strategic space simulation / 4X style core library. Manages star systems, corporations/factions, resources, expansion and exploration data structures.

## Domain Concepts
- Star Systems: location, resources, ownership
- Corporations / Factions: name, color, assets, home system
- Fleet / expansion placeholders (extendable)
- Turn or tick based progression hooks

## Architecture
`Galaxia_Base` exposes plain C# models & services; UI (e.g. `Galaxia_UI`) renders galaxy map, handles user commands and invokes engine services.

## Extending
- Add technology tree & research progression
- Implement AI expansion heuristics
- Diplomacy & trade modules
- Persistence layer (save / load galaxy)

## Testing
`Galaxia_BaseTests` validate creation rules, ownership transitions and resource calculations.

## Build
```
dotnet build Galaxia_Base/Galaxia_Base.csproj
```

## License
Internal strategy engine sample.
