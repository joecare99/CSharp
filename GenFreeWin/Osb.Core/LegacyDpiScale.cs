using System;

namespace Osb.Core;

public static class LegacyDpiScale
{
    public const int DefaultDeviceDpi = 96;

    public static double TwipsPerPixel(int deviceDpi = DefaultDeviceDpi)
    {
        if (deviceDpi <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(deviceDpi), "Device DPI must be greater than zero.");
        }

        return 1440.0 / deviceDpi;
    }

    public static double PixelsToTwips(double pixels, int deviceDpi = DefaultDeviceDpi)
    {
        return pixels * TwipsPerPixel(deviceDpi);
    }

    public static double TwipsToPixels(double twips, int deviceDpi = DefaultDeviceDpi)
    {
        return twips / TwipsPerPixel(deviceDpi);
    }
}
