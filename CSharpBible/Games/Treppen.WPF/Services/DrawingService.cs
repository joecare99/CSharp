using System.Windows.Media;
using System.Windows.Media.Imaging;
using Treppen.Base;
using Treppen.WPF.Services.Interfaces;

namespace Treppen.WPF.Services;

public class DrawingService : IDrawingService
{
    public BitmapSource CreateLabyrinthPreview(IHeightLabyrinth labyrinth)
    {
        int width = labyrinth.Dimension.Width;
        int height = labyrinth.Dimension.Height;
        var wb = new WriteableBitmap(width*5, height*5, 96, 96, PixelFormats.Bgra32, null);
        var pixels = new Int32[width*5 * height*5];

        int min = int.MaxValue;
        int max = int.MinValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var h = labyrinth[x, y];
                if (h < min) min = h;
                if (h > max) max = h;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int lab_x = width - x - 1;
                int lab_y = height - y - 1;

                var h = labyrinth[lab_x, lab_y];
                int color = ((byte)(h  * 128 / max )*(1<<16)+ (byte)(h * 128 / max)*(1<<8)+ (byte)(h * 128 / max+64))+(int)(0xff<<24);
                // Fülle ein 5x5 Pixel großes Quadrat
                for (int dy = 0; dy < 5; dy++)
                {
                    for (int dx = 0; dx < 5; dx++)
                    {
                        pixels[((y * 5) + dy) * (width * 5) + ((x * 5) + dx)] = color;
                    }
                }
                

                // Prüfe linken Nachbarn
                if (lab_x < width - 1)
                {
                    var h_right = labyrinth[lab_x + 1, lab_y];
                    if (Math.Abs(h - h_right) <= 1)
                    {
                        // Zeichne eine horizontale weiße Linie am unteren Rand des 5x5 Quadrats
                        for (int dx = 0; dx <= 5; dx++)
                        {
                            pixels[((y * 5) + 2) * (width * 5) + (x * 5) - dx + 2] = -1;
                        }
                    }
                }

                // Prüfe unteren Nachbarn
                if (lab_y < height-1 )
                {
                    var h_down = labyrinth[lab_x, lab_y + 1];
                    if (Math.Abs(h - h_down) <= 1)
                    {
                        // Zeichne eine vertikale weiße Linie am rechten Rand des 5x5 Quadrats
                        for (int dy = 0; dy <= 5; dy++)
                        {
                            pixels[((y * 5) - dy + 2) * (width * 5) + (x * 5) + 2] = -1;
                        }
                    }
                }
            }
        }

        wb.WritePixels(new System.Windows.Int32Rect(0, 0, width*5, height*5), pixels, width*5*sizeof(int)  , 0);
        return wb;
    }
}
