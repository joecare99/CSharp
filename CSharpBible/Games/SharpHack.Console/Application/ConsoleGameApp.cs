using System;
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleDisplay.View;
using SharpHack.Base.Data;
using SharpHack.Engine;
using SharpHack.Persist;
using Direction = SharpHack.Base.Model.Direction;
using SharpHack.ViewModel;

namespace SharpHack.Console;

/// <summary>
/// Coordinates the console game's life cycle.
/// </summary>
public sealed class ConsoleGameApp
{
    private readonly IConsole _console;
    private readonly GameSetup _setup = new();
    private GameViewModel _viewModel;
    private TileDisplay<DisplayTile> _tileDisplay;
    private Display _miniMap;
    private GameRenderer _renderer;
    private NavigationPrompt _navigationPrompt;

    private bool _autoDoorOpen = true;

    public ConsoleGameApp() : this(new ConsoleProxy())
    {
    }

    public ConsoleGameApp(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        ApplyContext(_setup.Create(_console, CreateSaveLoadService));
    }

    /// <summary>
    /// Runs the main game loop.
    /// </summary>
    public void Run()
    {
        _console.CursorVisible = false;
        _console.Title = "SharpHack";

        var running = true;
        while (running)
        {
            _renderer.Render();
            var key = _console.ReadKey()?.Key ?? ConsoleKey.NoName;
            running = HandleInput(key);
        }
    }

    private bool HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow: _viewModel.Move(Direction.North); break;
            case ConsoleKey.DownArrow: _viewModel.Move(Direction.South); break;
            case ConsoleKey.LeftArrow: _viewModel.Move(Direction.West); break;
            case ConsoleKey.RightArrow: _viewModel.Move(Direction.East); break;
            case ConsoleKey.NumPad7: _viewModel.Move(Direction.NorthWest); break;
            case ConsoleKey.NumPad9: _viewModel.Move(Direction.NorthEast); break;
            case ConsoleKey.NumPad1: _viewModel.Move(Direction.SouthWest); break;
            case ConsoleKey.NumPad3: _viewModel.Move(Direction.SouthEast); break;
            case ConsoleKey.NumPad5:
            case ConsoleKey.OemPeriod:
                _viewModel.Wait();
                break;

            case ConsoleKey.O:
                _viewModel.OpenDoorNearby();
                break;
            case ConsoleKey.C:
                _viewModel.CloseDoorNearby();
                break;
            case ConsoleKey.T:
                _viewModel.ToggleDoorNearby();
                break;

            case ConsoleKey.A:
                ToggleAutoDoor();
                break;

            case ConsoleKey.Enter:
                _viewModel.ExecutePrimaryAction();
                break;

            case ConsoleKey.S:
                SaveRun();
                break;

            case ConsoleKey.L:
                LoadRun();
                break;

            case ConsoleKey.N when !_viewModel.IsRunning:
                StartNewRun();
                break;

            case ConsoleKey.Escape:
                return false;

            case ConsoleKey.G:
                _navigationPrompt.GoRelative();
                break;
        }

        return true;
    }

    private void ToggleAutoDoor()
    {
        _autoDoorOpen = !_autoDoorOpen;
        _viewModel.AutoDoorOpen = _autoDoorOpen;
        _viewModel.Messages.Add($"AutoDoorOpen: {(_autoDoorOpen ? "ON" : "OFF")}");
    }

    private void StartNewRun()
    {
        ApplyContext(_setup.Create(_console, CreateSaveLoadService));
        _viewModel.Messages.Add("New run started.");
    }

    private void SaveRun()
    {
        if (_viewModel.SaveGameCommand.CanExecute(null))
        {
            _viewModel.SaveGameCommand.Execute(null);
        }
    }

    private void LoadRun()
    {
        if (_viewModel.LoadGameCommand.CanExecute(null))
        {
            _viewModel.LoadGameCommand.Execute(null);
            return;
        }

        ReportStatus("No saved run found.");
    }

    private IGameSaveLoadService CreateSaveLoadService(GameSession session)
    {
        return new ConsoleGameSaveLoadService(_setup.DurablePersist, session, ApplyRestoredState, ReportStatus);
    }

    private void ApplyRestoredState(RestoreGameState restoreGameState)
    {
        ApplyContext(_setup.CreateFromRestore(_console, restoreGameState, CreateSaveLoadService));
    }

    private void ReportStatus(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            _viewModel.Messages.Add(message);
        }
    }

    private void ApplyContext(GameContext context)
    {
        _viewModel?.Dispose();
        _viewModel = context.ViewModel;
        _tileDisplay = context.TileDisplay;
        _miniMap = context.MiniMap;
        _renderer = new GameRenderer(_console, _viewModel, _tileDisplay, _miniMap, () => _autoDoorOpen);
        _navigationPrompt = new NavigationPrompt(_console, _viewModel, _renderer);
        _tileDisplay.FncGetTile = _renderer.GetTileAt;
        _viewModel.AutoDoorOpen = _autoDoorOpen;
    }
}
