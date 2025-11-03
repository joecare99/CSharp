using System;
using System.Drawing;
using MathLibrary.TwoDim;

namespace Treppen.Base;

/// <summary>
/// Contract for height labyrinth engine to enable DI and testing.
/// </summary>
public interface IHeightLabyrinth
{
    Rectangle Dimension { get; set; }
    int this[int x, int y] { get; }
    int BaseLevel(int x, int y);
    void Generate();
    event Action<object, Point>? UpdateCell;
}
