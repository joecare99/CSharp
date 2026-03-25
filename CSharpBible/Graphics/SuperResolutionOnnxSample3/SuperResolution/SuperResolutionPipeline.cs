using SuperResolutionOnnxSample.ImageTiling;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample.SuperResolution;

public sealed class SuperResolutionPipeline
{
    private readonly ISuperResampler _resampler;
    private readonly ImageTiler _tiler;

    public SuperResolutionPipeline(ISuperResampler resampler)
    {
        _resampler = resampler;
        _tiler = new ImageTiler(resampler.InputSize, resampler.Scale);
    }

    public int Scale => _resampler.Scale;

    public int TilesetMirrorBorderMin { get; set; } = 2;

    public async Task<BitmapImage> UpscaleImageAsync(BitmapSource input)
    {
        input = WpfBitmapHelpers.EnsureRgb24(input);

        if (input.PixelWidth <= _resampler.InputSize && input.PixelHeight <= _resampler.InputSize)
        {
            var padded = WpfBitmapHelpers.PadToSize(input, _resampler.InputSize, _resampler.InputSize);
            var rgb = await _resampler.UpscaleRgb24TileAsync(padded).ConfigureAwait(false);

            int ow = input.PixelWidth * _resampler.Scale;
            int oh = input.PixelHeight * _resampler.Scale;
            return CropRgbFromUpscaledTile(rgb, ow, oh);
        }

        return await _tiler.ProcessWholeImageTiledAsync(
            input,
            async tile =>
            {
                var padded = WpfBitmapHelpers.PadToSize(tile, _resampler.InputSize, _resampler.InputSize);
                var rgb = await _resampler.UpscaleRgb24TileAsync(padded).ConfigureAwait(false);

                int ow = tile.PixelWidth * _resampler.Scale;
                int oh = tile.PixelHeight * _resampler.Scale;
                return CropRgbFromUpscaledTileBytes(rgb, ow, oh);
            }).ConfigureAwait(false);
    }

    public async Task<BitmapImage> UpscaleTilesetAsync(BitmapSource sheet, int tileSize)
    {
        sheet = WpfBitmapHelpers.EnsureRgb24(sheet);

        int border = Math.Max(TilesetMirrorBorderMin, tileSize / 4);

        return await _tiler.ProcessTilesetAsync(
            sheet,
            tileSize,
            async tile =>
            {
                // 1) Kontext erzeugen: Tile spiegelnd erweitern
                var extended = WpfBitmapHelpers.MirrorExtend(tile, border);

                // 2) Extended in ein 224x224 Eingabebild einbetten (oben/links, Rest schwarz)
                var padded = WpfBitmapHelpers.PadToSize(extended, _resampler.InputSize, _resampler.InputSize);

                // 3) Modell auf dem Kontext laufen lassen
                var rgb = await _resampler.UpscaleRgb24TileAsync(padded).ConfigureAwait(false);

                // 4) Aus dem 224*3-Ergebnis den inneren (nicht-border) Bereich ausschneiden
                int outTileSize = tile.PixelWidth * _resampler.Scale;
                int cropX = border * _resampler.Scale;
                int cropY = border * _resampler.Scale;
                return CropRgbRectFromUpscaledTileBytes(rgb, outTileSize, outTileSize, cropX, cropY);
            }).ConfigureAwait(false);
    }

    private BitmapImage CropRgbFromUpscaledTile(byte[] rgbTile, int outW, int outH)
    {
        var wb = new WriteableBitmap(outW, outH, 96, 96, PixelFormats.Rgb24, null);
        var bytes = CropRgbFromUpscaledTileBytes(rgbTile, outW, outH);
        wb.WritePixels(new Int32Rect(0, 0, outW, outH), bytes, outW * 3, 0);
        return WpfBitmapHelpers.ToBitmapImage(wb);
    }

    private byte[] CropRgbFromUpscaledTileBytes(byte[] rgbTile, int outW, int outH)
    {
        int tileOutW = _resampler.InputSize * _resampler.Scale;
        int tileStride = tileOutW * 3;

        var cropped = new byte[outW * outH * 3];
        for (int y = 0; y < outH; y++)
            Buffer.BlockCopy(rgbTile, y * tileStride, cropped, y * outW * 3, outW * 3);

        return cropped;
    }

    private byte[] CropRgbRectFromUpscaledTileBytes(byte[] rgbTile, int outW, int outH, int x, int y)
    {
        int tileOutW = _resampler.InputSize * _resampler.Scale;
        int srcStride = tileOutW * 3;

        var cropped = new byte[outW * outH * 3];
        for (int row = 0; row < outH; row++)
        {
            int srcOffset = ((y + row) * srcStride) + x * 3;
            int dstOffset = row * outW * 3;
            Buffer.BlockCopy(rgbTile, srcOffset, cropped, dstOffset, outW * 3);
        }
        return cropped;
    }
}
