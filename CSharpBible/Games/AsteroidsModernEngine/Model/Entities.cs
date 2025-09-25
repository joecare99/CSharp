using AsteroidsModern.Engine.Model.Interfaces;
using System.Numerics;

namespace AsteroidsModern.Engine.Model;

public abstract class Entity : IEntity
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Rotation { get; set; }
    public float Radius { get; init; }
    public bool IsAlive { get; set; } = true;
}

public sealed class Ship : Entity, IShip
{
    public float ThrustPower { get; init; } = 130f;
    public float RotationSpeed { get; init; } = 3.2f;
    public float Drag { get; init; } = 0.995f;
}

public sealed class Asteroid : Entity
{
}

public sealed class Bullet : Entity, ISLEntity
{
    public double Life { get; set; } = 1.2; // seconds
}
public sealed class Dust : Entity , ISLEntity
{
    public double Life { get; set; } = 0.4; // seconds
}
