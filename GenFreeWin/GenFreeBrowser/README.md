# GenFreeBrowser

WPF component & service library focused on geographic map display and place authority search (genealogy-centric). Provides map viewport, tile provider abstraction, caching layer, and pluggable place search authorities.

## Purpose
Unify mapping and location lookup concerns across genealogy applications (desktop UI + console tooling) with a clean API surface.

## Modules
- **Map**: Viewport state, tile sources/providers, HTTP tile retrieval, file cache.
- **Places**: `IPlaceAuthority` (Nominatim, GOV, GeoNames, etc.), `PlaceSearchService` aggregator.
- **DI**: `MapModule` extension method to register required services.

## Key Features
- Bing / OSM / Google (placeholder) provider factory helpers.
- File-based tile cache (`FileTileCache`).
- Reactive viewport state (center, zoom) for UI binding.
- Independent place authorities aggregated via parallel async queries.

## Targets
`net9.0-windows` (WPF usage). Could be split later for non-UI logic re-use.

## Getting Started
```csharp
var services = new ServiceCollection();
services.AddMapViewer();
services.AddHttpClient();
services.AddSingleton<IPlaceAuthority, NominatimAuthority>();
services.AddSingleton<IPlaceSearchService, PlaceSearchService>();
```

## Search Usage
```csharp
var results = await placeSearch.SearchAllAsync(new PlaceQuery("Berlin"));
foreach(var r in results) Console.WriteLine(r.Name);
```

## Map Embedding (Simplified)
```xml
<MapControls:MapView x:Name="Map" />
```
Bind `ViewportState` for pan/zoom persistence.

## Architecture
```
Map
  ITileSource -> IMapProvider -> HttpTileSource
  ITileCache (FileTileCache)
  ViewportState (observable)
Places
  IPlaceAuthority (NominatimAuthority, GovAuthority,...)
  PlaceSearchService (fan-out aggregator)
```

## Extending
- Add authority: implement `IPlaceAuthority` returning normalized `PlaceResult`.
- Custom tile provider: implement `IMapProvider`, supply URL template.
- Cache strategy swap: implement `ITileCache`.

## Roadmap
- Improved error telemetry / diagnostics hooks.
- Optional in-memory tile cache layer.
- Offline / pre-fetch tooling.

## Contributing
Adhere to folder-per-namespace rule. Provide unit tests for new authorities (mock HttpClient).

## License
(Insert license.)
