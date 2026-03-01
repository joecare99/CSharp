namespace Arkanoid.Base.Models;
/// <summary>
/// The ball object in the Arkanoid game.
/// </summary>
/// <seealso cref="Arkanoid.Base.Models.IGameObject" />
public class Ball : IGameObject
{
    /// <summary>
    /// Gets or sets the current world-space position of the ball in game units.
    /// </summary>
    /// <remarks>
    /// The position represents the center of the ball and is typically updated each frame
    /// based on <see cref="Velocity"/> and elapsed time.
    /// </remarks>
    public Vector2 Position { get; set; } = new(10, 10);

    /// <summary>
    /// Gets or sets the linear velocity of the ball in game units per second.
    /// </summary>
    /// <remarks>
    /// Positive X moves the ball to the right, and positive Y moves it upward, assuming a
    /// conventional Cartesian coordinate system.
    /// </remarks>
    public Vector2 Velocity { get; set; } = new(10, -10); // units per second

    /// <summary>
    /// Gets or sets the radius of the ball in game units.
    /// </summary>
    /// <remarks>
    /// The radius is used for collision detection and rendering. The ball's diameter is
    /// <c>2 * Radius</c>.
    /// </remarks>
    public float Radius { get; set; } = 0.5f;
}
