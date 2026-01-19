using System.Collections.Generic;
using System.Numerics;
using AsteroidsModern.Engine.Abstractions;

namespace AsteroidsModernEngine.Tests;

public sealed class FakeInput : IGameInput
{
    private readonly HashSet<GameKey> _down = new();
    public bool IsDown(GameKey key) => _down.Contains(key);
    public void Set(GameKey key, bool state)
    {
        if (state) _down.Add(key); else _down.Remove(key);
    }
}

public sealed class FakeTime : ITimeProvider
{
    public double TotalTime { get; private set; }
    public double DeltaTime { get; private set; }
    public void Advance(double dt)
    {
        DeltaTime = dt;
        TotalTime += dt;
    }
}

public sealed class FakeSound : ISound
{
    public int ThrustCount { get; private set; }
    public int ShootCount { get; private set; }
    public int BangCount { get; private set; }
    public void PlayThrust() => ThrustCount++;
    public void PlayShoot() => ShootCount++;
    public void PlayBang() => BangCount++;
}

public sealed class SpyRender : IRenderContext
{
    public int ClearCalls { get; private set; }
    public int Polylines { get; private set; }
    public int Circles { get; private set; }
    public int Texts { get; private set; }
    public int Pixels { get; private set; }

    public void Clear(Color color) => ClearCalls++;
    public void DrawPolygon(Vector2[] points, Color color, float thickness = 1f) => Polylines++;
    public void DrawCircle(Vector2 center, float radius, Color color, float thickness = 1f, int segments = 24) => Circles++;

    public void DrawText(string text, Vector2 position, Color color, float fontSize = 12) => Texts++;

    public void DrawPixel(Vector2 center, Color color) => Pixels++;
}
