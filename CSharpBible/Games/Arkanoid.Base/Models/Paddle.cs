using System.Collections.Generic;

namespace Arkanoid.Base.Models;

/// <summary>
/// Represents the player-controlled paddle used to deflect the ball in the game.
/// </summary>
/// <remarks>
/// The paddle is modeled as a horizontal segment positioned in world coordinates.
/// Movement is typically constrained to the horizontal axis by input handling elsewhere.
/// </remarks>
public class Paddle : IGameObject
{
    /// <summary>
    /// Gets or sets the world-space position of the paddle's reference point.
    /// </summary>
    /// <remarks>
    /// The exact interpretation of the reference point (e.g., center or left edge)
    /// is defined by the rendering and collision systems that consume this model.
    /// </remarks>
    public Vector2 Position { get; set; } = new(10, 20);

    /// <summary>
    /// Gets or sets the paddle width expressed in world units.
    /// </summary>
    /// <remarks>
    /// This value is used for collision checks and rendering to determine the paddle's span.
    /// </remarks>
    public float Width { get; set; } = 6f;

    /// <summary>
    /// Gets or sets the movement speed of the paddle in world units per second.
    /// </summary>
    /// <remarks>
    /// Higher values allow faster horizontal movement when input is applied.
    /// </remarks>
    public float Speed { get; set; } = 25f; // units per second
}
