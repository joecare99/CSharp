using Arkanoid.Base.Models;
using System;
using System.Linq;

namespace Arkanoid.Base;

/// <summary>
/// Provides the core game loop mechanics for the Arkanoid-style game,
/// including state updates, collision handling, and level setup.
/// </summary>
/// <remarks>
/// This engine operates on a simple fixed-field coordinate system and updates
/// the <see cref="GameState"/> in-place. It is intentionally lightweight and
/// deterministic for predictable gameplay behavior.
/// </remarks>
public class GameEngine
{
    /// <summary>
    /// Gets the current mutable game state, including ball, paddle, bricks, score, and lives.
    /// </summary>
    public GameState State { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEngine"/> class
    /// and creates the default level layout.
    /// </summary>
    public GameEngine()
    {
        CreateDefaultLevel();
    }

    /// <summary>
    /// Moves the paddle horizontally according to player input and elapsed time.
    /// </summary>
    /// <param name="direction">
    /// The horizontal input direction, typically in the range [-1, 1],
    /// where negative values move left and positive values move right.
    /// </param>
    /// <param name="deltaTime">The elapsed time since the last update, in seconds.</param>
    /// <remarks>
    /// The paddle position is clamped so it cannot leave the horizontal bounds of the field.
    /// </remarks>
    public void MovePaddle(float direction, float deltaTime)
    {
        var dx = State.Paddle.Speed * direction * deltaTime;
        var newX = Math.Clamp(State.Paddle.Position.X + dx, 0, State.FieldWidth - State.Paddle.Width);
        State.Paddle.Position = new Vector2(newX, State.Paddle.Position.Y);
    }

    /// <summary>
    /// Advances the simulation by the specified time step.
    /// </summary>
    /// <param name="deltaTime">The elapsed time since the last update, in seconds.</param>
    /// <remarks>
    /// If the game is over, no further updates are performed.
    /// This method updates the ball position and resolves collisions.
    /// </remarks>
    public void Update(float deltaTime)
    {
        if (State.IsGameOver) return;

        State.Ball.Position = new Vector2(
            State.Ball.Position.X + State.Ball.Velocity.X * deltaTime,
            State.Ball.Position.Y + State.Ball.Velocity.Y * deltaTime);

        HandleCollisions();
    }

    /// <summary>
    /// Detects and resolves collisions between the ball and walls, paddle, and bricks.
    /// </summary>
    /// <remarks>
    /// - Wall collisions reflect velocity components as needed.<br/>
    /// - Falling below the field reduces lives and resets the ball and paddle.<br/>
    /// - Paddle collisions reflect the ball upward when moving downward.<br/>
    /// - Brick collisions reflect vertical velocity and reduce hit points for breakable bricks.<br/>
    /// - If all bricks are destroyed, the game ends.
    /// </remarks>
    private void HandleCollisions()
    {
        var b = State.Ball;

        // walls
        if (b.Position.X - b.Radius < 0 || b.Position.X + b.Radius > State.FieldWidth)
            State.Ball.Velocity = new Vector2(-b.Velocity.X, b.Velocity.Y);
        if (b.Position.Y - b.Radius < 0)
            State.Ball.Velocity = new Vector2(b.Velocity.X, -b.Velocity.Y);
        if (b.Position.Y - b.Radius > State.FieldHeight)
        {
            State.Lives--;
            if (State.Lives <= 0) State.IsGameOver = true;
            ResetBallAndPaddle();
            return;
        }

        // paddle (simple AABB)
        var p = State.Paddle;
        if (b.Position.Y + b.Radius >= p.Position.Y &&
            b.Position.Y - b.Radius <= p.Position.Y + 1 &&
            b.Position.X >= p.Position.X &&
            b.Position.X <= p.Position.X + p.Width &&
            b.Velocity.Y > 0)
        {
            State.Ball.Velocity = new Vector2(b.Velocity.X, -Math.Abs(b.Velocity.Y));
        }

        // bricks
        foreach (var brick in State.Bricks.ToList())
        {
            if (brick.IsDestroyed) continue;

            if (b.Position.X + b.Radius < brick.Position.X ||
                b.Position.X - b.Radius > brick.Position.X + brick.Width ||
                b.Position.Y + b.Radius < brick.Position.Y ||
                b.Position.Y - b.Radius > brick.Position.Y + brick.Height)
                continue;

            State.Ball.Velocity = new Vector2(b.Velocity.X, -b.Velocity.Y);

            if (brick.Type != BrickType.Unbreakable)
            {
                brick.HitPoints--;
                if (brick.IsDestroyed)
                {
                    State.Score += 10;
                }
            }
        }

        if (State.Bricks.All(br => br.IsDestroyed))
            State.IsGameOver = true;
    }

    /// <summary>
    /// Resets the paddle and ball to their default starting positions and velocity.
    /// </summary>
    /// <remarks>
    /// This is invoked after losing a life or when initializing a level.
    /// </remarks>
    private void ResetBallAndPaddle()
    {
        State.Paddle.Position = new Vector2(State.FieldWidth / 2 - State.Paddle.Width / 2, State.FieldHeight - 2);
        State.Ball.Position = new Vector2(State.FieldWidth / 2, State.FieldHeight / 2);
        State.Ball.Velocity = new Vector2(15, -20);
    }

    /// <summary>
    /// Creates the default level layout consisting of a rectangular grid of bricks.
    /// </summary>
    /// <remarks>
    /// The layout uses a fixed number of rows and columns and distributes bricks evenly
    /// across the field width. All bricks are initialized as breakable with one hit point.
    /// </remarks>
    private void CreateDefaultLevel()
    {
        State.Bricks.Clear();
        var rows = 5;
        var cols = 10;
        var brickWidth = State.FieldWidth / cols;
        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < cols; x++)
            {
                State.Bricks.Add(new Brick
                {
                    Position = new Vector2(x * brickWidth, 2 + y),
                    Width = brickWidth - 0.5f,
                    Height = 1,
                    Type = BrickType.Normal,
                    HitPoints = 1
                });
            }
        }
        ResetBallAndPaddle();
    }
}
