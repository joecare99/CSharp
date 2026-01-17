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
        input = WpfBitmapHelpers.EnsureBgra32(input);

        if (input.PixelWidth <= _resampler.InputSize && input.PixelHeight <= _resampler.InputSize)
        {
            var padded = WpfBitmapHelpers.PadToSizeBgra32(input, _resampler.InputSize, _resampler.InputSize);

            var bytes = await UpscaleTilePreserveAlphaAsync(padded).ConfigureAwait(false);

            int ow = input.PixelWidth * _resampler.Scale;
            int oh = input.PixelHeight * _resampler.Scale;
            return CropBgraFromUpscaledTile(bytes, ow, oh);
        }

        return await _tiler.ProcessWholeImageTiledBgra32Async(
            input,
            async tile =>
            {
                var padded = WpfBitmapHelpers.PadToSizeBgra32(tile, _resampler.InputSize, _resampler.InputSize);
                var bytes = await UpscaleTilePreserveAlphaAsync(padded).ConfigureAwait(false);

                int ow = tile.PixelWidth * _resampler.Scale;
                int oh = tile.PixelHeight * _resampler.Scale;
                return CropBgraFromUpscaledTileBytes(bytes, ow, oh);
            }).ConfigureAwait(false);
    }

    public async Task<BitmapImage> UpscaleTilesetAsync(BitmapSource sheet, int tileSize)
    {
        sheet = WpfBitmapHelpers.EnsureBgra32(sheet);

        int border = Math.Max(TilesetMirrorBorderMin, tileSize / 4);

        return await _tiler.ProcessTilesetBgra32Async(
            sheet,
            tileSize,
            async tile =>
            {
                // 1) Kontext erzeugen: Tile spiegelnd erweitern
                var extended = WpfBitmapHelpers.MirrorExtendBgra32(tile, border);

                // 2) Extended in ein 224x224 Eingabebild einbetten (oben/links, Rest schwarz)
                var padded = WpfBitmapHelpers.PadToSizeBgra32(extended, _resampler.InputSize, _resampler.InputSize);

                // 3) Modell auf dem Kontext laufen lassen
                var bytes = await UpscaleTilePreserveAlphaAsync(padded).ConfigureAwait(false);

                // 4) Aus dem 224*3-Ergebnis den inneren (nicht-border) Bereich ausschneiden
                int outTileSize = tile.PixelWidth * _resampler.Scale;
                int cropX = border * _resampler.Scale;
                int cropY = border * _resampler.Scale;
                return CropBgraRectFromUpscaledTileBytes(bytes, outTileSize, outTileSize, cropX, cropY);
            }).ConfigureAwait(false);
    }

    private async Task<byte[]> UpscaleTilePreserveAlphaAsync(BitmapSource tileBgra32)
    {
        if (_resampler is OnnxYSuperResampler y)
        {
            return await y.UpscaleBgra32TileAsync(tileBgra32).ConfigureAwait(false);
        }

        // Fallback: upscale RGB only and set alpha to 255.
        var rgbTile = WpfBitmapHelpers.EnsureRgb24(tileBgra32);
        var rgb = await _resampler.UpscaleRgb24TileAsync(rgbTile).ConfigureAwait(false);

        int ow = _resampler.InputSize * _resampler.Scale;
        int oh = _resampler.InputSize * _resampler.Scale;
        var bgra = new byte[ow * oh * 4];
        for (int i = 0; i < ow * oh; i++)
        {
            bgra[i * 4] = rgb[i * 3 + 2];
            bgra[i * 4 + 1] = rgb[i * 3 + 1];
            bgra[i * 4 + 2] = rgb[i * 3];
            bgra[i * 4 + 3] = 255;
        }

        return bgra;
    }

    private BitmapImage CropBgraFromUpscaledTile(byte[] bgraTile, int outW, int outH)
    {
        var wb = new WriteableBitmap(outW, outH, 96, 96, PixelFormats.Bgra32, null);
        var bytes = CropBgraFromUpscaledTileBytes(bgraTile, outW, outH);
        wb.WritePixels(new Int32Rect(0, 0, outW, outH), bytes, outW * 4, 0);
        return WpfBitmapHelpers.ToBitmapImage(wb);
    }

    private byte[] CropBgraFromUpscaledTileBytes(byte[] bgraTile, int outW, int outH)
    {
        int tileOutW = _resampler.InputSize * _resampler.Scale;
        int tileStride = tileOutW * 4;

        var cropped = new byte[outW * outH * 4];
        for (int y = 0; y < outH; y++)
            Buffer.BlockCopy(bgraTile, y * tileStride, cropped, y * outW * 4, outW * 4);

        return cropped;
    }

    private byte[] CropBgraRectFromUpscaledTileBytes(byte[] bgraTile, int outW, int outH, int x, int y)
    {
        int tileOutW = _resampler.InputSize * _resampler.Scale;
        int srcStride = tileOutW * 4;

        var cropped = new byte[outW * outH * 4];
        for (int row = 0; row < outH; row++)
        {
            int srcOffset = ((y + row) * srcStride) + x * 4;
            int dstOffset = row * outW * 4;
            Buffer.BlockCopy(bgraTile, srcOffset, cropped, dstOffset, outW * 4);
        }
        return cropped;
    }
}
