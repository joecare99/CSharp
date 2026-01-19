using Arkanoid.Base.Models;
using System;
using System.Linq;

namespace Arkanoid.Base;

public class GameEngine
{
    public GameState State { get; } = new();

    public GameEngine()
    {
        CreateDefaultLevel();
    }

    public void MovePaddle(float direction, float deltaTime)
    {
        var dx = State.Paddle.Speed * direction * deltaTime;
        var newX = Math.Clamp(State.Paddle.Position.X + dx, 0, State.FieldWidth - State.Paddle.Width);
        State.Paddle.Position = new Vector2(newX, State.Paddle.Position.Y);
    }

    public void Update(float deltaTime)
    {
        if (State.IsGameOver) return;

        State.Ball.Position = new Vector2(
            State.Ball.Position.X + State.Ball.Velocity.X * deltaTime,
            State.Ball.Position.Y + State.Ball.Velocity.Y * deltaTime);

        HandleCollisions();
    }

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

    private void ResetBallAndPaddle()
    {
        State.Paddle.Position = new Vector2(State.FieldWidth / 2 - State.Paddle.Width / 2, State.FieldHeight - 2);
        State.Ball.Position = new Vector2(State.FieldWidth / 2, State.FieldHeight / 2);
        State.Ball.Velocity = new Vector2(15, -20);
    }

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
