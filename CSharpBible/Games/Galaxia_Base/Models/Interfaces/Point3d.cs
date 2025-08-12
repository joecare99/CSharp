namespace Galaxia.Models.Interfaces;

public struct Point3d
{
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
}