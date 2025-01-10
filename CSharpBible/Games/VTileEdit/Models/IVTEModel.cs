using System;
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
    SingleTile GetTileDef(Enum? tile);
    void SetTileDef(Enum tile, string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors);
 }