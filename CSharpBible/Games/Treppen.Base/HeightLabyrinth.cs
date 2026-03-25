using System;
using System.Drawing;
using System.IO;
using MathLibrary.TwoDim;
using static MathLibrary.TwoDim.Directions2D;

namespace Treppen.Base;

/// <summary>
/// Height-based labyrinth engine ported from Pascal THeightLaby.
/// </summary>
public sealed class HeightLabyrinth : IHeightLabyrinth
{
    /*
    Pseudocode/Plan:
    - Ziel: Das Seed-Pattern (Signatur) aus Pascal (FPrest2) analog mittels SetLData in das Raster _z schreiben.
      • Das Pattern besteht aus Ziffern und '.'-Zeichen; in Pascal wird jedes Zeichen in eine Zahl umgewandelt:
        val = Ord(char) - Ord('0') + Offset. Für '.' (ASCII 46) mit Offset=2 ergibt das 0 => "unbesetzt".
      • Die Indizierung in Pascal liest Sdat rückwärts:
        index = (xm*ym) - y*xm - x, wobei Strings 1-basiert sind.
        In C# (0-basiert) wird daraus: idx0 = index - 1 = (xm*ym) - y*xm - x - 1.
      • Schleifenbereiche: x in [0..xm-1], y in [0..ym-1]; schreiben nach _z[x,y].
      • Sicherheitsgrenzen: Nur schreiben, wenn (x < _dimension.Width && y < _dimension.Height).
    - Implementierung:
      • private const string FPrest2 = "4465452.
                                        7575331.
                                        6566211.
                                        21326510
                                        30213422
                                        43312434";
      • private void SetLData(string sdat, int xm, int ym, int offset)
         - prüfe sdat.Length >= xm*ym
         - für x=0..xm-1
           für y=0..ym-1
             idx0 = (xm*ym) - y*xm - x - 1
             val = (sdat[idx0] - '0') + offset
             wenn InBounds(x,y) dann _z[x,y] = val
      • In Generate():
         - Nach Array.Clear(_z, 0, _z.Length) SetLData(FPrest2, 8, 6, 2) aufrufen.
      • Rest der Logik bleibt unverändert.
    */

    private Rectangle _dimension;
    private int[,] _z;
    private readonly Random _rnd = new();

    // Seed-Pattern (Signatur) wie im Pascal-Original
    // private const string FPrest2 = "4465452.7575331.6566211.213265103021342243312434";
    private readonly uint[] FPrest3 = [0x7645452E,
                                       0x5746331E,
                                       0x6655211E,
                                       0x44125510,
                                       0x35213422
                                      ,0x32312434];

    public event Action<object, Point>? UpdateCell;

    public Rectangle Dimension
    {
        get => _dimension;
        set { _dimension = value; _z = new int[value.Width, value.Height]; }
    }

    public int this[int x, int y] => InBounds(x, y) ? _z[x, y] : 0;
    private bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < _dimension.Width && y < _dimension.Height;

    public int BaseLevel(int x, int y) => (int)Math.Truncate((x / 1.3 + y / 1.3) + 1);

    // Mappt die kompakten Seed-Daten in das Raster, wie in Pascal SetLData
    private void SetLData(string sdat, int xm, int ym, int offset)
    {
        if (string.IsNullOrEmpty(sdat)) return;
        int total = xm * ym;
        if (sdat.Length < total) return;

        for (int x = 0; x < xm; x++)
        {
            for (int y = 0; y < ym; y++)
            {
                int idx0 = total - (y * xm) - x - 1; // 0-basierte Entsprechung zu Pascal
                char ch = sdat[idx0];
                int val = (ch - '0') + offset; // '.' -> -2 + 2 => 0
                if (InBounds(x, y))
                    _z[x, y] = val;
            }
        }
    }

    private void SetLData2(uint[] data, int xm, int ym, int offset)
    {
        if (data.Length < ym - 1) return;

        for (int x = 0; x < xm; x++)
        {
            for (int y = 0; y < ym; y++)
            {
                var nibble = (int)((data[ym-y-1] >> 4 * x) & 0x0F);
                int val = (nibble == 14 ? 0 : nibble + offset); // 14='.'→0
                if (InBounds(x, y))
                    _z[x, y] = val;
            }
        }
    }

