namespace Arkanoid.Base.Models;

/// <summary>
/// Defines the available brick categories used by the game logic, rendering, and collision handling
/// to determine durability and behavior when hit by a ball.
/// </summary>
public enum BrickType
{
    /// <summary>
    /// A standard brick that is destroyed after a single hit.
    /// </summary>
    Normal,

    /// <summary>
    /// A reinforced brick that requires multiple hits to break.
    /// </summary>
    Strong,

    /// <summary>
    /// An indestructible brick that never breaks when hit.
    /// </summary>
    Unbreakable
}
