using System;
using System.Numerics;
using AsteroidsModern.Engine.Abstractions;

namespace AsteroidsModern.Engine.Services;

/// <summary>
/// TitleScreen encapsulates menu navigation and simple UI state (options/highscore/credits).
/// It is fully input-driven and renders itself via IRenderContext.
/// Exposes events for transitioning into gameplay.
/// </summary>
public sealed class TitleScreen
{
    public event Action? StartGameRequested;

    public Vector2 WorldSize { get; set; } = new(800, 600);

    public int StartLives { get; private set; } = 3;
    public bool SoundEnabled { get; private set; } = true;

    public int HighScore { get; private set; }
    private int _lastScore;

    public enum Screen { Title, Options, Highscores, Credits }
    private Screen _screen = Screen.Title;
    private int _titleIndex = 0;
    private int _optionsIndex = 0;

    // Edge detection
    private bool _leftPrev, _rightPrev, _firePrev, _thrustPrev;

    public void SetLastScore(int score)
    {
        _lastScore = score;
        if (score > HighScore) HighScore = score;
        _screen = Screen.Title;
    }

    public void Update(IGameInput input)
    {
        bool leftNow = input.IsDown(GameKey.Left);
        bool rightNow = input.IsDown(GameKey.Right);
        bool fireNow = input.IsDown(GameKey.Fire);
        bool thrustNow = input.IsDown(GameKey.Thrust);

        bool leftPressed = leftNow && !_leftPrev;
        bool rightPressed = rightNow && !_rightPrev;
        bool firePressed = fireNow && !_firePrev;
        bool thrustPressed = thrustNow && !_thrustPrev;

        _leftPrev = leftNow; _rightPrev = rightNow; _firePrev = fireNow; _thrustPrev = thrustNow;

        switch (_screen)
        {
            case Screen.Title:
                if (leftPressed) _titleIndex = (_titleIndex + 3) % 4;
                if (rightPressed) _titleIndex = (_titleIndex + 1) % 4;
                if (firePressed)
                {
                    switch (_titleIndex)
                    {
                        case 0: StartGameRequested?.Invoke(); break;
                        case 1: _screen = Screen.Options; break;
                        case 2: _screen = Screen.Highscores; break;
                        case 3: _screen = Screen.Credits; break;
                    }
                }
                if (thrustPressed) StartGameRequested?.Invoke();
                break;

            case Screen.Options:
                if (leftPressed || rightPressed)
                {
                    if (_optionsIndex == 0)
                    {
                        SoundEnabled = !SoundEnabled;
                        _optionsIndex = 1;
                    }
                    else
                    {
                        StartLives += rightPressed ? 1 : -1;
                        StartLives = Math.Clamp(StartLives, 1, 9);
                        _optionsIndex = 0;
                    }
                }
                if (thrustPressed) _screen = Screen.Title;
                break;

            case Screen.Highscores:
            case Screen.Credits:
                if (thrustPressed) _screen = Screen.Title;
                break;
        }
    }

    public void Render(IRenderContext ctx)
    {
        switch (_screen)
        {
            case Screen.Title: RenderTitle(ctx); break;
            case Screen.Options: RenderOptions(ctx); break;
            case Screen.Highscores: RenderHighscores(ctx); break;
            case Screen.Credits: RenderCredits(ctx); break;
        }
    }

    private void RenderTitle(IRenderContext ctx)
    {
        var center = WorldSize / 2f;
        ctx.DrawText("ASTEROIDS MODERN", center + new Vector2(-160, -120), Color.White, 28f);
        ctx.DrawText("Wähle mit Links/Rechts, bestätige mit Feuer", center + new Vector2(-220, -80), Color.Gray, 14f);

        string[] items = ["Start", "Optionen", "Highscores", "Credits"];
        float startX = center.X - 220;
        for (int i = 0; i < items.Length; i++)
        {
            var color = (i == _titleIndex) ? Color.White : Color.Gray;
            ctx.DrawText(items[i], new Vector2(startX + i * 140, center.Y), color, 20f);
        }

        ctx.DrawText($"Highscore: {HighScore}", center + new Vector2(-80, 60), Color.White, 16f);
        if (_lastScore > 0)
            ctx.DrawText($"Letzter Score: {_lastScore}", center + new Vector2(-90, 90), Color.Gray, 14f);
    }

    private void RenderOptions(IRenderContext ctx)
    {
        var center = WorldSize / 2f;
        ctx.DrawText("OPTIONEN", center + new Vector2(-70, -120), Color.White, 26f);
        ctx.DrawText("Links/Rechts: Wert ändern, Schub: Zurück", center + new Vector2(-230, -80), Color.Gray, 14f);

        string[] labels =
        [
            $"Sound: {(SoundEnabled ? "Ein" : "Aus")}",
            $"Start-Leben: {StartLives}"
        ];

        float startY = center.Y - 10;
        for (int i = 0; i < labels.Length; i++)
        {
            var color = (i == _optionsIndex) ? Color.White : Color.Gray;
            ctx.DrawText(labels[i], new Vector2(center.X - 100, startY + i * 30), color, 18f);
        }

        ctx.DrawText("Schub = Zurück", center + new Vector2(-60, 80), Color.Gray, 14f);
    }

    private void RenderHighscores(IRenderContext ctx)
    {
        var center = WorldSize / 2f;
        ctx.DrawText("HIGHSCORES", center + new Vector2(-80, -60), Color.White, 26f);
        ctx.DrawText($"Highscore: {HighScore}", center + new Vector2(-70, -10), Color.White, 20f);
        ctx.DrawText($"Letzter Score: {_lastScore}", center + new Vector2(-80, 20), Color.Gray, 18f);
        ctx.DrawText("Schub = Zurück", center + new Vector2(-60, 80), Color.Gray, 14f);
    }

    private void RenderCredits(IRenderContext ctx)
    {
        var center = WorldSize / 2f;
        ctx.DrawText("CREDITS", center + new Vector2(-45, -80), Color.White, 26f);
        ctx.DrawText("Spielidee: Asteroids Hommage", center + new Vector2(-140, -20), Color.Gray, 18f);
        ctx.DrawText("Code & Engine: Modern Engine Demo", center + new Vector2(-160, 10), Color.Gray, 18f);
        ctx.DrawText("Schub = Zurück", center + new Vector2(-60, 80), Color.Gray, 14f);
    }
}
