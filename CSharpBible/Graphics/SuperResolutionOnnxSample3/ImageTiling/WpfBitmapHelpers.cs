using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample.ImageTiling;

public static class WpfBitmapHelpers
{
    public static BitmapSource EnsureRgb24(BitmapSource src)
    {
        if (src.Format == PixelFormats.Rgb24) return src;
        var converted = new FormatConvertedBitmap(src, PixelFormats.Rgb24, null, 0);
        converted.Freeze();
        return converted;
    }

    public static byte[] CopyRgb24Bytes(BitmapSource src)
    {
        src = EnsureRgb24(src);
        int w = src.PixelWidth;
        int h = src.PixelHeight;
        var buffer = new byte[w * h * 3];
        src.CopyPixels(buffer, w * 3, 0);
        return buffer;
    }

    public static void WriteRgb24Bytes(WriteableBitmap wb, byte[] rgb24, int width, int height)
    {
        wb.WritePixels(new Int32Rect(0, 0, width, height), rgb24, width * 3, 0);
    }

    public static BitmapImage ToBitmapImage(WriteableBitmap wb)
    {
        using var ms = new MemoryStream();
        var enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(wb));
        enc.Save(ms);
        ms.Position = 0;
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = ms;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    public static BitmapSource PadToSize(BitmapSource src, int width, int height)
    {
        if (src.PixelWidth == width && src.PixelHeight == height) return src;
        src = EnsureRgb24(src);

        var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Rgb24, null);
        var buf = CopyRgb24Bytes(src);
        wb.WritePixels(new Int32Rect(0, 0, src.PixelWidth, src.PixelHeight), buf, src.PixelWidth * 3, 0);
        wb.Freeze();
        return wb;
    }

    public static BitmapSource MirrorExtend(BitmapSource src, int border)
    {
        src = EnsureRgb24(src);
        if (border <= 0) return src;

        int w = src.PixelWidth;
        int h = src.PixelHeight;
        int ew = w + 2 * border;
        int eh = h + 2 * border;

        var srcBuf = CopyRgb24Bytes(src);
        var ext = new byte[ew * eh * 3];

        for (int y = 0; y < eh; y++)
        {
            int sy = MirrorIndex(y - border, h);
            for (int x = 0; x < ew; x++)
            {
                int sx = MirrorIndex(x - border, w);

                int sIdx = (sy * w + sx) * 3;
                int dIdx = (y * ew + x) * 3;
                ext[dIdx] = srcBuf[sIdx];
                ext[dIdx + 1] = srcBuf[sIdx + 1];
                ext[dIdx + 2] = srcBuf[sIdx + 2];
            }
        }

        var wb = new WriteableBitmap(ew, eh, 96, 96, PixelFormats.Rgb24, null);
        wb.WritePixels(new Int32Rect(0, 0, ew, eh), ext, ew * 3, 0);
        wb.Freeze();
        return wb;

        static int MirrorIndex(int i, int len)
        {
            if (len <= 1) return 0;
            while (i < 0 || i >= len)
            {
                if (i < 0) i = -i - 1;
                else i = (2 * len - 1) - i;
            }
            return i;
        }
    }

    public static BitmapSource Crop(BitmapSource src, Int32Rect rect)
    {
        var cropped = new CroppedBitmap(src, rect);
        cropped.Freeze();
        return cropped;
    }
}
