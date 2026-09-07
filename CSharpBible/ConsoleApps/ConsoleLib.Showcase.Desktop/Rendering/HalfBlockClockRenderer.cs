using System;
using System.Drawing;
using System.Text;

namespace ConsoleLib.Showcase.Desktop.Rendering;

/// <summary>Renders the clock as two vertical pixels per console cell.</summary>
public static class HalfBlockClockRenderer
{
    public static string RenderColorMap(DateTime time, int pixelWidth = 29, int pixelHeight = 28)
    {
        var pixels = new ConsoleColor[Math.Max(1, pixelWidth), Math.Max(2, pixelHeight)];
        var centerX = pixelWidth / 2;
        var centerY = pixelHeight / 2;
        var radius = Math.Max(2, Math.Min(pixelWidth, pixelHeight) / 2 - 1);

        DrawCircle(pixels, centerX, centerY, radius+0.4, ConsoleColor.Yellow);
        DrawMarker(pixels, centerX, centerY - radius + 1, ConsoleColor.White);
        DrawMarker(pixels, centerX + radius - 1, centerY, ConsoleColor.White);
        DrawMarker(pixels, centerX, centerY + radius - 1, ConsoleColor.White);
        DrawMarker(pixels, centerX - radius + 1, centerY, ConsoleColor.White);
     //   DrawHand(pixels, centerX, centerY, radius - 1, time.Second * 6d, ConsoleColor.Green);
        DrawHand(pixels, centerX, centerY, radius - 6, 45d, ConsoleColor.Red);
        DrawHand(pixels, centerX, centerY, radius - 6, (time.Hour % 12) * 30d + time.Minute * 0.5d, ConsoleColor.Gray);
        DrawHand(pixels, centerX, centerY, radius - 4, time.Minute * 6d+ time.Second * 0.1d, ConsoleColor.White);
        SetPixel(pixels, centerX, centerY, ConsoleColor.White);

        var rows = new string[pixelHeight];
        for (var y = 0; y < pixelHeight; y++)
        {
            var row = new char[pixelWidth];
            for (var x = 0; x < pixelWidth; x++)
                row[x] = (char)((int)pixels[x, y] < 10
                    ? '0' + (int)pixels[x, y]
                    : 'A' + (int)pixels[x, y] - 10);
            rows[y] = new string(row);
        }
        return string.Join(Environment.NewLine, rows);
    }

    public static string Render(DateTime time, int pixelWidth = 48, int pixelHeight = 24)
    {
        var pixels = new byte[Math.Max(1, pixelWidth), Math.Max(2, pixelHeight)];
        var centerX = pixelWidth / 2;
        var centerY = pixelHeight / 2;
        var radius = Math.Max(2, Math.Min(pixelWidth, pixelHeight) / 2 - 2);

        DrawCircle(pixels, centerX, centerY, radius, 1);
        DrawHand(pixels, centerX, centerY, radius - 7, (time.Hour % 12) * 30d + time.Minute * 0.5d, 4);
   //     DrawHand(pixels, centerX, centerY, radius - 4, time.Minute * 6d, 3);
    //    DrawHand(pixels, centerX, centerY, radius - 1, time.Second * 6d, 5);
        SetPixel(pixels, centerX, centerY, 5);

        var output = new StringBuilder();
        for (var y = 0; y < pixelHeight; y += 2)
        {
            if (y > 0)
                output.Append(Environment.NewLine);

            for (var x = 0; x < pixelWidth; x++)
            {
                var top = pixels[x, y];
                var bottom = y + 1 < pixelHeight ? pixels[x, y + 1] : (byte)0;
                output.Append(GetCell(top, bottom));
            }
        }

        return output.ToString();
    }

    private static char GetCell(byte top, byte bottom)
    {
        if (top == 0 && bottom == 0)
            return ' ';
        if (top == bottom)
            return top switch
            {
                1 => '░',
                3 => '▒',
                4 => '▓',
                _ => '█'
            };
        return top >= bottom ? '▀' : '▄';
    }

