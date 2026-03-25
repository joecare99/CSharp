// Sie müssen das NuGet-Paket "System.Drawing.Common" zu Ihrem Projekt hinzufügen, um Bitmap und andere Typen aus System.Drawing zu verwenden.
// Beispiel (in Visual Studio): Rechtsklick auf das Projekt > NuGet-Pakete verwalten > System.Drawing.Common suchen und installieren.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// Stellen Sie sicher, dass Ihr Projekt einen Verweis auf das NuGet-Paket "System.Drawing.Common" hat.
// In Visual Studio: Rechtsklick auf das Projekt > NuGet-Pakete verwalten > System.Drawing.Common suchen und installieren.

class HilpertColorMap
{
    public static Bitmap Map3DTo2DColor(int sizeExponent, int i, int j)
    {
        int n = (int)Math.Pow(2, sizeExponent);
        int totalPixels = n * n;
        Bitmap img = new Bitmap(n, n, PixelFormat.Format24bppRgb);

        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                int distance = HilpertMath.Xy2d(n, x, y);
                double normDist = (double)distance / (totalPixels - 1);
                var r = HilpertMath.GetCubeCoordinateFromDistance(normDist, i);
                // HSV Werte zuweisen
                // Hue: 0 bis 360 Grad
                // Saturation: 1.0 (voll)
                // Value (Helligkeit): 1.0 (voll)
                var (hue, saturation, value) = j switch
                {
                    1 => (r.X, r.Z, r.Y),
                    2 => (r.Y, r.X, r.Z),
                    3 => (r.Z, r.Y, r.X),
                    4 => (r.X, r.Y, r.Z),
                    5 => (r.Y, r.Z, r.X),
                    _ => (r.Z, r.X, r.Y),
                };
                /*Color pixelColor = Color.FromArgb(
                    (int)(hue * 255),
                    (int)(saturation * 255),
                    (int)(value * 255)
                );*/
                Color pixelColor = ColorHelpers.ColorFromHSV(hue * 360.0, saturation, value);
                img.SetPixel(x, y, pixelColor);
            }
        }
        return img;
    }
}
