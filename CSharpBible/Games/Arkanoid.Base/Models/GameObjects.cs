namespace Arkanoid.Base.Models;

public enum BrickType
{
    Normal,
    Strong,
    Unbreakable
}

public record Vector2(float X, float Y);

public class Paddle
{
    public Vector2 Position { get; set; } = new(10, 20);
    public float Width { get; set; } = 6f;
    public float Speed { get; set; } = 25f; // units per second
}

public class Ball
{
    public Vector2 Position { get; set; } = new(10, 10);
    public Vector2 Velocity { get; set; } = new(10, -10); // units per second
    public float Radius { get; set; } = 0.5f;
}

public class Brick
{
    public Vector2 Position { get; set; }
    public float Width { get; set; } = 3f;
    public float Height { get; set; } = 1f;
    public BrickType Type { get; set; }
    public int HitPoints { get; set; } = 1;
    public bool IsDestroyed => HitPoints <= 0 && Type != BrickType.Unbreakable;
}

public class GameState
{
    public Paddle Paddle { get; } = new();
    public Ball Ball { get; } = new();
    public List<Brick> Bricks { get; } = new();
    public int Score { get; set; }
    public int Lives { get; set; } = 3;
    public float FieldWidth { get; set; } = 80;
    public float FieldHeight { get; set; } = 40;
    public bool IsGameOver { get; set; }
}
