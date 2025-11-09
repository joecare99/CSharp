using System.Numerics;

namespace AsteroidsModern.Engine.Model.Interfaces;

public interface IEntity
{
    Vector2 Position { get; set; }
    Vector2 Velocity { get; set; }
    float Rotation { get; set; }
    float Radius { get; init; }
    bool IsAlive { get; set; }
}