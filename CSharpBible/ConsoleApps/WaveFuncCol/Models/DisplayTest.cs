using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using ConsoleDisplay.View;
using DisplayTest.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using WaveFunCollapse.Models.Data;

namespace DisplayTest.Models;

public class DisplayTest : IDisplayTest
{

    public IConsole? console { get => Display.myConsole; set => Display.myConsole = value; }
    public Func<ConsoleColor[]>? D1SBuffer { get; set; }
    public Func<ConsoleColor[]>? D2SBuffer { get; set; }
    public Func<ConsoleColor[]>? D3SBuffer { get; set; }
    public Func<string, Bitmap?>? LoadImage { get; set; }
    public Action<int, int, ConsoleColor>? D1PutPixel { get; set; }
    public Action<int, int, byte, byte, byte>? D2PutPixelC { get; set; }
    public Action<int, int, byte, byte, byte>? D1PutPixelC { get; set; }
    public Action? DUpdate { get; set; }

    const int D1Width = 9;
    const int D2Width = 27;

    public void ShowImage1(string filename)
    {
        var bitmap = LoadImage(filename);
        if (bitmap == null)
            return;
        var rBitmap = new Bitmap(bitmap, new Size(D1Width, D1Width));
        for (int i = 0; i < D1Width; i++)
            for (int j = 0; j < D1Width; j++)
            {
                Color c = rBitmap.GetPixel(i, j);
                D1PutPixelC(i, j, c.R, c.G, c.B);
            }
        DUpdate();
    }

    public void ShowImage2(string filename)
    {
        var bitmap = LoadImage(filename);
        if (bitmap == null)
            return;
        var rBitmap = new Bitmap(bitmap, new Size(D2Width, D2Width));
        for (int i = 0; i < D2Width; i++)
            for (int j = 0; j < D2Width; j++)
            {
                Color c = rBitmap.GetPixel(i, j);
                D2PutPixelC(i, j, c.R, c.G, c.B);
            }
        DUpdate();
    }
    private Dictionary<Color, int> GroupColors(Bitmap? bitmap)
    {
        var colorGroups = new Dictionary<Color, int>();

        for (int i = 0; i < bitmap.Width; i++)

            for (int j = 0; j < bitmap.Height; j++)
            {
                var c = bitmap.GetPixel(i, j);
                if (!colorGroups.ContainsKey(c))
                {
                    colorGroups[c] = 0;
                }
                colorGroups[c] += 1;
            }


        foreach (var group in colorGroups)
        {
            console.WriteLine($"Color: {group.Key}, Pixels: {group.Value}");
        }
        return colorGroups;
    }

