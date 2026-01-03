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

    public IEnumerable<Enum> TileKeys => _data.Keys;

    public Type KeyType => _data.KeyType;

    public void Clear() => _data.Clear();

    public void LoadFromStream(Stream stream, EStreamType eStreamType) => _data.LoadFromStream(stream, eStreamType);

    public void SaveToStream(Stream stream, EStreamType eStreamType) => _data.WriteToStream(stream, eStreamType);

    public void SetTileSize(Size size) => _data.SetTileSize(size);

    public SingleTile GetTileDef(Enum? tile) => _data.GetTileDef(tile);

    public void SetTileDef(Enum tile, string[] lines, FullColor[] colors)
        => _data.SetTileDef(tile, new SingleTile(lines, colors));

    public TileInfo GetTileInfo(Enum tile) => _data.GetTileInfo(tile);

    public void SetTileInfo(Enum tile, TileInfo info) => _data.SetTileInfo(tile, info);

    public void SaveTileToStream(Enum tile, Stream stream, EStreamType eStreamType)
    {
        var single = new VisTileData();
        single.SetTileSize(_data.TileSize);
        var def = _data.GetTileDef(tile);
        var info = _data.GetTileInfo(tile);
        single.SetTileDef(tile, def);
        single.SetTileInfo(tile, info);
        single.WriteToStream(stream, eStreamType);
    }
}
