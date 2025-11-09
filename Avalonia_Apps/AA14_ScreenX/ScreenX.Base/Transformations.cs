using System;

namespace ScreenX.Base;

public static class Transformations
{
    static double PQLength(in ExPoint p) => p.X * p.X + p.Y * p.Y;
    static double PLength(in ExPoint p) => Math.Sqrt(PQLength(p));
    static double ArcSinXp(in ExPoint p)
    {
        if (p.X > 0) return Math.Atan(p.Y / p.X);
        if (p.X < 0)
        {
            if (p.Y >= 0) return Math.Atan(p.Y / p.X) + Math.PI;
            else return Math.Atan(p.Y / p.X) - Math.PI;
        }
        if (p.Y > 0) return 0.5 * Math.PI;
        if (p.Y < 0) return -0.5 * Math.PI;
        return 0;
    }

    public static ExPoint Null(ExPoint p, ExPoint p0, ref bool brk) => p;

    public static ExPoint Strflucht(ExPoint p, ExPoint p0, ref bool brk)
    {
        if (p.Y == 0) return new ExPoint(0, 0);
        if (p.Y > 0) return new ExPoint(p.X * 4 / p.Y, 12 - (32 / p.Y));
        else return new ExPoint(p.X * 4 / p.Y, -12 - (32 / p.Y));
    }

    public static ExPoint Ballon(ExPoint p, ExPoint p0, ref bool brk)
    {
        if (p.X == 0 && p.Y == 0) return new ExPoint(0, 0);
        var force = 1 - 0.3 / (PQLength(p) / 100 + 0.3);
        return new ExPoint(p.X * force, p.Y * force);
    }

    public static ExPoint Sauger(ExPoint p, ExPoint p0, ref bool brk)
    {
        if (p.X == 0 && p.Y == 0) return new ExPoint(0, 0);
        var force = 1 + 0.3 / (PQLength(p) / 100 + 0.1);
        return new ExPoint(p.X * force, p.Y * force);
    }

    public static ExPoint Tunnel(ExPoint p, ExPoint p0, ref bool brk)
    {
        var y = PLength(p);
        if (y == 0) return new ExPoint(0, 0);
        var x = ArcSinXp(p) / Math.PI;
        if (y > 0) return new ExPoint(x * 16, 10 - 32 / y);
        else return new ExPoint(x * 16, 0);
    }

    public static ExPoint Schnecke2(ExPoint p, ExPoint p0, ref bool brk)
    {
        var y = PLength(p);
        if (y == 0) return new ExPoint(0, 0);
        var x = ArcSinXp(p);
        return new ExPoint(x * 32 / Math.PI, y + (x * 4 / Math.PI) - 6);
    }

    public static ExPoint Strudel(ExPoint p, ExPoint p0, ref bool brk)
    {
        var y = PLength(p);
        if (y == 0) return new ExPoint(0, 0);
        var x = ArcSinXp(p);
        var ang = x + Math.PI - Math.PI * Math.Pow((y - 1) / y, 2);
        return new ExPoint(Math.Sin(ang) * y, Math.Cos(ang) * y);
    }

    public static ExPoint Strudel2(ExPoint p, ExPoint p0, ref bool brk)
    {
        const double rm = 12;
        var r = PLength(p);
        double y;
        if (Math.Abs(r) < rm) y = (1 + Math.Cos(r * Math.PI / rm)) * Math.PI * 0.25;
        else y = 0;
        if (r == 0) return new ExPoint(0, 0);
        var ny = Math.Cos(y) * p.Y - Math.Sin(y) * p.X;
        var nx = Math.Sin(y) * p.Y + Math.Cos(y) * p.X;
        return new ExPoint(nx, ny);
    }

    public static ExPoint Wobble2(ExPoint p, ExPoint p0, ref bool brk)
    {
        var y = PLength(p);
        return new ExPoint(p.X + Math.Sin(p.Y * Math.PI / 3) * 0.2 * y,
        p.Y + Math.Cos(p.X * Math.PI / 3) * 0.2 * y);
    }

