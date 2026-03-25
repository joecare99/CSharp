namespace Arkanoid.Base.Models;

/// <summary>
/// Represents the complete mutable state of an Arkanoid game session, including
/// player control, ball physics, level geometry, scoring, and terminal flags.
/// </summary>
/// <remarks>
/// This type is intended to be the single source of truth for the game loop,
/// allowing rendering and simulation to read and update a unified model.
/// </remarks>
public class GameState
{
    /// <summary>
    /// Gets the player's paddle, which is responsible for reflecting the ball.
    /// </summary>
    /// <remarks>
    /// The paddle position and size are typically updated by player input and
    /// used during collision checks with the ball.
    /// </remarks>
    public Paddle Paddle { get; } = new();

    /// <summary>
    /// Gets the active ball instance used for movement, collisions, and scoring.
    /// </summary>
    /// <remarks>
    /// The ball's position and velocity are advanced each tick of the game loop
    /// and are evaluated against the field bounds, paddle, and bricks.
    /// </remarks>
    public Ball Ball { get; } = new();

    /// <summary>
    /// Gets the collection of bricks currently present in the playfield.
    /// </summary>
    /// <remarks>
    /// Bricks are typically removed when hit by the ball and may contribute
    /// to the score or trigger power-ups in extended implementations.
    /// </remarks>
    public List<Brick> Bricks { get; } = new();

    /// <summary>
    /// Gets or sets the player's current score.
    /// </summary>
    /// <remarks>
    /// This value is generally increased when the ball destroys bricks or
    /// completes objectives, and may be used for high score tracking.
    /// </remarks>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the number of remaining lives.
    /// </summary>
    /// <remarks>
    /// A life is typically lost when the ball exits the bottom boundary of the field.
    /// When this value reaches zero, the game is considered over.
    /// </remarks>
    public int Lives { get; set; } = 3;

    /// <summary>
    /// Gets or sets the width of the playable field in game units.
    /// </summary>
    /// <remarks>
    /// Used to clamp paddle movement and to reflect the ball from the left and right walls.
    /// </remarks>
    public float FieldWidth { get; set; } = 80;

    /// <summary>
    /// Gets or sets the height of the playable field in game units.
    /// </summary>
    /// <remarks>
    /// Used to determine the top boundary for ball reflection and the bottom
    /// boundary for life loss.
    /// </remarks>
    public float FieldHeight { get; set; } = 40;

    /// <summary>
    /// Gets or sets a value indicating whether the game has ended.
    /// </summary>
    /// <remarks>
    /// This flag is typically set when <see cref="Lives"/> reaches zero or when
    /// all bricks have been cleared, depending on game rules.
    /// </remarks>
    public bool IsGameOver { get; set; }
}
