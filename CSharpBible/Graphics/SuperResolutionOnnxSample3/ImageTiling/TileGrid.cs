using System;
using System.Collections.Generic;
using System.Windows;

namespace SuperResolutionOnnxSample.ImageTiling;

public readonly record struct TileGrid(int TileSize, int Columns, int Rows)
{
    public int TotalTiles => Columns * Rows;

    public static TileGrid FromImage(int imageWidth, int imageHeight, int tileSize)
    {
        if (tileSize <= 0) throw new ArgumentOutOfRangeException(nameof(tileSize));
        if (imageWidth <= 0 || imageHeight <= 0) throw new ArgumentOutOfRangeException("image size");
        if (imageWidth % tileSize != 0 || imageHeight % tileSize != 0)
            throw new InvalidOperationException("Image dimensions are not divisible by tile size.");

        return new TileGrid(tileSize, imageWidth / tileSize, imageHeight / tileSize);
    }

    public IEnumerable<Int32Rect> EnumerateRects()
    {
        for (int y = 0; y < Rows; y++)
            for (int x = 0; x < Columns; x++)
                yield return new Int32Rect(x * TileSize, y * TileSize, TileSize, TileSize);
    }
}
