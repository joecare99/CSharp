namespace AsteroidsModern.Engine.Abstractions;

public interface ITimeProvider
{
    double TotalTime { get; }
    double DeltaTime { get; }
}
