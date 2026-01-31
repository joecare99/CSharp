namespace Arkanoid.Base.Models;
/// <summary>
/// Interface IGameObject
/// </summary>
public interface IGameObject
{
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    Vector2 Position { get; set; }
}