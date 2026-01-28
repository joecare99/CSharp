namespace Arkanoid.Base.Models;

/// <summary>
/// Represents a two-dimensional vector with single-precision floating-point components,
/// commonly used to describe positions, directions, or sizes in 2D space.
/// </summary>
/// <param name="X">
/// The horizontal component of the vector, where positive values typically indicate movement or offset to the right.
/// </param>
/// <param name="Y">
/// The vertical component of the vector, where positive values typically indicate movement or offset downward in screen space,
/// depending on the coordinate system used by the game.
/// </param>
public record Vector2(float X, float Y);