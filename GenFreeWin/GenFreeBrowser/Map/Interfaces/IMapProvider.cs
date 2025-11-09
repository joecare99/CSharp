using System;
using System.Threading.Tasks;

namespace GenFreeBrowser.Map.Interfaces;

public interface IMapProvider
{
    /// <summary>
    /// Stable, eindeutige Kennung (persistierbar) z.B. "osm", "bing_roads".
    /// </summary>
    string Id { get; }
    /// <summary>
    /// Anzeigename für UI.
    /// </summary>
    string Name { get; }
    int MinZoom { get; }
    int MaxZoom { get; }
    /// <summary>
    /// Returns a URL to load given tile (if remote) OR a cache key path.
    /// Can return null if tile outside supported range.
    /// </summary>
    string? GetTileUrl(TileId id);
}

public interface ITileSource
{
    Task<byte[]?> GetTileAsync(TileId id);
}