    public void Generate()
    {
        Array.Clear(_z, 0, _z.Length);

        // Seed-Pattern (Signatur) initial aufbringen
        SetLData2(FPrest3, 8, 6, 2);

        var start = new Point(1, Math.Min(6, Math.Max(0, _dimension.Height - 1)));
        if (!InBounds(start.X, start.Y)) return;

        _z[start.X, start.Y] = BaseLevel(start.X, start.Y) - 1;

        var fifo = new Point[_dimension.Width * _dimension.Height];
        int push = 0, pop = 0;
        var act = start; var stored = act;
        int dirCount = 1;

        while (dirCount != 0 || push >= pop)
        {
            // swap
            (act, stored) = (stored, act);
            UpdateCell?.Invoke(this, act);
            dirCount = 0;
            var pos = new Point[4];
            var hh = new int[4];

            for (int i = 1; i < Dir4.Length; i++)
            {
                pos[dirCount] = new Point(Dir4[i].X, Dir4[i].Y);
                var next = new Point(act.X - pos[dirCount].X, act.Y - pos[dirCount].Y);
                if (InBounds(next.X, next.Y) &&
                    CalcStepHeight(act, next, out var h) &&
                    Math.Abs(h - BaseLevel(next.X, next.Y) - 1) < 3)
                {
                    hh[dirCount] = h; dirCount++;
                }
            }

            if (dirCount > 0)
            {
                var pick = _rnd.Next(dirCount);
                if (dirCount > 1) { fifo[push++] = act; }
                var next = new Point(act.X - pos[pick].X, act.Y - pos[pick].Y);
                act = next; _z[next.X, next.Y] = hh[pick];
            }
            else if (push >= pop)
            { 
                act = fifo[pop++];
            }
        }

        // restliche 0-Zellen heuristisch füllen
        for (int x = (_dimension.Width - 1) | 1; x >= 0; x--)
            for (int y = (_dimension.Height - 1) | 1; y >= 0; y--)
            {
                var pp = new Point(x ^ 1, y ^ 1);
                if (!InBounds(pp.X, pp.Y) || _z[pp.X, pp.Y] != 0) continue;

                bool first = true; int zm = 0, cn = 0, zx = 0, cx = 0;

                for (int i = 1; i < Dir4.Length; i++)
                {
                    var n = new Point(pp.X + Dir4[i].X, pp.Y + Dir4[i].Y);
                    var zz = InBounds(n.X, n.Y) ? _z[n.X, n.Y] : 0;

                    if (zz > 0 && (first || zz <= zm)) { if (zz < zm) cn = 0; zm = zz; if (cn < 2) cn++; }
                    if (zz > 0 && (first || zz >= zx)) { if (zz > zx) cx = 0; zx = zz; if (cx < 2) cx++; first = false; }
                }

                if (zm > 0)
                    _z[pp.X, pp.Y] = (cx == 1 && zx - zm < 6) ? zx + cx : zm - cn;
                else
                    _z[pp.X, pp.Y] = BaseLevel(pp.X, pp.Y);
            }
    }

    private bool CalcStepHeight(Point act, Point next, out int height)
    {
        height = 0;
        var ph = this[act.X, act.Y];
        if (ph == 0 || this[next.X, next.Y] != 0) return false;

        bool canm1 = true, canz = true, canp1 = true;
        IntPoint dd = default;

        for (int i = 1; i < Dir4.Length; i++)
        {
            var t = new Point(next.X + Dir4[i].X, next.Y + Dir4[i].Y);
            if (!(t == act))
            {
                var lb = this[t.X, t.Y];
                canm1 &= (lb == 0) || (lb < ph - 2) || (lb > ph);
                canz  &= (lb == 0) || (lb < ph - 1) || (lb > ph + 1);
                canp1 &= (lb == 0) || (lb < ph)     || (lb > ph + 2);
            }
            else dd = Dir4[i];
        }

        var dr = new IntPoint(-dd.Y, dd.X);

        var left = new Point(next.X - dr.X, next.Y - dr.Y);
        if (InBounds(left.X, left.Y) && this[left.X, left.Y] == 0)
        {
            var fl = new Point(left.X + dd.X, left.Y + dd.Y);
            var lb = InBounds(fl.X, fl.Y) ? this[fl.X, fl.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz  &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);

            var bl = new Point(left.X - dd.X, left.Y - dd.Y);
            lb = InBounds(bl.X, bl.Y) ? this[bl.X, bl.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz  &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);
        }

        var right = new Point(next.X + dr.X, next.Y + dr.Y);
        if (InBounds(right.X, right.Y) && this[right.X, right.Y] == 0)
        {
            var fr = new Point(right.X + dd.X, right.Y + dd.Y);
            var lb = InBounds(fr.X, fr.Y) ? this[fr.X, fr.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz  &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);

            var br = new Point(right.X - dd.X, right.Y - dd.Y);
            lb = InBounds(br.X, br.Y) ? this[br.X, br.Y] : 0;
            canm1 &= (lb == 0) || (lb != ph - 1);
            canz  &= (lb == 0) || (lb != ph);
            canp1 &= (lb == 0) || (lb != ph + 1);
        }

        var blv = BaseLevel(next.X, next.Y);
        if (!(canm1 || canz || canp1)) return false;

        if (canp1 && (blv > ph || (!canz && (!canm1 || blv == ph)))) { height = ph + 1; return true; }
        if (canm1 && (blv < ph || (!canz && (!canp1 || blv == ph)))) { height = ph - 1; return true; }
        if (canz) { height = ph; return true; }

        return false;
    }

