using System;
using ConsoleLib.Showcase.Desktop.Controls;

namespace ConsoleLib.Showcase.Desktop.Rendering;

/// <summary>Rasterizes a clock face and its hands into a Braille canvas.</summary>
public static class BrailleClockRenderer
{
    public static string Render(DateTime time, int pixelWidth = 36, int pixelHeight = 24)
    {
        var pixels = new bool[Math.Max(1, pixelWidth), Math.Max(1, pixelHeight)];
        var centerX = pixelWidth / 2;
        var centerY = pixelHeight / 2;
        var radius = Math.Max(2, Math.Min(pixelWidth, pixelHeight) / 2 - 2);

        DrawCircle(pixels, centerX, centerY, radius);
        DrawCardinalTick(pixels, centerX, centerY - radius);
        DrawCardinalTick(pixels, centerX + radius, centerY);
        DrawCardinalTick(pixels, centerX, centerY + radius);
        DrawCardinalTick(pixels, centerX - radius, centerY);

        DrawHand(pixels, centerX, centerY, radius - 5, HourAngle(time));
        DrawHand(pixels, centerX, centerY, radius - 3, time.Minute * 6);
        DrawHand(pixels, centerX, centerY, radius - 1, time.Second * 6);
        SetPixel(pixels, centerX, centerY);

        return BrailleCanvas.Render(pixels, pixelWidth, pixelHeight);
    }

    private static double HourAngle(DateTime time) =>
        (time.Hour % 12) * 30d + time.Minute * 0.5d;

    private static void DrawCardinalTick(bool[,] pixels, int x, int y)
    {
        SetPixel(pixels, x, y);
        SetPixel(pixels, x + 1, y);
        SetPixel(pixels, x - 1, y);
        SetPixel(pixels, x, y + 1);
        SetPixel(pixels, x, y - 1);
    }

    private static void DrawCircle(bool[,] pixels, int centerX, int centerY, int radius)
    {
        for (var angle = 0; angle < 360; angle += 2)
        {
            var radians = angle * Math.PI / 180d;
            SetPixel(
                pixels,
                centerX + (int)Math.Round(Math.Cos(radians) * radius),
                centerY + (int)Math.Round(Math.Sin(radians) * radius));
        }
    }

    private static void DrawHand(bool[,] pixels, int centerX, int centerY, int length, double angle)
    {
        var radians = (angle - 90d) * Math.PI / 180d;
        var endX = centerX + (int)Math.Round(Math.Cos(radians) * length);
        var endY = centerY + (int)Math.Round(Math.Sin(radians) * length);
        var deltaX = Math.Abs(endX - centerX);
        var stepX = centerX < endX ? 1 : -1;
        var deltaY = -Math.Abs(endY - centerY);
        var stepY = centerY < endY ? 1 : -1;
        var error = deltaX + deltaY;
        var x = centerX;
        var y = centerY;

        while (true)
        {
            SetPixel(pixels, x, y);
            if (x == endX && y == endY)
                break;

            var doubledError = 2 * error;
            if (doubledError >= deltaY)
            {
                error += deltaY;
                x += stepX;
            }
            if (doubledError <= deltaX)
            {
                error += deltaX;
                y += stepY;
            }
        }
    }

    private static void SetPixel(bool[,] pixels, int x, int y)
    {
        if ((uint)x < pixels.GetLength(0) && (uint)y < pixels.GetLength(1))
            pixels[x, y] = true;
    }
}
