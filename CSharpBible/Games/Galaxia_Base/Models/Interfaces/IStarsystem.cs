namespace Galaxia.Models.Interfaces;

public interface IStarsystem
{
    /// <summary>
    /// Gets the name of the Starsystem.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }
    /// <summary>
    /// Gets the position of the starsystem in 3D space.
    /// </summary>
    Point3d Position { get; }
    /// <summary>
    /// Gets the population of the starsystem.
    /// </summary>
    float Population { get; }
    /// <summary>
    /// Gets the amount of resources of the starsystem.
    /// </summary>
    float Resources { get; }
}