    private static void DrawCircle(byte[,] pixels, int centerX, int centerY, int radius, byte shade)
    {
        for (var angle = 0; angle < 360; angle += 2)
        {
            var radians = angle * Math.PI / 180d;
            SetPixel(pixels,
                centerX + (int)Math.Round(Math.Cos(radians) * radius),
                centerY + (int)Math.Round(Math.Sin(radians) * radius),
                shade);
        }
    }

    private static void DrawCircle(ConsoleColor[,] pixels, int centerX, int centerY, double radius, ConsoleColor color)
    {
        for (var secans = 0; secans <= radius / 1.41; secans += 1)
        {
            var radians = Math.Asin((double)secans / radius);
            var cosradians = (int)Math.Round(Math.Cos(radians) * radius);
            SetPixel(pixels, centerX + cosradians,
                centerY + secans, color);
            SetPixel(pixels, centerX + cosradians,
                centerY - secans, color);
            SetPixel(pixels, centerX - cosradians,
                centerY + secans, color);
            SetPixel(pixels, centerX - cosradians,
                centerY - secans, color);
            SetPixel(pixels, centerX + secans,
                centerY + cosradians, color);
            SetPixel(pixels, centerX + secans,
                centerY - cosradians, color);
            SetPixel(pixels, centerX - secans,
                centerY + cosradians, color);
            SetPixel(pixels, centerX - secans,
                centerY - cosradians, color);
        }
    }

    private static void DrawMarker(ConsoleColor[,] pixels, int x, int y, ConsoleColor color)
    {
        for (var dx = -1; dx <= 1; dx++)
            for (var dy = -1; dy <= 1; dy++)
                SetPixel(pixels, x + dx, y + dy, color);
    }

    private static void DrawHand(byte[,] pixels, int centerX, int centerY, double length, double angle, byte shade)
    {
        var radians = (angle - 90d) * Math.PI / 180d;
        var endX = centerX + Math.Cos(radians) * length;
        var endY = centerY + Math.Sin(radians) * length;
        var deltaX = Math.Abs(endX - centerX);
        var stepX = centerX < endX ? 1 : -1;
        var deltaY = -Math.Abs(endY - centerY);
        var stepY = centerY < endY ? 1 : -1;
        var error = deltaX + deltaY;
        var x = centerX;
        var y = centerY;

        while (true)
        {
            SetPixel(pixels, x, y, shade);
            if ( Math.Abs(x - endX) < 0.5 && Math.Abs(y - endY) < 0.5)
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

    private static void SetPixel(byte[,] pixels, int x, int y, byte shade)
    {
        if ((uint)x < pixels.GetLength(0) && (uint)y < pixels.GetLength(1))
            pixels[x, y] = Math.Max(pixels[x, y], shade);
    }

    private static void DrawHand(ConsoleColor[,] pixels, int centerX, int centerY, double length, double angle, ConsoleColor color)
    {
        var radians = (angle - 90d) * Math.PI / 180d;
        var endX = centerX + Math.Cos(radians) * length;
        var endY = centerY + Math.Sin(radians) * length;
        var deltaX = Math.Abs(endX - centerX);
        var stepX = centerX < endX ? 1 : -1;
        var deltaY = -Math.Abs(endY - centerY);
        var stepY = centerY < endY ? 1 : -1;
        var error = deltaX + deltaY;
        var x = centerX;
        var y = centerY;
        while ((endX-x)*stepX + (endY-y)*stepY > 0)
        {
            SetPixel(pixels, x, y, color);
            var doubledError = 2 * error;
            if (doubledError >= deltaY) { error += deltaY; x += stepX; }
       //     doubledError = 2 * error;
            if (doubledError <= deltaX) { error += deltaX; y += stepY; }
        }
    }

    private static void SetPixel(ConsoleColor[,] pixels, int x, int y, ConsoleColor color)
    {
        if ((uint)x < pixels.GetLength(0) && (uint)y < pixels.GetLength(1))
            pixels[x, y] = color;
    }
}
