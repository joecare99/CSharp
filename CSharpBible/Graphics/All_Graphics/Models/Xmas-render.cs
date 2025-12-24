using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace All_Graphics.Models
{
    public class Xmas_render
    {
        private const int SquareW = 64;
        private static readonly double gc = Math.Sqrt(1.25) - 0.5;
        private static readonly double o = Math.Sqrt(2);

        private static Color CalcCol(double J, double i)
        {
            double r, f, fh, fl, c, a;
            int s;
            Color cc;

            // Tree-radius
            r = 0.1 + (i - Math.Truncate(i / 10) * 7) - Math.Truncate(i / 40) * (i - 31);

            // flame-data
            f = gc * 80 - J - Math.Sin(Math.Truncate(i / 4) * gc * Math.PI + o) * Math.Truncate(i / 4) * 2;
            fh = 0.2 - Math.Sin(i * 0.5 * Math.PI) * 1.1;
            fl = -0.2 - Math.Abs(Math.Sin(i * 0.25 * Math.PI) * 2.3);
            s = 1;
            if (Math.Sin(Math.Truncate(i / 4) * 4.9) < 0)
                s = -1;

            // candle-data
            c = gc * 80 - J - Math.Sin(Math.Truncate(i / 4) * gc * Math.PI - gc * Math.PI + o) * (Math.Truncate(i / 4) * 2 - 2);

            // apple-data
            a = gc * 80 - J - Math.Cos(Math.Truncate(i / 5) * gc * Math.PI + o) * Math.Truncate(i / 5) * 2.5;

            // Compute color-data
            if ((i >= 8) && (i < 36) && (f * s < fh) && (f * s > fl))
            {
                int g = (int)Math.Truncate((1 - (fh * 0.2 - fl * 0.2 - Math.Abs(f * s * 2 - fl - fh) * 0.2)) * 255);
                cc = Color.FromRgb(255, Clamp(g), 0);
            }
            else if ((i >= 12) && (i < 40) && (Math.Abs(c) <= 1))
            {
                int val = (int)Math.Truncate((c + 1) * 100);
                cc = Color.FromRgb(Clamp(val), Clamp(val), 0);
            }
            else if ((i >= 5) && (i < 45) && (Math.Abs(a) <= 0.1 + Math.Sqrt(Math.Max(0, 1 - Math.Pow(i / 2.5 - Math.Truncate(i / 5) * 2 - 1, 2))) * 2.5))
            {
                int val = (int)Math.Truncate((a - i + Math.Truncate(i / 5) * 5 + 6.2) * 35);
                cc = Color.FromRgb(Clamp(val), 0, 0);
            }
            else if (Math.Abs(gc * 80 - J) > r)
            {
                double term1 = Math.Truncate(i / (gc * 50));
                double absTerm = Math.Abs(i - gc * 50);
                
                int rVal = (int)Math.Truncate((1 - term1) * absTerm * 3);
                int gVal = (int)Math.Truncate(term1 * (128 + absTerm * 5));
                int bVal = (int)Math.Truncate((1 - term1) * 160 + absTerm * 3);
                
                cc = Color.FromRgb(Clamp(rVal), Clamp(gVal), Clamp(bVal));
            }
            else
            {
                int rVal = (int)Math.Truncate(Math.Truncate(i / 40) * 128);
                int gVal = (int)Math.Truncate(((gc * 80 + r - J) / r) * 127);
                cc = Color.FromRgb(Clamp(rVal), Clamp(gVal), 0);
            }
            return cc;
        }

        private static byte Clamp(int val)
        {
            if (val < 0) return 0;
            if (val > 255) return 255;
            return (byte)val;
        }

        public static BitmapSource Render(int width, int height)
        {
            WriteableBitmap bm = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            
            double yf = 50.0 / height;
            double xf = 80.0 / width;
            
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            
            for (int y = 0; y < height; y++)
            {
                double i = y * yf;
                for (int x = 0; x < width; x++)
                {
                    double J = x * xf;
                    Color c = CalcCol(J, i);
                    
                    int index = y * stride + x * 4;
                    pixels[index] = c.B;
                    pixels[index + 1] = c.G;
                    pixels[index + 2] = c.R;
                    pixels[index + 3] = 255;
                }
            }
            
            bm.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            
            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawImage(bm, new Rect(0, 0, width, height));
                
                double fontSize = height / 8.0;
                
                var merryText = new FormattedText(
                    "Merry",
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    fontSize,
                    Brushes.Red, 
                    VisualTreeHelper.GetDpi(dv).PixelsPerDip);
                
                dc.DrawText(merryText, new Point(height / 25.0, Math.Truncate(height * gc)));
                
                var xmasText = new FormattedText(
                    "Xmas",
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    fontSize,
                    Brushes.Red,
                    VisualTreeHelper.GetDpi(dv).PixelsPerDip);
                    
                dc.DrawText(xmasText, new Point(height / 3.0, (height * 7.0) / 9.0));
            }
            rtb.Render(dv);
            return rtb;
        }
    }
}
