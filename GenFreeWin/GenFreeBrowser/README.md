# GenFreeBrowser

WPF browser application and place-authority library focused on genealogy-centric location lookup. The reusable map viewer has been extracted into the separate `GenFreeBrowser.Map` project.

## Purpose
Unify mapping and location lookup concerns across genealogy applications (desktop UI + console tooling) with a clean API surface.

## Modules
- **Places**: `IPlaceAuthority` (Nominatim, GOV, GeoNames, etc.), `PlaceSearchService` aggregator.
- **Browser app**: WPF composition root wiring browser-specific services together with the extracted map module.

## Key Features
- Independent place authorities aggregated via parallel async queries.

## Targets
`net9.0-windows` for the WPF browser application and place search integration.

## Getting Started
```csharp
var services = new ServiceCollection();
services.AddHttpClient();
services.AddSingleton<IPlaceAuthority, NominatimAuthority>();
services.AddSingleton<IPlaceSearchService, PlaceSearchService>();
```

## Search Usage
```csharp
var results = await placeSearch.SearchAllAsync(new PlaceQuery("Berlin"));
foreach(var r in results) Console.WriteLine(r.Name);
```

## Architecture
```
Places
  IPlaceAuthority (NominatimAuthority, GovAuthority,...)
  PlaceSearchService (fan-out aggregator)
```

## Extending
- Add authority: implement `IPlaceAuthority` returning normalized `PlaceResult`.
- Map functionality: use the separate `GenFreeBrowser.Map` project.

## Roadmap
- Improved error telemetry / diagnostics hooks.
- Optional in-memory tile cache layer.
- Offline / pre-fetch tooling.

## Contributing
Adhere to folder-per-namespace rule. Provide unit tests for new authorities (mock HttpClient).

## License
(Insert license.)
