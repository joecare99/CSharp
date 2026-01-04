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
    IEnumerable<int> TileKeys { get; }
    SingleTile GetTileDef(int? tile);
    void SetTileDef(int tile, string[] lines, FullColor[] colors);
    TileInfo GetTileInfo(int tile);
    void SetTileInfo(int tile, TileInfo info);
    Type KeyType { get; }
    void SaveTileToStream(int tile, Stream stream, EStreamType eStreamType);
    void LoadTileToStream(int tile, Stream stream, EStreamType eStreamType);
}