namespace AsteroidsModern.Engine.Model.Interfaces;

public interface IShip: IEntity
{
    float ThrustPower { get; init; }
    float RotationSpeed { get; init; }
    float Drag { get; init; }
}