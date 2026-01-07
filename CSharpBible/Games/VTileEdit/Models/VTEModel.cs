using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace VTileEdit.Models;

public class VTEModel : IVTEModel
{
    private readonly VisTileData _data = new();

    public Size TileSize => _data.TileSize;

    public IEnumerable<int> TileKeys => _data.Keys;

    public string KeyTypeStr => _data.KeyTypeStr;

    public void Clear() => _data.Clear();

    public void LoadFromStream(Stream stream, EStreamType eStreamType) => _data.LoadFromStream(stream, eStreamType);

    public void SaveToStream(Stream stream, EStreamType eStreamType) => _data.WriteToStream(stream, eStreamType);

    public void SetTileSize(Size size) => _data.SetTileSize(size);

    public SingleTile GetTileDef(int? tile) => _data.GetTileDef(tile);

    public void SetTileDef(int tile, string[] lines, FullColor[] colors)
        => _data.SetTileDef(tile, new SingleTile(lines, colors));

    public TileInfo GetTileInfo(int tile) => _data.GetTileInfo(tile);

    public void SetTileInfo(int tile, TileInfo info) => _data.SetTileInfo(tile, info);

    public void SaveTileToStream(int tile, Stream stream, EStreamType eStreamType)
    {
        var single = new VisTileData();
        single.SetTileSize(_data.TileSize);
        var def = _data.GetTileDef(tile);
        var info = _data.GetTileInfo(tile);
        single.SetTileDef(tile, def);
        single.SetTileInfo(tile, info);
        single.WriteToStream(stream, eStreamType);
    }

    public void LoadTileToStream(int tile, Stream stream, EStreamType eStreamType)
    {
        var single = new VisTileData();
        single.LoadFromStream(stream, eStreamType);
        var def = single.GetTileDef(tile);
        var info = single.GetTileInfo(tile);
        _data.SetTileDef(tile, def);
        _data.SetTileInfo(tile, info);
    }
}
