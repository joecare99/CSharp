# VectorGfx

Experimental vector style console graphics playground. Demonstrates drawing higher level geometric primitives (lines, polygons, possibly bezier approximations) on a character grid or abstract surface.

## Objectives
- Explore techniques for representing vector data in low resolution console medium
- Provide reusable drawing algorithms for other mini games

## Potential Components
- Bresenham / incremental line & circle algorithms
- Shape filling and outline strategies
- Layer / z-order simulation with character & color blending
- Simple animation loop utilities

## Usage
Library or console app (depending on project structure) can be run to render sample scenes. Extend by adding new primitive drawers.

## Ideas for Extension
- Export rendered frames to ANSI art files
- Implement sprite-to-vector conversion utilities
- Benchmark different rasterization approaches

## Build
```
dotnet build VectorGfx/VectorGfx.csproj
```

## License
Internal exploratory sample.
