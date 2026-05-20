using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpHack.Persist;
using SharpHack.WPF2D.Services;
using SharpHack.ViewModel;

namespace SharpHack.WPF2D.ViewModels;

public sealed record TilePaletteEntry(DisplayTile Tile, string Name, int Index);

/// <summary>
/// Main window view model.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly Func<LayeredGameViewModel> _createGame;
    private readonly Func<RestoreGameState, LayeredGameViewModel> _createRestoredGame;
    private readonly IGameSaveLoadService _saveLoadService;
    private LayeredGameViewModel _game;
    public RelayCommand StartNewRunCommand { get; }
    public RelayCommand SaveGameCommand { get; }
    public RelayCommand LoadGameCommand { get; }

    private string _statusMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="game">The game view model.</param>
    /// <param name="createGame">Factory for creating a fresh game view model.</param>
    public MainViewModel(
        LayeredGameViewModel game,
        Func<LayeredGameViewModel> createGame,
        Func<RestoreGameState, LayeredGameViewModel> createRestoredGame,
        GamePersist gamePersist)
    {
        _createGame = createGame ?? throw new ArgumentNullException(nameof(createGame));
        _createRestoredGame = createRestoredGame ?? throw new ArgumentNullException(nameof(createRestoredGame));
        _saveLoadService = new WpfGameSaveLoadService(
            gamePersist ?? throw new ArgumentNullException(nameof(gamePersist)),
            () => Game,
            restore => Game = _createRestoredGame(restore),
            ReportStatus);
        StartNewRunCommand = new RelayCommand(StartNewRun, CanStartNewRun);
        SaveGameCommand = new RelayCommand(SaveGame, CanSaveGame);
        LoadGameCommand = new RelayCommand(LoadGame, CanLoadGame);
        Game = game;
        TilePalette = Enum.GetValues<DisplayTile>()
            .OrderBy(tile => (int)tile)
            .Select(tile => new TilePaletteEntry(tile, tile.ToString(), (int)tile))
            .ToArray();
    }

    /// <summary>
    /// Gets the game view model.
    /// </summary>
    public LayeredGameViewModel Game
    {
        get => _game;
        private set
        {
            if (ReferenceEquals(_game, value))
            {
                return;
            }

            if (_game != null)
            {
                _game.PropertyChanged -= OnGamePropertyChanged;
                _game.Dispose();
            }

            SetProperty(ref _game, value);
            _game.PropertyChanged += OnGamePropertyChanged;
            StartNewRunCommand.NotifyCanExecuteChanged();
            SaveGameCommand.NotifyCanExecuteChanged();
            LoadGameCommand.NotifyCanExecuteChanged();
        }
    }

    public IReadOnlyList<TilePaletteEntry> TilePalette { get; }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private void OnGamePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(LayeredGameViewModel.RunState) or nameof(LayeredGameViewModel.IsRunning))
        {
            StartNewRunCommand.NotifyCanExecuteChanged();
        }

        if (e.PropertyName is nameof(LayeredGameViewModel.CanSave) or nameof(LayeredGameViewModel.CanLoad))
        {
            SaveGameCommand.NotifyCanExecuteChanged();
            LoadGameCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanStartNewRun()
    {
        return !Game.IsRunning;
    }

    private void StartNewRun()
    {
        Game = _createGame();
    }

    private bool CanSaveGame()
    {
        return _saveLoadService.CanSave;
    }

    private void SaveGame()
    {
        _saveLoadService.Save();
    }

    private bool CanLoadGame()
    {
        return _saveLoadService.CanLoad;
    }

    private void LoadGame()
    {
        _saveLoadService.Load();
    }

    private void ReportStatus(string message)
    {
        StatusMessage = message ?? string.Empty;
    }
}
