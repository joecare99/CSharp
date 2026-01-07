using SharpHack.Base.Model;
using System.Collections;

namespace SharpHack.Base.Interfaces
{
    public interface IMap : IEnumerable
    {
        Tile this[Point p] { get; }
        Tile this[int x, int y] { get; }

        int Height { get; }
        int Width { get; }

        (int X, int Y) GetOldPos(int x, int y);
        bool IsValid(int x, int y);
        bool IsValid(Point p);
    }
}