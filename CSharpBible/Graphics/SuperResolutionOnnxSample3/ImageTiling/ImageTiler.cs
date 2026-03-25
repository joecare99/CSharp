using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample.ImageTiling;

public sealed class ImageTiler
{
    private readonly int _tileSize;
    private readonly int _scale;

    public ImageTiler(int tileSize, int scale)
    {
        _tileSize = tileSize;
        _scale = scale;
    }

    public async Task<BitmapImage> ProcessTilesetAsync(BitmapSource sheet, int tileSize, Func<BitmapSource, Task<byte[]>> processTileRgb24)
    {
        if (processTileRgb24 == null) throw new ArgumentNullException(nameof(processTileRgb24));
        sheet = WpfBitmapHelpers.EnsureRgb24(sheet);

        var grid = TileGrid.FromImage(sheet.PixelWidth, sheet.PixelHeight, tileSize);
        int outW = sheet.PixelWidth * _scale;
        int outH = sheet.PixelHeight * _scale;

        var outBuffer = new byte[outW * outH * 3];

        int idx = 0;
        foreach (var rect in grid.EnumerateRects())
        {
            var tile = new CroppedBitmap(sheet, rect);
            var tileRgb = await processTileRgb24(tile).ConfigureAwait(false);

            int dx = rect.X * _scale;
            int dy = rect.Y * _scale;
            int copyW = rect.Width * _scale;
            int copyH = rect.Height * _scale;
            CopyTile(tileRgb, copyW, copyH, outBuffer, outW, dx, dy, copyW, copyH);

            idx++;
        }

        var wb = new WriteableBitmap(outW, outH, 96, 96, PixelFormats.Rgb24, null);
        wb.WritePixels(new Int32Rect(0, 0, outW, outH), outBuffer, outW * 3, 0);
        return WpfBitmapHelpers.ToBitmapImage(wb);
    }

    public async Task<BitmapImage> ProcessWholeImageTiledAsync(BitmapSource src, Func<BitmapSource, Task<byte[]>> processTileRgb24, int overlap = 16)
    {
        if (processTileRgb24 == null) throw new ArgumentNullException(nameof(processTileRgb24));
        src = WpfBitmapHelpers.EnsureRgb24(src);

        int w = src.PixelWidth;
        int h = src.PixelHeight;
        int ow = w * _scale;
        int oh = h * _scale;
        var outBuffer = new byte[ow * oh * 3];

        int stride = _tileSize - overlap;
        if (stride <= 0) stride = _tileSize;

        for (int y = 0; y < h; y += stride)
        {
            int tileH = Math.Min(_tileSize, h - y);
            for (int x = 0; x < w; x += stride)
            {
                int tileW = Math.Min(_tileSize, w - x);
                var rect = new Int32Rect(x, y, tileW, tileH);
                var cropped = new CroppedBitmap(src, rect);
                var tileRgb = await processTileRgb24(cropped).ConfigureAwait(false);

                int destW = tileW * _scale;
                int destH = tileH * _scale;
                int destX = x * _scale;
                int destY = y * _scale;

                CopyTile(tileRgb, destW, destH, outBuffer, ow, destX, destY, destW, destH);
            }
        }

        var wb = new WriteableBitmap(ow, oh, 96, 96, PixelFormats.Rgb24, null);
        wb.WritePixels(new Int32Rect(0, 0, ow, oh), outBuffer, ow * 3, 0);
        return WpfBitmapHelpers.ToBitmapImage(wb);
    }

    public Task<byte[]> ProcessSingleTileAsync(BitmapSource src, Func<BitmapSource, Task<byte[]>> processPadded224Rgb24)
    {
        if (processPadded224Rgb24 == null) throw new ArgumentNullException(nameof(processPadded224Rgb24));

        // pad to model input size
        var padded = WpfBitmapHelpers.PadToSize(src, _tileSize, _tileSize);
        return processPadded224Rgb24(padded);
    }

    private static void CopyTile(byte[] src, int srcW, int srcH, byte[] dest, int destW, int dx, int dy, int copyW, int copyH)
    {
        int srcStride = srcW * 3;
        int destStride = destW * 3;
        for (int y = 0; y < copyH; y++)
        {
            int srcOffset = y * srcStride;
            int destOffset = (dy + y) * destStride + dx * 3;
            Buffer.BlockCopy(src, srcOffset, dest, destOffset, copyW * 3);
        }
    }
}
