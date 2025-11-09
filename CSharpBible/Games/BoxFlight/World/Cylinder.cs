using System.Windows;

namespace BoxFlight.World;

public sealed class Cylinder : Obstacle
{
    public override bool HitTest(Point cPoint, Vector renDir, out float rDistQ, out float r)
    {
        var vp = new Vector(Pos.X, Pos.Y) + (Vector)cPoint + new Vector(400, 400);
        vp -= new Vector(Math.Floor(vp.X / 400.0) * 400.0, Math.Floor(vp.Y / 400.0) * 400.0);
        vp -= new Vector(200, 200);

        r = (float)(vp.X * renDir.Y - vp.Y * renDir.X);
        var ld = (vp.X * renDir.X + vp.Y * renDir.Y);
        var res = (Math.Abs(r) < Size) && (ld < 0);
        if (res)
            rDistQ = (float)(Math.Sqrt(vp.X * vp.X + vp.Y * vp.Y) - Math.Sqrt(Size * Size - r * r));
        else
            rDistQ = 0f;
        return res;
    }

    public override Vector Reflect(Vector renDir, float rr, out float xx)
    {
        double reflOm = Math.Abs(rr) > Size ? Math.Sign(rr) * Math.PI : Math.Asin(rr / Size) * 2.0;
        var c = Math.Cos(reflOm);
        var s = Math.Sin(reflOm);
        var nx = -renDir.X * c + renDir.Y * s;
        var ny = -renDir.Y * c - renDir.X * s;
        var c2 = Math.Cos(reflOm * 0.5);
        var s2 = Math.Sin(reflOm * 0.5);
        xx = (float)(-renDir.X * c2 + renDir.Y * s2);
        return new Vector(nx, ny);
    }
}
