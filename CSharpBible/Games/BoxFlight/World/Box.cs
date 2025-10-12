using System.Windows;

namespace BoxFlight.World;

public sealed class Box : Obstacle
{
    public override bool HitTest(Point cPoint, Vector renDir, out float rDistQ, out float r)
    {
        var vp = new Vector(Pos.X, Pos.Y) + (Vector)cPoint + new Vector(400, 400);
        vp -= new Vector(Math.Floor(vp.X / 400.0) * 400.0, Math.Floor(vp.Y / 400.0) * 400.0);
        vp -= new Vector(200, 200);

        r = 0f;
        if (Math.Abs(renDir.X) > 1e-30)
        {
            rDistQ = (float)((-vp.X - Size * Math.Sign(renDir.X)) / renDir.X);
            var t1 = renDir.Y * rDistQ;
            var ok = Math.Abs(t1 + vp.Y) < Size;
            if (ok) { r = 0.5f; return true; }
        }
        if (Math.Abs(renDir.Y) > 1e-30)
        {
            rDistQ = (float)((-vp.Y - Size * Math.Sign(renDir.Y)) / renDir.Y);
            var t1 = renDir.X * rDistQ;
            var ok = Math.Abs(t1 + vp.X) < Size;
            if (ok) { r = -0.5f; return true; }
        }
        rDistQ = 0f;
        return false;
    }

    public override Vector Reflect(Vector renDir, float rr, out float xx)
    {
        var v = renDir;
        if (rr > 0) { v.X = -v.X; xx = (float)-Math.Sign(renDir.X); }
        else { v.Y = -v.Y; xx = 0f; }
        return v;
    }
}
