using System.Numerics;

namespace AsteroidsModern.Engine.Abstractions;

public interface IRenderContext
{
    void Clear(Color color);
    void DrawPolygon(Vector2[] points, Color color, float thickness = 1f);
    void DrawCircle(Vector2 center, float radius, Color color, float thickness = 1f, int segments = 24);
    void DrawPixel(Vector2 center, Color color);
    void DrawText(string text, Vector2 position, Color color, float fontSize = 12f);
}

public readonly record struct Color(byte R, byte G, byte B, byte A=255)
{
    public static readonly Color Black = new(0,0,0);
    public static readonly Color White = new(255,255,255);
    public static readonly Color Gray = new(128,128,128);
}