    public bool LoadFromStream(Stream stream)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        try
        {
            long startPos = stream.CanSeek ? stream.Position : 0;

            // Erkennung Binary-Header (UTF-16 little endian char[] von BinaryWriter.Write(char[]))
            Span<byte> hdr = stackalloc byte[3];
            int r = stream.Read(hdr);
            bool isBinary = false;
            if (r == hdr.Length)
            {
                Span<byte> magic = stackalloc byte[]
                {
                    (byte)'H',(byte)'L',(byte)'B'
                };
                isBinary = hdr.SequenceEqual(magic);
            }

            if (stream.CanSeek)
                stream.Position = startPos;

            if (isBinary)
            {
                var br = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
                // Header überspringen (8 chars)
                var headerChars = br.ReadChars(8);
                var header = new string(headerChars);
                if (header != "HLB1.00\0") return false;

                int w = br.ReadInt32();
                int h = br.ReadInt32();
                if (w <= 0 || h <= 0) return false;

                Dimension = new Rectangle(0, 0, w, h);

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        if (!InBounds(x, y)) return false;
                        _z[x, y] = br.ReadInt32();
                    }
                }
                return true;
            }
            else
            {
                // JSON laden
                using var doc = System.Text.Json.JsonDocument.Parse(stream);
                var root = doc.RootElement;

                if (!root.TryGetProperty("version", out var sVersion) ||
                    !root.TryGetProperty("width", out var wProp) ||
                    !root.TryGetProperty("height", out var hProp) ||
                    !root.TryGetProperty("data", out var dProp))
                    return false;

                int w = wProp.GetInt32();
                int h = hProp.GetInt32();
                if (w <= 0 || h <= 0) return false;

                Dimension = new Rectangle(0, 0, w, h);

                if (dProp.ValueKind != System.Text.Json.JsonValueKind.Array || dProp.GetArrayLength() != h)
                    return false;

                int y = 0;
                foreach (var rowEl in dProp.EnumerateArray())
                {
                    if (rowEl.ValueKind != System.Text.Json.JsonValueKind.Array || rowEl.GetArrayLength() != w)
                        return false;

                    int x = 0;
                    foreach (var cellEl in rowEl.EnumerateArray())
                    {
                        _z[x, y] = cellEl.GetInt32();
                        x++;
                    }
                    y++;
                }
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    /*
    Pseudocode/Plan:
    - Ziel: Stream-Inhalt als binär (xBinary=true) oder JSON (xBinary=false) speichern.
    - Vorbedingungen: _z ist initialisiert; Dimension gibt Breite/Höhe vor.
    - Binärformat:
      • Verwende BinaryWriter (leaveOpen=true).
      • Schreibe Header: Int32 Width, Int32 Height.
      • Schleife über y=0..Height-1, x=0..Width-1 (stabile Reihenfolge) und schreibe Int32 _z[x,y].
    - JSON-Format:
      • Erzeuge DTO mit:
        { width: int, height: int, data: int[][] }
      • Konvertiere _z (int[,]) zu int[height][width] (Zeilen = y, Spalten = x).
      • Verwende System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(dto) und schreibe in Stream.
      • JsonSerializerOptions: WriteIndented=false für kompakten Output.
    - Ressourcen:
      • Writer nicht disposen, leaveOpen verwenden; Stream bleibt Eigentum des Aufrufers.
    */

    public void SaveToStream(Stream stream, bool xBinary = false)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        int width = _dimension.Width;
        int height = _dimension.Height;

        if (width <= 0 || height <= 0 || _z is null || _z.Length == 0)
            throw new InvalidOperationException("Leeres Labyrinth kann nicht gespeichert werden.");

        if (xBinary)
        {
            using var bw = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
            bw.Write([(byte)'H',(byte)'L', (byte)'B', (byte)'1', (byte)'.', (byte)'0', (byte)'0', (byte)0]); // Header
            bw.Write(width);
            bw.Write(height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bw.Write(_z[x, y]);
                }
            }
            bw.Flush();
            return;
        }

        // JSON-Ausgabe
        var data = new int[height][];
        for (int y = 0; y < height; y++)
        {
            var row = new int[width];
            for (int x = 0; x < width; x++)
            {
                row[x] = _z[x, y];
            }
            data[y] = row;
        }

        var dto = new
        {
            version="1.00",
            width,
            height,
            data
        };

        var json = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(dto, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = false
        });
        stream.Write(json, 0, json.Length);
    }
}
