# MapDemo

WPF sample application demonstrating integration of the map viewer components from `GenFreeBrowser.Map` including provider selection and basic viewport interaction.

## Purpose
Serve as a manual test bench / visual harness for tile providers, caching logic, and viewport behaviors separate from larger host applications.

## Features
- Registers multiple map providers (Bing, OSM, Google placeholder) via DI.
- Instantiates `MapView` control bound to shared `ViewportState`.
- Demonstrates `AddMapViewer` extension usage.

## Running
```bash
cd Gen_FreeWin/MapDemo
 dotnet run
```

## Code Snippet
```csharp
var sc = new ServiceCollection();
sc.AddMapViewer();
sc.AddMapProviderCatalog(osm, bing);
var sp = sc.BuildServiceProvider();
```

## Extending
- Add UI for zoom level display.
- Bind keyboard shortcuts for pan / zoom.
- Add overlay layer (e.g. markers for search results).

## Contributing
Ensure new provider additions update README with configuration notes.

## License
(Insert license.)
