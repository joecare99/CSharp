namespace Arkanoid.Base.Models;

/// <summary>
/// Represents a single brick in the game arena, including its position, size, type,
/// durability, and destruction state.
/// </summary>
/// <remarks>
/// A brick can be breakable or unbreakable depending on its <see cref="Type"/>. The
/// <see cref="HitPoints"/> value defines how many hits are required to destroy it.
/// The <see cref="IsDestroyed"/> property reflects whether the brick should be
/// considered removed from play.
/// </remarks>
public class Brick : IGameObject
{
    /// <summary>
    /// Gets or sets the top-left position of the brick in world coordinates.
    /// </summary>
    public Vector2 Position { get; set; } = new(0, 0);

    /// <summary>
    /// Gets or sets the width of the brick in world units.
    /// </summary>
    public float Width { get; set; } = 3f;

    /// <summary>
    /// Gets or sets the height of the brick in world units.
    /// </summary>
    public float Height { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the brick's type, which determines its behavior (e.g., breakable or unbreakable).
    /// </summary>
    public BrickType Type { get; set; }

    /// <summary>
    /// Gets or sets the current hit points of the brick.
    /// </summary>
    /// <remarks>
    /// When this value reaches zero or below, the brick is considered destroyed
    /// unless it is of type <see cref="BrickType.Unbreakable"/>.
    /// </remarks>
    public int HitPoints { get; set; } = 1;

    /// <summary>
    /// Gets a value indicating whether the brick is destroyed and should be removed from play.
    /// </summary>
    /// <remarks>
    /// Unbreakable bricks are never destroyed even if <see cref="HitPoints"/> is zero or negative.
    /// </remarks>
    public bool IsDestroyed => HitPoints <= 0 && Type != BrickType.Unbreakable;
}