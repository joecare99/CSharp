using System;
using System.Collections.Generic;
using System.Numerics;
using AsteroidsModern.Engine.Abstractions;
using AsteroidsModern.Engine.Model.Interfaces;
using AsteroidsModern.Engine.Services.Interfaces;

namespace AsteroidsModern.Engine.Services;

/// <summary>
/// GameWorld orchestrates the high level game flow between the Title UI and the gameplay simulation.
/// - Delegates title/menu input & rendering to TitleScreen
/// - Delegates physics, collisions, HUD and scoring to GameSimulation
/// - Bridges events (start game, game over) between both
/// </summary>
public sealed class GameWorld(IRandom rand) : IGameWorld
{
    private enum WorldState { Title, Playing }

    private readonly TitleScreen _title = new();
    private readonly GameSimulation _simulation = new(rand);

    private WorldState _state = WorldState.Playing; // keep default for tests; UI calls ShowTitle()

    public IShip Ship => _simulation.Ship;
    public IReadOnlyList<IEntity> Asteroids => _simulation.Asteroids;
    public IReadOnlyList<ISLEntity> Bullets => _simulation.Bullets;
    public IReadOnlyList<ISLEntity> Dust => _simulation.Dust;

    public Vector2 WorldSize
    {
        get => _simulation.WorldSize;
        set
        {
            _simulation.WorldSize = value;
            _title.WorldSize = value;
        }
    }

    public void ShowTitle() => _state = WorldState.Title;

    public void Reset() => _simulation.Reset();

    public void Update(IGameInput input, ITimeProvider time, ISound sound)
    {
        // Hook events lazily (idempotent)
        EnsureWiring();

        switch (_state)
        {
            case WorldState.Title:
                _title.Update(input);
                break;

            case WorldState.Playing:
                _simulation.Update(input, time, sound, _title.SoundEnabled);
                break;
        }
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Clear(Color.Black);
        switch (_state)
        {
            case WorldState.Title:
                _title.Render(ctx);
                break;
            case WorldState.Playing:
                _simulation.Render(ctx);
                break;
        }
    }

    private bool _wired;
    private void EnsureWiring()
    {
        if (_wired) return;
        _wired = true;

        _title.StartGameRequested += OnStartGameRequested;
        _simulation.GameOver += OnGameOver;
    }

    private void OnStartGameRequested()
    {
        _simulation.StartGame(_title.StartLives);
        _state = WorldState.Playing;
    }

    private void OnGameOver(int finalScore)
    {
        _title.SetLastScore(finalScore);
        _state = WorldState.Title;
    }
}
