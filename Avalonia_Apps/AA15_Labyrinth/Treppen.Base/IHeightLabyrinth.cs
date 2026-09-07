using System;
using System.Drawing;
using MathLibrary.TwoDim;

namespace Treppen.Base;

/// <summary>
/// Contract for height labyrinth engine to enable DI and testing.
/// </summary>
public interface IHeightLabyrinth
{
    /// <summary>
    /// Gets or sets the bounds of the labyrinth grid.
    /// </summary>
    Rectangle Dimension { get; set; }

    /// <summary>
    /// Gets the height at the specified grid coordinate.
    /// </summary>
    /// <param name="x">The horizontal grid coordinate.</param>
    /// <param name="y">The vertical grid coordinate.</param>
    /// <returns>The height at the specified coordinate, or zero outside the grid.</returns>
    int this[int x, int y] { get; }

    /// <summary>
    /// Calculates the base height for the specified grid coordinate.
    /// </summary>
    /// <param name="x">The horizontal grid coordinate.</param>
    /// <param name="y">The vertical grid coordinate.</param>
    /// <returns>The base height for the coordinate.</returns>
    int BaseLevel(int x, int y);

    /// <summary>
    /// Generates the height values for the configured labyrinth grid.
    /// </summary>
    void Generate();

    /// <summary>
    /// Occurs when the value of a grid cell is updated during generation.
    /// </summary>
    event Action<object, Point>? UpdateCell;
}