    public static ExPoint Wobble3(ExPoint p, ExPoint p0, ref bool brk)
    {
        const double rm = 12;
        var r = PLength(p);
        double x;
        if (Math.Abs(r) < rm) x = (1 + Math.Cos(r * Math.PI / rm)) * 10;
        else x = 0;
        return new ExPoint(p.X + Math.Sin(p.Y * Math.PI / 3) * 0.2 * x,
        p.Y + Math.Cos(p.X * Math.PI / 3) * 0.2 * x);
    }

    public static ExPoint JuliaStep(ExPoint p, ExPoint p0, ref bool brk)
    {
        if ((p.X * p.X * p.Y * p.Y) > 8000) return p;
        return new ExPoint(Math.Pow(p.X * 0.2 - 0.7, 2) * 5 - Math.Pow(p.Y * 0.2 + 0.3, 2) * 5,
        (p.Y * 0.2 + 0.3) * (p.X * 0.2 - 0.7));
    }

    public static ExPoint MandelbrStep(ExPoint p, ExPoint p0, ref bool brk)
    {
        const double fakt = 0.1;
        const double mfakt = 1 / fakt;
        var lsqx = p.X * p.X;
        var lsqy = p.Y * p.Y;
        if (lsqx + 3 * p.X + lsqy > 6 * mfakt * mfakt)
        {
            brk = true;
            return p;
        }
        return new ExPoint((lsqx - lsqy) * fakt + p0.X, 2 * p.Y * p.X * fakt + p0.Y);
    }

    public static ExPoint MandelbrStepN(ExPoint p, ExPoint p0, ref bool brk, int maxRec)
    {
        var res = p;
        var k = 1;
        while (k < maxRec && !brk)
        {
            res = MandelbrStep(res, p0, ref brk);
            k++;
        }
        return res;
    }

    public static ExPoint MandelbrFull(ExPoint p, ExPoint p0, ref bool brk, int maxRec)
    {
        const double fakt = 0.1;
        const double mfakt = 1 / fakt;
        int lc = 0;
        double lsqx = 0, lsqy = 0;
        var p1 = new ExPoint(0, 0);
        while ((lsqx + 3 * p1.X + lsqy < 6 * mfakt * mfakt) && (lc < maxRec))
        {
            p1 = new ExPoint((lsqx - lsqy) * fakt + p0.X, 2 * p1.Y * p1.X * fakt + p0.Y);
            lsqx = p1.X * p1.X;
            lsqy = p1.Y * p1.Y;
            lc++;
        }
        if (lc < maxRec)
        {
            var dlc = 1.0 / maxRec;
            return new ExPoint(Math.Sin(lc * 0.1) * (maxRec - lc) * dlc,
            Math.Cos(lc * 0.1) * (maxRec - lc) * dlc);
        }
        return p1;
    }

    public static ExPoint Rotat(ExPoint p, double r)
    => new ExPoint(Math.Sin(r) * p.Y + Math.Cos(r) * p.X,
    -Math.Sin(r) * p.X + Math.Cos(r) * p.Y);

    public static ExPoint Kugel(ExPoint p, ExPoint p0, ref bool brk)
    {
        const double kippDeg = -30;
        const double drehDeg = -40;
        const double degrPart = 0.005555555555555556; //1/180
        const double rm = 12;
        var r = PLength(p);
        if (Math.Abs(r) > rm) return p;
        p = Rotat(p, Math.PI * kippDeg * degrPart);
        var zm = Math.Sqrt(rm * rm - p.Y * p.Y);
        double z = Math.Abs(zm) >= Math.Abs(p.X) ? Math.Sqrt(zm * zm - p.X * p.X) : 0;
        var p2 = Rotat(new ExPoint(p.Y, z), Math.PI * drehDeg * degrPart);
        var p3 = new ExPoint(ArcSinXp(new ExPoint(p.X, p2.Y)), ArcSinXp(new ExPoint(p2.X, Math.Sqrt(p2.Y * p2.Y + p.X * p.X))));
        return new ExPoint(10 - 4 * p3.X / Math.PI * (int)(rm * 0.5),
        p3.X / Math.PI * 4 - 4 * p3.Y / Math.PI * (int)(rm * 0.5) + 12);
    }
}
