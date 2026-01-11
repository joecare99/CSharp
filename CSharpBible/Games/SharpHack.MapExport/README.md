# SharpHack.MapExport

Exports a freshly generated SharpHack map to a full-resolution PNG using the same `DisplayTile` mapping logic as the WPF client.

## Usage

```powershell
# From repo root

# Uses defaults from `SharpHack.Wpf/tiles.tileset.json` if found,
# otherwise falls back to `TileService` defaults.
dotnet run --project .\SharpHack.MapExport\SharpHack.MapExport.csproj -- -o .\map.png

# Explicit tileset + tileSize
dotnet run --project .\SharpHack.MapExport\SharpHack.MapExport.csproj -- -t .\SharpHack.Wpf\tiles.png -s 32 -o .\map.png
```

Notes:
- Windows-only (`net10.0-windows`).
- The exporter marks the whole map as explored/visible to get a full map render.
