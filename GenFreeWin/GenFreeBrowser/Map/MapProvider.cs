using GenFreeBrowser.Map.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GenFreeBrowser.Map;

public sealed class MapProvider : IMapProvider
{
    private readonly List<LayerDef> _layers = new();
    private int _layerIndex;

    private record LayerDef(
        string UrlTemplate,
        int ServerCount,
        int MinZoom,
        int MaxZoom,
        Func<int, string>? ServerFormatter,
        Func<TileId, string>? XFormatter,
        Func<TileId, string>? YFormatter,
        Func<TileId, string>? ZFormatter);

    public string Id { get; }
    public string Name { get; }

    public MapProvider(string id, string name)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public int LayerCount => _layers.Count;

    public int Layer
    {
        get => _layerIndex;
        set
        {
            if (value < 0 || value >= _layers.Count)
                throw new ArgumentOutOfRangeException(nameof(value));
            _layerIndex = value;
        }
    }

    public int MinZoom => _layers.Count == 0 ? 0 : _layers[_layerIndex].MinZoom;
    public int MaxZoom => _layers.Count == 0 ? 0 : _layers[_layerIndex].MaxZoom;

    public void AddUrl(string template, int serverCount, int minZoom, int maxZoom,
        Func<int, string>? serverFormatter = null,
        Func<TileId, string>? xFormatter = null,
        Func<TileId, string>? yFormatter = null,
        Func<TileId, string>? zFormatter = null)
    {
        _layers.Add(new LayerDef(template, serverCount, minZoom, maxZoom, serverFormatter, xFormatter, yFormatter, zFormatter));
        _layerIndex = 0;
    }

    private int[] _serverRoundRobin = Array.Empty<int>();

    public string? GetTileUrl(TileId id)
    {
        if (_layers.Count == 0) return null;
        var l = _layers[_layerIndex];
        if (id.Z < l.MinZoom || id.Z > l.MaxZoom) return null;

        if (_serverRoundRobin.Length != _layers.Count)
            _serverRoundRobin = new int[_layers.Count];

        var svrIdx = l.ServerCount <= 0 ? 0 : (_serverRoundRobin[_layerIndex]++ % l.ServerCount);

        string Replace(string pattern, string value) => Regex.Replace(pattern, "%" + value + "%", _ => value, RegexOptions.IgnoreCase);

        string url = l.UrlTemplate;
        var svrVal = l.ServerFormatter?.Invoke(svrIdx) ?? svrIdx.ToString();
        var xVal = l.XFormatter?.Invoke(id) ?? id.X.ToString();
        var yVal = l.YFormatter?.Invoke(id) ?? id.Y.ToString();
        var zVal = l.ZFormatter?.Invoke(id) ?? id.Z.ToString();

        url = url.Replace("%serv%", svrVal)
                 .Replace("%x%", xVal)
                 .Replace("%y%", yVal)
                 .Replace("%z%", zVal);
        return url;
    }

    // Static helpers (equivalent to Pascal functions)
    public static string LetterServer(int id) => ((char)('a' + id)).ToString();

    public static string QuadKey(TileId tile)
    {
        // Bing maps quadkey
        var quad = new char[tile.Z];
        for (int i = tile.Z; i > 0; i--)
        {
            int digit = 0;
            long mask = 1L << (i - 1);
            if ((tile.X & mask) != 0) digit += 1;
            if ((tile.Y & mask) != 0) digit += 2;
            quad[tile.Z - i] = (char)('0' + digit);
        }
        return new string(quad);
    }
}
