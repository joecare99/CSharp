using System;

namespace Galaxia.Models.Interfaces;

#pragma warning disable CS0659 // Typ überschreibt Object.Equals(object o), überschreibt jedoch nicht Object.GetHashCode()
public struct Point3d
#pragma warning restore CS0659 // Typ überschreibt Object.Equals(object o), überschreibt jedoch nicht Object.GetHashCode()
{
    internal static readonly Point3d Zero=new();

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