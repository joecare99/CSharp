using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Run with: dotnet run AsciiClock.cs

Console.OutputEncoding = Encoding.UTF8;
Console.CursorVisible = false;
Console.Clear();

// 5 Spalten, 12 Pixel Höhe pro Ziffer
const int GW = 10;
const int GH = 12;

// 2 Pixel (oben/unten) -> Half-Block-Char
static char PairToChar(bool top, bool bottom)
{
    if (!top && !bottom) return ' ';
    if (top && !bottom) return '▀';
    if (!top && bottom) return '▄';
    return '█';
}

static bool[,] UnpackFontBytesToGlyph(byte[] packedBytes,int o)
{
    var glyph = new bool[GH, GW];

    var bitIndex = 0;
    for (var y = 0; y < GH; y++)
    {
        for (var x = 0; x < GW; x++)
        {
            var byteIndex = bitIndex / 8;
            var innerBitIndex = bitIndex % 8;

            var b = byteIndex < packedBytes.Length ? packedBytes[byteIndex+o*((GH*GW+7)/8)] : (byte)0;
            glyph[y, x] = (b & (1 << innerBitIndex)) != 0;

            bitIndex++;
        }
    }

    return glyph;
}

// Font-Definition: Text-Zeilen
var font = new Dictionary<char, bool[,]>();

var packedBytes = Convert.FromBase64String("/Pj3/OGHH3744Yc/7x8/wIADDz78+AMOOOCA47///vgHPOCAAw8ePHjw4P///vsHDhz48Ac84IAD758/4MADDz748MOOO/7/Dw44/vvvgAP++Ac84IAD758/+PHnwAH//Hf84Yc/7x8///8POODAgwcPHjx48MAB/Pj3/OH/+/f84Yc/7x8//Pj3/OH/+w844IADzx8/AAAAAx54wAAAADDggQcM");
for (int c = 0;c<11;c++)
{
    font["0123456789:"[c]] = UnpackFontBytesToGlyph(packedBytes, c);
}


// Hauptloop
while (true)
{
    var now = DateTime.Now;
    string time = now.ToString("HH:mm:ss");
    Draw(time, font);
    var next = now.AddSeconds(1);
    int sleepMs = (int)(next - DateTime.Now).TotalMilliseconds;
    if (sleepMs < 0) sleepMs = 0;
    Thread.Sleep(sleepMs);
}

static void Draw(string time, Dictionary<char, bool[,]> font)
{
    var firstGlyph = font.Values.First();
    var gh = firstGlyph.GetLength(0);
    var gw = firstGlyph.GetLength(1);

    var sb = new StringBuilder();
    // wir fassen je 2 Pixel-Zeilen zu einer Textzeile zusammen
    for (int row = 0; row < gh; row += 2)
    {
        foreach (char ch in time)
        {
            if (!font.TryGetValue(ch, out var glyph))
            {
                for (int x = 0; x < gw; x++)
                    sb.Append(' ');
            }
            else
            {
                for (int x = 0; x < gw; x++)
                {
                    bool top = row < glyph.GetLength(0) && x < glyph.GetLength(1) && glyph[row, x];
                    bool bottom = row + 1 < glyph.GetLength(0) && x < glyph.GetLength(1) && glyph[row + 1, x];
                    sb.Append(PairToChar(!top, !bottom));
                }
            }

            // fester Zwischenraum: hier ein voller Block
            sb.Append(PairToChar(true, true));
        }
        sb.AppendLine();
    }

    var lines = sb.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    int width = 0;
    foreach (var line in lines)
        if (line.Length > width) width = line.Length;

    string edge = "+" + new string('-', width) + "+";
    Console.SetCursorPosition(0, 0);
    Console.WriteLine(edge);
    foreach (var line in lines)
    {
        if (line.Length == 0) continue;
        Console.WriteLine("|" + line.PadRight(width) + "|");
    }
    Console.WriteLine(edge);
}