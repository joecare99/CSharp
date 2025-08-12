using Galaxia.Models.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace Galaxia.Models;

public class Sector(ISpace space, Point3d position) : ISector
{
    private readonly ISpace _space = space;
    /// <summary>
    /// Gets the name of the sector.
    /// </summary>
    public string Name => $"{(char)((int)'A'+position.X+position.Y*4)}";
    /// <summary>
    /// Gets the color of the sector.
    /// </summary>
    /// <value>The color.</value>
    public Color Color => position.Z < 1 ? Color.Red : Color.Blue;
    /// <summary>
    /// Gets the position of the sector in 3D space.
    /// </summary>
    /// <value>The position.</value>
    public Point3d Position => position;
    /// <summary>
    /// Gets the starsystems contained in the sector.
    /// </summary>
    /// <value>The starsystems.</value>
    public IList<IStarsystem> Starsystems { get; } = [];
}