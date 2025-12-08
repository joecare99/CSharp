# AsteroidsModernEngine

Modernized variant of the classic Asteroids engine with additional architectural patterns (screen manager, input abstraction, event driven state changes).

## Enhancements Over Base
- Title / menu / options screen state machine
- Decoupled input interface (`IGameInput`) enabling automated tests
- High score / options persistence hooks
- Potential DI friendly design for services (sound, storage)

## Core Systems
- Screen Manager: manages active screen enum & transitions
- Entity Update Pipeline: updates ship, asteroids, projectiles
- Collision Resolver: isolates collision consequences
- Event Surface: raises strongly typed events for UI layer

## Testing
Unit tests (see `AsteroidsModernEngine.Tests`) simulate input frames and assert screen transitions, score changes and event raising.

## Build
```
dotnet build AsteroidsModernEngine/AsteroidsModernEngine.csproj
```

## Extending
- Implement additional screens (credits, achievements)
- Add dependency injection container integration
- Introduce ECS or component-based entity composition

## License
Internal modern arcade engine sample.
