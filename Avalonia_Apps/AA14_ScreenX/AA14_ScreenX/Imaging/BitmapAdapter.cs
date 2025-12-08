using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Runtime.InteropServices;

namespace AA14_ScreenX.Imaging;

internal static class BitmapAdapter
{
    public static Bitmap FromArgbPixels(int width, int height, uint[] argb)
    {
        if (argb is null) throw new ArgumentNullException(nameof(argb));
        var bmp = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        using var fb = bmp.Lock();
        var bytes = new byte[width * height * 4];
        for (int i = 0; i < argb.Length; i++)
        {
            uint c = argb[i];
            bytes[i * 4 + 0] = (byte)(c); // B
            bytes[i * 4 + 1] = (byte)(c >> 8); // G
            bytes[i * 4 + 2] = (byte)(c >> 16); // R
            bytes[i * 4 + 3] = (byte)(c >> 24); // A
        }
        Marshal.Copy(bytes, 0, fb.Address, bytes.Length);
        return bmp;
    }
}
