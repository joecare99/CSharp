# VectorGfx2

Second iteration / refinement of the vector graphics console rendering experiments. Focuses on modularizing algorithms and improving performance or fidelity vs the original `VectorGfx` project.

## Improvements Over VectorGfx
- Cleaner separation of raster back end and shape generation
- Potential caching of stroke patterns
- Extended primitive set (arcs, splines) where implemented

## Architecture Sketch
```
Shape Model -> Tessellator / Rasterizer -> Console Buffer Adapter -> Output
```

## Extension Points
- Swap raster back end (offscreen buffer vs direct console writes)
- Plug in different text glyph density mappings for faux shading
- Add profiling hooks to measure algorithm performance

## Build
```
dotnet build VectorGfx2/VectorGfx2.csproj
```

## Status
Experimental. Some components may be placeholders awaiting implementation.

## License
Internal experimental module.
