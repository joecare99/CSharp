// Zweck: Auslagerung der Welt-Erstellung (vormals InitWorld) in eine dedizierte Klasse.
// Plan (Pseudocode):
// - Klasse GameWorld mit Property Objects (Obstacle[]).
// - Im Konstruktor: zufällige Hindernisse erzeugen (Box/Cylinder) inkl. Position, Größe, Farbe.
// - Pfad-Scan (wie zuvor in InitWorld): entlang der PathFunction mögliche Kollisionen mit dem Pfad prüfen und Objekte ggf. auf Größe 0 setzen.
// - Eigene private PathFunction (gleiche Implementierung) innerhalb GameWorld, damit ViewModel unabhängig bleibt.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace BoxFlight.World;

public sealed class GameWorld : IGameWorld
{
    public Obstacle[] Objects { get; private set; } = Array.Empty<Obstacle>();

    // Internal state (camera, view, spatial index)
    private Point _cPoint, _cPoint2, _cPoint3;
    private Vector _cView, _cMove;
    private readonly Dictionary<(int X, int Y), List<int>> _objIndex = new();

    public void Initialize()
    {
        var rnd = new Random();
        Objects = new Obstacle[1600];
        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i] = (rnd.Next(2) == 0) ? new Cylinder() : new Box();
            var o = Objects[i];
            o.Pos = new Point((i / 40) * 10 + rnd.Next(16), (i % 40) * 10 + rnd.Next(16));
            o.Size = rnd.Next(4) + 3;
            var c = (((int)o.Pos.X) ^ ((int)o.Pos.Y)) % 50;
            o.Color = Color.FromRgb((byte)(100 + c * 3), (byte)(250 - Math.Abs(25 - c) * 3), (byte)(250 - c * 3));
        }
        _cPoint = PathFunction(0);

        // Remove intersecting objects along path
        for (int k = 0; k <= 2000; k++)
        {
            var pp = PathFunction(k * Math.PI / 500.0);
            foreach (var o in Objects)
            {
                var vp = new Vector(o.Pos.X, o.Pos.Y) + (Vector)pp + new Vector(400, 400);
                vp -= new Vector(Math.Floor(vp.X / 400.0) * 400.0, Math.Floor(vp.Y / 400.0) * 400.0);
                vp -= new Vector(200, 200);
                if (Math.Abs(vp.X) + Math.Abs(vp.Y) < 5 + o.Size * 2)
                    o.Size = 0;
            }
        }

        BuildIndex();
    }

    public void Step(int time, bool stereo, int scrollOffset, int focusX, int maxHeight, RenderEntry[] render,
                     out Point cPoint2, out Point cPoint3)
    {
        const double freq = 4.0;
        const double sqrt2 = 0.7071067811865476;
        double omega = time * freq * Math.PI * 2.0 / 6000.0;

        var cMoveOld = _cPoint;
        _cPoint = PathFunction(omega);
        var lcv1p = PathFunction(omega + scrollOffset / 1000.0);
        var lcv2p = PathFunction(omega + scrollOffset / 1000.0 + 0.01);
        _cView = new Vector(lcv2p.X - lcv1p.X, lcv2p.Y - lcv1p.Y);
        var ll = _cView.Length; if (ll > 0) _cView /= ll;

        var lCv1 = new Vector(_cView.X * sqrt2 - _cView.Y * sqrt2, _cView.Y * sqrt2 + _cView.X * sqrt2);
        var lCv2 = new Vector(lCv1.Y, -lCv1.X);

        _cMove = _cPoint - cMoveOld;
        _cPoint.X -= Math.Truncate(_cPoint.X / 400.0) * 400.0;
        _cPoint.Y -= Math.Truncate(_cPoint.Y / 400.0) * 400.0;

        var sPnt = _cPoint;
        if (stereo) { _cPoint.X += _cView.Y * 0.4; _cPoint.Y += -_cView.X * 0.4; }
        else        { _cPoint.X += -_cView.Y * 0.4; _cPoint.Y +=  _cView.X * 0.4; }
        sPnt.X += _cView.X; sPnt.Y += _cView.Y;

        foreach (var o in Objects)
        {
            o.Visible = false; o.Hit = false;
            if (o.Size <= 0) continue;
            var vp = new Vector(o.Pos.X, o.Pos.Y) + (Vector)_cPoint + new Vector(400, 400);
            vp -= new Vector(Math.Floor(vp.X / 400.0) * 400.0, Math.Floor(vp.Y / 400.0) * 400.0);
            vp -= new Vector(200, 200);
            o.Vp = new Point(vp.X, vp.Y);
            bool vis = (vp.X * _cView.X + vp.Y * _cView.Y) < -o.Size + 1;
            vis &= (vp.X * lCv1.X + vp.Y * lCv1.Y) < +o.Size + 1;
            vis &= (vp.X * lCv1.X + vp.Y * lCv1.Y) > -200;
            vis &= (vp.X * lCv2.X + vp.Y * lCv2.Y) < +o.Size + 1;
            vis &= (vp.X * lCv2.X + vp.Y * lCv2.Y) > -200;
            o.Visible = vis;
        }

        int half = render.Length / 2;
        int hrc2 = half / 2;
        int obj = -1;
        for (int j = 0; j < render.Length; j++)
        {
            double s, c;
            if (j <= half) { s = Math.Sin((hrc2 - j) * 0.6 / hrc2); c = Math.Cos((hrc2 - j) * 0.6 / hrc2); }
            else           { s = Math.Sin((-j + hrc2 * 3) * 0.6 / hrc2); c = Math.Cos((-j + hrc2 * 3) * 0.6 / hrc2); }
            var renDir = new Vector(_cView.X * c + _cView.Y * s, -_cView.X * s + _cView.Y * c);
            render[j] = Trace(maxHeight, sPnt, obj, renDir, j == focusX);
        }

        if (stereo) { _cPoint.X += -_cView.Y * 0.8; _cPoint.Y += _cView.X * 0.8; }
        else        { _cPoint.X +=  _cView.Y * 0.8; _cPoint.Y += -_cView.X * 0.8; }

        cPoint2 = _cPoint2; cPoint3 = _cPoint3;
    }

    private RenderEntry Trace(int maxHeight, Point sPnt, int obj, Vector renDir, bool hit)
    {
        float rDistQ, r;
        double rDist; double rr = 0; int hObj = obj;
        var entry = new RenderEntry { Oo1 = -1, Oo2 = -1, Light = 0, Height = 0, Light2 = 0, Height2 = 0, Shad = 0 };
        if (obj >= 0 && Objects[obj].HitTest(_cPoint, renDir, out rDistQ, out r) && rDistQ > 2 && rDistQ < 300) { rDist = rDistQ; rr = r; }
        else { obj = -1; rDist = 300.0; }

        ObjHit(_cPoint, renDir, -1, ref obj, ref rDist, ref rr);
        if (obj >= 0)
        {
            var o = Objects[obj];
            o.Hit = true;
            var renDir2 = o.Reflect(renDir, (float)rr, out var xxa);
            double ll = 96.0 + 96.0 * xxa;
            double ll2 = Math.Pow(4.0 - 4.0 * renDir2.Y, 4) - 4096.5 + 256.0;
            ll = Math.Max(ll, ll2);
            entry.Oo1 = obj; entry.Light = (int)ll; entry.Height = (int)(maxHeight / (rDist + 1.0)); entry.Color = o.Color;
            if (ll <= 192.0)
            {
                var cPoint2 = new Point(_cPoint.X + renDir.X * rDist, _cPoint.Y + renDir.Y * rDist);
                if (hit) _cPoint2 = cPoint2;
                double rDist2 = 300.0 - rDist; int obj2 = -1; double rr2 = 0.0; ObjHit(cPoint2, renDir2, obj, ref obj2, ref rDist2, ref rr2);
                if (obj2 >= 0)
                {
                    Objects[obj2].Hit = true;
                    var renDir3 = Objects[obj].Reflect(renDir2, (float)rr, out var xxa2);
                    ll = 32.0 + 32.0 * xxa2 + ll;
                    ll2 = Math.Pow(4.0 - 4.0 * renDir3.Y, 4) - 4096.5 + 256.0;
                    ll = Math.Max(ll, ll2);
                    entry.Oo2 = obj2; entry.Light2 = (int)ll; entry.Height2 = (int)(maxHeight / (rDist + rDist2 + 1.0)); entry.Color2 = Objects[obj2].Color;
                    var cPoint3 = new Point(cPoint2.X + renDir2.X * Math.Max(rDist2, 5.0), cPoint2.Y + renDir2.Y * Math.Max(rDist2, 5.0));
                    if (hit) _cPoint3 = cPoint3;
                }
                else { entry.Light2 = 0; entry.Height2 = 0; }
            }
            else { entry.Light2 = 0; entry.Height2 = 0; entry.Oo2 = -1; if (hit) _cPoint3 = new Point(_cPoint2.X + renDir2.X * 40.0, _cPoint2.Y + renDir2.Y * 40.0); }
        }
        else { entry.Light = 0; entry.Height = 0; entry.Light2 = 0; entry.Height2 = 0; entry.Oo1 = -1; entry.Oo2 = -1; if (hit) _cPoint2 = new Point(_cPoint.X + renDir.X * 200.0, _cPoint.Y + renDir.Y * 200.0); }

        int lsobj = hObj; double shDist = 300.0; double shRr = 0.0;
        ObjHit(sPnt, renDir, -1, ref lsobj, ref shDist, ref shRr);
        if (lsobj >= 0) entry.Shad = (int)(maxHeight / (shDist + 1.0)); else entry.Shad = 0;
        return entry;
    }

    private void ObjHit(Point cPoint, Vector renDir, int ignObj, ref int obj, ref double rDist, ref double rr)
    {
        for (int dd = 0; dd <= 50; dd++)
        {
            int cx = (int)Math.Truncate(600 - cPoint.X - renDir.X * dd * 9); cx = ((cx % 400) + 400) % 400;
            int cy = (int)Math.Truncate(600 - cPoint.Y - renDir.Y * dd * 9); cy = ((cy % 400) + 400) % 400;
            foreach (var i in GetIndex(new Point(cx, cy)))
            {
                var o = Objects[i]; o.Visible = true; if (i == ignObj) continue;
                if (o.HitTest(cPoint, renDir, out var rDistQ, out var r) && rDistQ > 0.01f && rDistQ < rDist)
                { obj = i; rDist = rDistQ; rr = r; }
            }
            if ((int)rDist < (dd - 1) * 8) break;
        }
    }

    private IEnumerable<int> GetIndex(Point p)
    {
        int xx = (int)p.X / 10; int yy = (int)p.Y / 10;
        if (xx < 0 || xx >= 40 || yy < 0 || yy >= 40) yield break;
        if (_objIndex.TryGetValue((xx, yy), out var list)) { foreach (var i in list) yield return i; }
    }

    private void AppendIndex(Point p, int i)
    {
        int xx = (int)p.X / 10; int yy = (int)p.Y / 10; var key = (xx, yy);
        if (!_objIndex.TryGetValue(key, out var list)) { list = new List<int>(); _objIndex[key] = list; }
        if (!list.Contains(i)) list.Add(i);
    }

    private void BuildIndex()
    {
        _objIndex.Clear();
        for (int j = 0; j < Objects.Length; j++)
        {
            var o = Objects[j]; if (o.Size <= 0) continue;
            AppendIndex(o.Pos, j);
            AppendIndex(new Point((o.Pos.X + o.Size) % 400, (o.Pos.Y + o.Size) % 400), j);
            AppendIndex(new Point(o.Pos.X, (o.Pos.Y + o.Size) % 400), j);
            AppendIndex(new Point((o.Pos.X - o.Size + 400) % 400, (o.Pos.Y + o.Size) % 400), j);
            AppendIndex(new Point((o.Pos.X - o.Size + 400) % 400, o.Pos.Y), j);
            AppendIndex(new Point((o.Pos.X - o.Size + 400) % 400, (o.Pos.Y - o.Size + 400) % 400), j);
            AppendIndex(new Point(o.Pos.X, (o.Pos.Y - o.Size + 400) % 400), j);
            AppendIndex(new Point((o.Pos.X + o.Size) % 400, (o.Pos.Y - o.Size + 400) % 400), j);
            AppendIndex(new Point((o.Pos.X + o.Size) % 400, o.Pos.Y), j);
        }
    }

    private static Point PathFunction(double omega)
    {
        return new Point(
            -Math.Cos(omega) * 200.0,
            (Math.Sin(omega) * Math.Sin(-Math.Cos(omega) * Math.PI * 0.5) * 5.5 + omega) / Math.PI / 2.0 * 200.0
        );
    }
}