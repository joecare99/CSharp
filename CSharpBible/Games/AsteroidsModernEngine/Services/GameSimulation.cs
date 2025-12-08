using System;
using System.Collections.Generic;
using System.Numerics;
using AsteroidsModern.Engine.Abstractions;
using AsteroidsModern.Engine.Model;

namespace AsteroidsModern.Engine.Services;

/// <summary>
/// GameSimulation encapsulates game play: physics, input, collisions, scoring, HUD.
/// It renders gameplay and HUD, and exposes a GameOver event.
/// </summary>
public sealed class GameSimulation(IRandom rand)
{
    private readonly List<Asteroid> _asteroids = new();
    private readonly List<Bullet> _bullets = new();
    private readonly List<Dust> _dust = new();
    private readonly IRandom _rand = rand;

    public Ship Ship { get; } = new() { Radius = 12f };
    public IReadOnlyList<Asteroid> Asteroids => _asteroids;
    public IReadOnlyList<Bullet> Bullets => _bullets;
    public IReadOnlyList<Dust> Dust => _dust;

    public Vector2 WorldSize { get; set; } = new(800, 600);

    public int Score { get; private set; }
    public int Lives { get; private set; }

    public event Action<int>? GameOver; // final score

    public void StartGame(int startLives)
    {
        Score = 0;
        Lives = Math.Max(1, startLives);
        Reset();
    }

    public void Reset()
    {
        _asteroids.Clear();
        _bullets.Clear();
        Ship.Position = WorldSize / 2f;
        Ship.Velocity = Vector2.Zero;
        Ship.Rotation = 0f;

        for (int i = 0; i < 6; i++)
            _asteroids.Add(RandomAsteroid());
    }

    public void Update(IGameInput input, ITimeProvider time, ISound sound, bool soundEnabled)
    {
        float dt = (float)time.DeltaTime;

        if (input.IsDown(GameKey.Quit))
           GameOver?.Invoke(0);

        // Ship control
        if (input.IsDown(GameKey.Left)) Ship.Rotation -= Ship.RotationSpeed * dt;
        if (input.IsDown(GameKey.Right)) Ship.Rotation += Ship.RotationSpeed * dt;
        if (input.IsDown(GameKey.Thrust))
        {
            var dir = new Vector2((float)Math.Cos(Ship.Rotation - MathF.PI / 2f), (float)Math.Sin(Ship.Rotation - MathF.PI / 2f));
            Ship.Velocity += dir * Ship.ThrustPower * dt;
            if (soundEnabled) sound.PlayThrust();
            for (int i = 0; i < 2; i++)
                _dust.Add(new Dust
                {
                    Position = Ship.Position,
                    Velocity = Ship.Velocity + Rotate(new Vector2(0, 100), Ship.Rotation + (0.5f - _rand.NextSingle()) * 0.3f)* (0.5f + _rand.NextSingle())
                });
        }
        Ship.Velocity *= Ship.Drag; // tiny drag
        Ship.Position += Ship.Velocity * dt;
        Wrap(Ship);

        // Fire
        if (input.IsDown(GameKey.Fire))
        {
            TryShoot(time);
            if (soundEnabled) sound.PlayShoot();
        }

        // Update bullets
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            var b = _bullets[i];
            b.Position += b.Velocity * dt;
            b.Life -= time.DeltaTime;
            Wrap(b);
            if (b.Life <= 0) _bullets.RemoveAt(i);
        }

        // Update bullets
        for (int i = _dust.Count - 1; i >= 0; i--)
        {
            var b = _dust[i];
            b.Position += b.Velocity * dt;
            b.Life -= time.DeltaTime;
            Wrap(b);
            if (b.Life <= 0) _dust.RemoveAt(i);
        }

        // Update asteroids
        foreach (var a in _asteroids)
        {
            a.Position += a.Velocity * dt;
            Wrap(a);
        }

        // Collisions: Ship vs Asteroids
        for (int i = _asteroids.Count - 1; i >= 0; i--)
        {
            if (Collides(Ship, _asteroids[i]))
            {
                if (soundEnabled) sound.PlayBang();
                LoseLifeOrGameOver();
                return;
            }
        }

