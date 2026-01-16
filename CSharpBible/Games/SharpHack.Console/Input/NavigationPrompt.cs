using System;
using BaseLib.Interfaces;
using SharpHack.Base.Data;
using SharpHack.Base.Model;
using SharpHack.ViewModel;

namespace SharpHack.Console;

/// <summary>
/// Handles navigation related prompts and commands.
/// </summary>
internal sealed class NavigationPrompt
{
    private readonly IConsole _console;
    private readonly GameViewModel _viewModel;
    private readonly GameRenderer _renderer;

    public NavigationPrompt(IConsole console, GameViewModel viewModel, GameRenderer renderer)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    public void GoRelative()
    {
        var playerPos = _viewModel.Player.Position;
        var (dx, dy) = ReadIntPairAtHud($"GoTo (dx,dy) from [{playerPos.X},{playerPos.Y}]: ");
        var target = new Point(playerPos.X + dx, playerPos.Y + dy);

        if (!_viewModel.Map.IsValid(target))
        {
            _viewModel.Messages.Add("Target is outside of the map.");
            return;
        }

        _viewModel.GoToWorldAsync(target).GetAwaiter().GetResult();
    }

    private (int dx, int dy) ReadIntPairAtHud(string prompt)
    {
        var previousVisibility = _console.CursorVisible;
        var previousFore = _console.ForegroundColor;
        var previousBack = _console.BackgroundColor;

        try
        {
            _console.CursorVisible = true;
            _console.ForegroundColor = ConsoleColor.White;
            _console.BackgroundColor = ConsoleColor.Black;

            while (true)
            {
                _renderer.Render();

                _console.SetCursorPosition(0, ConsoleHudLayout.HudY + 1);
                _console.Write(new string(' ', 79));
                _console.SetCursorPosition(0, ConsoleHudLayout.HudY + 1);
                _console.Write(prompt);

                var inputX = Math.Min(_console.BufferWidth - 1, prompt.Length);
                _console.SetCursorPosition(inputX, ConsoleHudLayout.HudY + 1);

                var input = _console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                var parts = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    continue;
                }

                if (int.TryParse(parts[0], out var dx) && int.TryParse(parts[1], out var dy))
                {
                    return (dx, dy);
                }
            }
        }
        finally
        {
            _console.ForegroundColor = previousFore;
            _console.BackgroundColor = previousBack;
            _console.CursorVisible = previousVisibility;
        }
    }
}
