using System.Collections.Generic;
using System.Drawing;

namespace Galaxia.Models.Interfaces;

public interface ISector
{
    string Name { get; }
    /// <summary>
    /// Gets the position of the sector in 3D space.
    /// </summary>
    Point3d Position { get; }
    /// <summary>
    /// Gets the starsystems contained in the sector.
    /// </summary>
    IList<IStarsystem> Starsystems { get; }
    Color Color { get; }
}