using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace VTileEdit.Models;

public class VTEModel : IVTEModel
{
    private readonly VisTileData _data = new();

    public Size TileSize => _data.TileSize;

    public Type KeyType => _data.KeyType;

    public void Clear() => _data.Clear();

    public void LoadFromStream(Stream stream, EStreamType eStreamType) => _data.LoadFromStream(stream, eStreamType);

    public void SaveToStream(Stream stream, EStreamType eStreamType) => _data.WriteToStream(stream, eStreamType);

    public void SetTileSize(Size size) => _data.SetTileSize(size);

    public SingleTile GetTileDef(Enum? tile) => _data.GetTileDef(tile);

    public void SetTileDef(Enum tile, string[] lines, FullColor[] colors)
        => _data.SetTileDef(tile, new SingleTile(lines, colors));
}
