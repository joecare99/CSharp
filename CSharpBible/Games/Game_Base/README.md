# Game_Base

Shared abstractions and base utilities used across multiple mini game engines in this repository.

## Purpose
Centralize common patterns so each individual game project (Snake, Tetris, Sokoban, etc.) can focus on its domain.

## Provided Elements (typical)
- Base interfaces for game loops / tick handling
- Common model primitives (points, sizes, directions) where framework neutral
- Reusable helper utilities (timing, randomization wrappers)
- Event contracts for view model or renderer notification

## Design Principles
- Dependency Inversion: engines depend on abstractions here, not on concrete UIs
- Lightweight: avoid heavy dependencies to keep portability (targets multiple TFMs)
- Testable: keep logic free of direct console or framework calls

## Usage Pattern
Other engines reference `Game_Base` and implement the interfaces defined here. UI layers then consume those engines without duplicating boilerplate.

## Build
```
dotnet build Game_Base/Game_Base.csproj
```

## Extending
Add only generally applicable concepts. If a concept is strongly domain specific keep it inside that game engine.

## License
Internal sample module.
