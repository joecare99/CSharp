using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace VTileEdit.Models;

public interface IVTEModel
{
    void LoadFromStream(Stream stream, EStreamType eStreamType);
    void SaveToStream(Stream stream, EStreamType eStreamType);
    void Clear();
    void SetTileSize(Size size);
    Size TileSize { get; }
    IEnumerable<Enum> TileKeys { get; }
    SingleTile GetTileDef(Enum? tile);
    void SetTileDef(Enum tile, string[] lines, FullColor[] colors);
    TileInfo GetTileInfo(Enum tile);
    void SetTileInfo(Enum tile, TileInfo info);
    Type KeyType { get; }
    void SaveTileToStream(Enum tile, Stream stream, EStreamType eStreamType);
}