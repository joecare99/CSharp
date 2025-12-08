using System;
using System.Windows;

namespace Asteroids.Model.Types;

public static class PointHelper
{
    public static double dLength(this Point p)
        =>Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2));

    public static double dAngle(this Point p)
        => Math.Atan2(p.Y, p.X);
    public static double Distance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }
}
