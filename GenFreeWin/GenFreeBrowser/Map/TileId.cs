namespace GenFreeBrowser.Map;

/// <summary>
/// Tile identification in a WebMercator XYZ tiling scheme
/// </summary>
public readonly record struct TileId(long X, long Y, int Z)
{
    public override string ToString() => $"{Z}/{X}/{Y}";
}
