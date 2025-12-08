using System.Numerics;
using AsteroidsModern.Engine.Abstractions;
using AsteroidsModern.Engine.Model.Interfaces;

namespace AsteroidsModern.Engine.Services.Interfaces;

public interface IGameWorld
{
    IShip Ship { get; }
    IReadOnlyList<IEntity> Asteroids { get; }
    IReadOnlyList<ISLEntity> Bullets { get; }
    IReadOnlyList<ISLEntity> Dust { get; }
    Vector2 WorldSize { get; }

    void Reset();
    void Update(IGameInput input, ITimeProvider time, ISound sound);
    void Render(IRenderContext ctx);
    void ShowTitle();
}