        // Collisions: Bullets vs Asteroids
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            for (int j = _asteroids.Count - 1; j >= 0; j--)
            {
                if (Collides(_bullets[i], _asteroids[j]))
                {
                    if (soundEnabled) sound.PlayBang();
                    Score += GetPointsForAsteroid(_asteroids[j]);
                    _bullets.RemoveAt(i);
                    SplitAsteroid(j);
                    break;
                }
            }
        }
     
        if (_asteroids.Count == 0)
        {
            // Level cleared, spawn new asteroids
            for (int i = 0; i < 6; i++)
                _asteroids.Add(RandomAsteroid());
        }
    }

    public void Render(IRenderContext ctx)
    {
        // Ship triangle
        var p = Ship.Position; float r = Ship.Rotation; float s = 14f;
        Vector2 tip = p + new Vector2(MathF.Cos(r - MathF.PI / 2), MathF.Sin(r - MathF.PI / 2)) * s;
        Vector2 left = p + new Vector2(MathF.Cos(r + MathF.PI * 2 / 3 - MathF.PI / 2), MathF.Sin(r + MathF.PI * 2 / 3 - MathF.PI / 2)) * s * 0.8f;
        Vector2 right = p + new Vector2(MathF.Cos(r - MathF.PI * 2 / 3 - MathF.PI / 2), MathF.Sin(r - MathF.PI * 2 / 3 - MathF.PI / 2)) * s * 0.8f;
        ctx.DrawPolygon(new[] { tip, left, right }, Color.White, 2f);

        foreach (var a in _asteroids)
            ctx.DrawCircle(a.Position, a.Radius, Color.Gray, 2f);

        foreach (var b in _bullets)
            ctx.DrawCircle(b.Position, 2f, Color.White, 2f);

        foreach (var b in _dust)
            ctx.DrawPixel(b.Position, Color.White);

        // HUD
        ctx.DrawText($"Score: {Score}", new Vector2(10, 10), Color.White, 16f);
        ctx.DrawText($"Leben: {Lives}", new Vector2(MathF.Max(10f, WorldSize.X - 140f), 10), Color.White, 16f);
    }

    private void LoseLifeOrGameOver()
    {
        if (Lives > 1)
        {
            Lives--;
            ResetAfterLifeLoss();
        }
        else
        {
            GameOver?.Invoke(Score);
        }
    }

    private void ResetAfterLifeLoss()
    {
        _bullets.Clear();
        Ship.Position = WorldSize / 2f;
        Ship.Velocity = Vector2.Zero;
        Ship.Rotation = 0f;
    }

    private double _lastShotTime = double.NegativeInfinity;
    private void TryShoot(ITimeProvider time)
    {
        if (time.TotalTime - _lastShotTime < 0.150) return;
        _lastShotTime = time.TotalTime;

        var dir = new Vector2((float)Math.Cos(Ship.Rotation - MathF.PI / 2f), (float)Math.Sin(Ship.Rotation - MathF.PI / 2f));
        var b = new Bullet
        {
            Position = Ship.Position + dir * 16f,
            Velocity = Ship.Velocity + dir * 380f,
            Radius = 2f,
        };
        _bullets.Add(b);
    }

    private void SplitAsteroid(int index)
    {
        var a = _asteroids[index];
        _asteroids.RemoveAt(index);
        if (a.Radius > 14f)
        {
            float nr = a.Radius * 0.6f;
            _asteroids.Add(new Asteroid { Position = a.Position, Velocity = Rotate(a.Velocity, +0.5f), Radius = nr });
            _asteroids.Add(new Asteroid { Position = a.Position, Velocity = Rotate(a.Velocity, -0.5f), Radius = nr });
        }
        for (int i = 0; i < 15; i++)
            _dust.Add(new Dust
            {
                Position = a.Position,
                Velocity = a.Velocity + Rotate(new Vector2(1, 0), _rand.NextSingle() * MathF.PI * 2f) * (50f+ _rand.NextSingle()*50f)
            });
    }

    private int GetPointsForAsteroid(Asteroid a)
    {
        if (a.Radius > 26f) return 20;
        if (a.Radius > 18f) return 50;
        return 100;
    }

    private Asteroid RandomAsteroid()
    {
        var pos = new Vector2(_rand.NextSingle() * WorldSize.X, _rand.NextSingle() * WorldSize.Y);
        var vel = new Vector2(_rand.NextSingle() - 0.5f, _rand.NextSingle() - 0.5f) * 60f;
        float radius = 22f + _rand.NextSingle() * 30f;
        return new Asteroid { Position = pos, Velocity = vel, Radius = radius };
    }

    private static bool Collides(Entity a, Entity b)
        => Vector2.DistanceSquared(a.Position, b.Position) < (a.Radius + b.Radius) * (a.Radius + b.Radius);

    private void Wrap(Entity e)
    {
        var pos = e.Position;
        if (pos.X < 0) pos.X += WorldSize.X; else if (pos.X > WorldSize.X) pos.X -= WorldSize.X;
        if (pos.Y < 0) pos.Y += WorldSize.Y; else if (pos.Y > WorldSize.Y) pos.Y -= WorldSize.Y;
        e.Position = pos;
    }

    private static Vector2 Rotate(Vector2 v, float angle)
    {
        float c = MathF.Cos(angle), s = MathF.Sin(angle);
        return new Vector2(v.X * c - v.Y * s, v.X * s + v.Y * c);
    }
}
