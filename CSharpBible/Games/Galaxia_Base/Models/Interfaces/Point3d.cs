using System;

namespace Galaxia.Models.Interfaces;

public struct Point3d
{
    internal static readonly Point3d Zero;

    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    public Point3d(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public override string ToString() => $"({X}, {Y}, {Z})";
    public override bool Equals(object? obj)
    {
        if (obj is Point3d other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }
        return false;
    }

    public int DistanceTo(Point3d other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
    }
}