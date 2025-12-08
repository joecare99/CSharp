# Asteroids_Base

Engine library for an Asteroids style arcade game. Handles ship physics, asteroid field generation, projectile logic and collision detection separate from rendering.

## Features
- 2D vector based kinematics (position, velocity, rotation)
- Wrap-around playfield logic
- Randomized asteroid spawning & fragmentation upon destruction
- Ship thrust, rotation, firing with cooldown
- Collision detection (ship–asteroid, bullet–asteroid)
- Event hooks for sound / scoring

## Physics Model
Uses simple Euler integration per tick. Extend with acceleration smoothing or frame time scaling if needed.

## Extensibility
- Add UFO enemy with its own AI
- Power-ups (shield, rapid fire, hyperspace)
- Particle system for explosions
- Scoring & high score persistence

## Consuming Projects
A console or graphical (WPF/WinUI) front end subscribes to engine events and renders the scene.

## Build
```
dotnet build Asteroids_Base/Asteroids_Base.csproj
```

## License
Internal sample arcade engine.