    public void AnalyseImage(string filename)
    {
        ShowImage1(filename);
        var bitmap = LoadImage(filename);
        if (bitmap == null)
            return;

        var gc = GroupColors(bitmap);

        List<Point> ttileRelKoor = [
            new(-1, -1), new(0, -1), new(1, -1),
            new(-1, 0), new(0,0), new(1,0),
            new(-1,1),new(0,1),new(1,1)
        ];

        List<TileData> lTile = new();

            for (int j = 0; j < bitmap.Height; j++)
        for (int i = 0; i < bitmap.Width; i++)
            {
                // Find tiles
                var tOrgdata = new Point(i, j);
                IList<int> tci = new List<int>();
                // Find groupindex of pixelcolor
                foreach (var ttile in ttileRelKoor)
                {
                    var tRelData = tOrgdata + new Size(ttile);
                    if (tRelData.X < 0 || tRelData.X >= bitmap.Width)
                    {
                        tRelData.X = (tRelData.X+ bitmap.Width) % bitmap.Width;
                        tRelData.Y = (tRelData.Y+bitmap.Height / 2) % bitmap.Height;
                    }
                    if (tRelData.Y < 0 || tRelData.Y >= bitmap.Height)
                    {
                        tRelData.Y = (tRelData.Y+ bitmap.Height) % bitmap.Height;
                        tRelData.X = (tRelData.X + bitmap.Width / 2) % bitmap.Width;
                    }
                    tci.Add(gc.Keys.IndexOf(bitmap.GetPixel(tRelData.X, tRelData.Y)));
                }

                var iHash = tci.Aggregate(0L, (l, i) => unchecked(l * 257L) ^ i.GetHashCode());
                var lTileF = lTile.Where(t => t.LTHash == iHash);
                if (lTileF.Count() != 0)
                {
                    var flag = false;
                    foreach (var t in lTileF)
                    {
                        if (t.OrgData is Tuple<Point, IList<int>> p
                            && p.Item2.Count() == tci.Count
                            && tci.Zip(p.Item2, (i1, i2) => i1 == i2).All(e => e == true))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        continue;
                }
                lTile.Add(new TileData(lTile.Count, new Tuple<Point, IList<int>>(tOrgdata, tci), iHash));
            }

        var s = 0;
        foreach (var t in lTile)
        {
            if (t.OrgData is Tuple<Point, IList<int>> p)
            {
                var sp = new Point((s * 3) % D2Width + 1, ((s * 3) / D2Width) * 3 + 1);
                foreach (var ttile in ttileRelKoor.Zip(p.Item2, (i1, i2) => (i1, i2)))
                {
                    var tRelData = sp + new Size(ttile.i1);
                    D2PutPixelC(tRelData.X, tRelData.Y, gc.Keys.ElementAt(ttile.i2).R, gc.Keys.ElementAt(ttile.i2).G, gc.Keys.ElementAt(ttile.i2).B);
                }
            }
            s++;
        }
        DUpdate();
    }

    public void DisplayTest3(IRandom rnd)
    {
        Display.myConsole.Title = "Test3";
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < D1SBuffer().Length; j++)
                D1SBuffer()[j] = (ConsoleColor)(Math.Round(Math.Sin((j % D1Width - 10) * (0.05 + Math.Sin(i * 0.6) * 0.04) + i * 0.05) * 16 + Math.Cos((j / D1Width - 10) * (0.05 + Math.Sin(i * 0.55) * 0.04) + i * 0.05) * 16 + 32) % 16);

            for (int j = 0; j < D2SBuffer().Length; j++)
            {
                int r = (int)Math.Round(1.25 + 0.5 * rnd.NextDouble() + Math.Sin((j % D2Width - i * 0.4) * -0.6) * 1.5);
                int g = (int)Math.Round(1.25 + 0.5 * rnd.NextDouble() + Math.Sin((j / D2Width + (j % D2Width) / 2 + i * 0.4) * 0.6) * 1.5);
                int b = (int)Math.Round(1.25 + 0.5 * rnd.NextDouble() + Math.Sin((j / D2Width - (j % D2Width) / 2 - i * 0.4) * 0.6) * 1.5);
                D2PutPixelC((j / D2Width), (j % D2Width), (byte)(r * 64), (byte)(g * 64), (byte)(b * 64));
            }

            D3SBuffer()[i / 2 + (((i + 1) / 2) % 2) * 50] = ConsoleColor.Green;

            DUpdate();
            Thread.Sleep(40);
        }
    }

    /// <summary>
    /// Test2:<br />
    /// <list type="bullet"><item>Vertical Moving Color-Dots on the first display </item><item>Color-Plasma on the second display</item><item>Filling the third display (gauge) with red.
    /// </item></list></summary>
    public void DisplayTest2()
    {
        Display.myConsole.Title = "Test2";
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < D1SBuffer().Length; j++)
                D1PutPixel((j / D1Width), (j % D1Width), (ConsoleColor)((i + j) % 16));

            for (int j = 0; j < D2SBuffer().Length; j++)
                D2SBuffer()[j] = (ConsoleColor)(Math.Round(Math.Sin((j % D2Width - 5) * (0.07 + Math.Sin(i * 0.5) * 0.05) + i * 0.05) * 16 + Math.Cos((j / D2Width - 5) * (0.07 + Math.Sin(i * 0.5) * 0.05) + i * 0.05) * 16 + 32) % 16);

            D3SBuffer()[i / 2 + ((i / 2) % 2) * 50] = ConsoleColor.Red;

            DUpdate();
            Thread.Sleep(40);
        }
    }

    /// <summary>
    /// Test1:<br />
    /// <list type="bullet">
    /// <item>Horizontal Moving Color-Dots on the first display</item>
    /// <item>Random Dots on second display</item>
    /// <item>Filling the third display (gauge) with yellow.</item>
    /// </list>
    /// </summary>
    /// <param name="rnd">The random.</param>
    /// <autogeneratedoc />
    public void DisplayTest1(IRandom rnd)
    {
        //
        Display.myConsole.Title = "Test1";
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < D1SBuffer().Length; j++)
                D1SBuffer()[j] = (ConsoleColor)((i + j) % 16);

            for (int j = 0; j < 5; j++)
                D2SBuffer()[rnd.Next(D2SBuffer().Length)] = (ConsoleColor)((i / D2Width) % 16);

            D3SBuffer()[i / 2 + (i % 2) * 50] = ConsoleColor.Yellow;

            DUpdate();
            Thread.Sleep(40);
        }
    }

}
